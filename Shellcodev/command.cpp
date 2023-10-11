#include "repl.h"
#include "color.hpp"

BOOL xorNulls;

// Helper vectors for selecting random registers in .rsf
// TODO: Add those of XMM registers to x64 that are non-mutable by any flag conditions (especially by CF and ZF)
std::vector<std::string> gregs_x64{ "rax", "rbx", "rcx", "rdx", "r8", "r9", "r10", "r11", "r12"};
std::vector<std::string> gregs_x32{ "eax", "ebx", "ecx", "edx"};

void shelldev_print_assembly(unsigned char* encode, size_t size)
{
	printf("assembled (%zu bytes): ", size);

	for (size_t i = 0; i < size; ++i)
		if (encode[i] == 0x0)
			//std::cout << std::hex << dye::light_red("0x") << dye::light_red(static_cast<int>(encode[i])) << " ";
			std::cout << std::hex << dye::light_red("0x00") << " ";
		else 
			//std::cout << std::hex << "0x" << static_cast<int>(encode[i]) << " ";
			// SUGGESTION: The above can also be used in .toshell with hex cast but I am not sure if it won't break int-based size extraction of bytearray
			printf("0x%x, ", encode[i]);

	printf("\n");
}

static BOOL shelldev_command_kernel32(shell_t* sh, std::vector<std::string> parts)
{
	do
	{
		if (parts.size() != 1)
		{
			shelldev_print_errors("Usage: .kernel32 <func>");
			break;
		}

		HMODULE kernel32 = GetModuleHandleA("kernel32.dll");
		FARPROC addr = GetProcAddress(kernel32, parts[0].c_str());

		if (!addr)
		{
			shelldev_print_errors("Unable to find that export!");
			break;
		}

		shelldev_print_good("Kernel32.dll at %p, export located at %p", (LPVOID)kernel32, (LPVOID)addr);

	} while (0);

	return TRUE;
}

static BOOL shelldev_command_shellcode(shell_t* sh, std::vector<std::string> parts)
{
	do
	{
		std::string fixed = join(parts, "");
		std::string bin_str = from_hex(std::begin(fixed), std::end(fixed));
		std::vector<std::uint8_t> bytes(std::begin(bin_str), std::end(bin_str));

		if (bytes.size() == 0)
		{
			shelldev_print_errors("Usage: .shellcode hexdata");
			break;
		}

		if (!shelldev_write_shellcode(sh, &bytes[0], bytes.size()))
		{
			shelldev_print_errors("Unable to allocate shellcode!");
			return TRUE;
		}

		shelldev_debug_shellcode(sh);
		shelldev_print_registers(sh);

	} while (0);

	return TRUE;
}

static BOOL shelldev_command_peb(shell_t* sh, std::vector<std::string> parts, std::vector<asm_t>* assemblies)
{
	std::string instructions;
#ifdef _M_X64
	// xor eax, eax
	// mov rax, gs:[eax+0x60]
	// unsigned char bytes[] = { 0x31, 0xc0, 0x65, 0x48, 0x8b, 0x40, 0x60 };
	instructions = "xor eax, eax;mov rax, gs:[eax+0x60]";
#elif defined(_M_IX86)
	// xor eax, eax
	// mov eax, fs:[eax+0x30]
	// unsigned char bytes[] = { 0x31, 0xC0, 0x64, 0x8B, 0x40, 0x30 };
	instructions = "xor eax, eax;mov eax, fs:[eax+0x30]";
#endif

	shelldev_run_shellcode(sh, instructions, assemblies);

	return TRUE;
}


static BOOL shelldev_command_abort(shell_t* sh, std::vector<asm_t>* assemblies)
{
	// TODO: I will add loop binder and unbinder so that below works + add exit routine
	std::string instructions;
#ifdef _M_X64
	instructions = "push rbx; xor rbx, rbx; cmp rax, rbx; jne exitlogic; pop rbx; exitlogic:";
#elif defined(_M_IX86)
	instructions = "push ebx; xor ebx, ebx; cmp eax, ebx; jne exitlogic; pop ebx; exitlogic:";
#endif

	shelldev_run_shellcode(sh, instructions, assemblies);

	return TRUE;
}

static BOOL shelldev_command_allocate(shell_t* sh, std::vector<std::string> parts)
{
	do
	{
		if (parts.size() != 1)
		{
			shelldev_print_errors("Usage: .alloc size");
			break;
		}

		size_t size = atol(parts[0].c_str());

		if (size == 0)
		{
			shelldev_print_errors("Usage: .alloc size");
			break;
		}

		LPVOID addr = VirtualAllocEx(
			sh->procInfo.hProcess,
			NULL,
			size,
			MEM_COMMIT,
			PAGE_EXECUTE_READWRITE
		);

		if (!addr)
		{
			shelldev_print_errors("Unable to allocate memory!");
			break;
		}

		shelldev_print_good("Allocated RWX memory at %p (size: %d)", addr, size);
	} while (0);

	return TRUE;
}

static BOOL shelldev_command_write(shell_t* sh, std::vector<std::string> parts)
{

	do
	{
		if (parts.size() < 2)
		{
			shelldev_print_errors("Usage: .write addr hexdata");
			break;
		}


		unsigned long long x = 0;
		std::istringstream iss(parts[0]);
		iss >> std::hex >> x;
		parts.erase(parts.begin());

		std::string fixed = join(parts, "");
		//separate<2, ' '>(fixed);
		std::string bin_str = from_hex(std::begin(fixed), std::end(fixed));
		std::vector<std::uint8_t> bytes(std::begin(bin_str), std::end(bin_str));

		if (x == 0 || bytes.size() == 0)
		{
			shelldev_print_errors("Usage: .write addr hexdata");
			break;
		}

		SIZE_T nBytes;

		if (!WriteProcessMemory(
			sh->procInfo.hProcess,
			(LPVOID)x,
			&bytes[0],
			bytes.size(),
			&nBytes
		))
		{
			shelldev_print_errors("Unable to write hex data!");
			break;
		}

		shelldev_print_good("Wrote %d bytes to %p", nBytes, (LPVOID)x);
		shelldev_print_bytes(&bytes[0], (int)bytes.size(), x);

	} while (0);


	return TRUE;
}

static BOOL shelldev_command_read(shell_t* sh, std::vector<std::string> parts)
{
	do
	{
		if (parts.size() != 2)
		{
			shelldev_print_errors("Usage: .read addr size");
			break;
		}

		size_t size = atol(parts[1].c_str());

		unsigned long long x = 0;
		std::istringstream iss(parts[0]);
		iss >> std::hex >> x;

		if (size == 0 || x == 0)
		{
			shelldev_print_errors("Usage: .read addr size");
			break;
		}

		std::vector<unsigned char> bytes;
		bytes.reserve(size);

		SIZE_T nBytes;
		
		if (!ReadProcessMemory(
			sh->procInfo.hProcess,
			(LPCVOID)x,
			&bytes[0],
			size,
			&nBytes
		))
		{
			shelldev_print_errors("Unable to read from address: %p!", (LPVOID)x);
			break;
		}

		shelldev_print_bytes(&bytes[0], (int)nBytes, x);

	} while (0);

	return TRUE;
}

static BOOL shelldev_command_loadlibrary(shell_t* sh, std::vector<std::string> parts)
{
	do
	{
		if (parts.size() < 1)
		{
			shelldev_print_errors("The path is missing!");
			break;
		}

		std::string dll = join(parts, "");

		LPVOID pStr = VirtualAllocEx(
			sh->procInfo.hProcess,
			NULL,
			dll.length() + 1,
			MEM_COMMIT,
			PAGE_READWRITE);

		if (!pStr)
		{
			shelldev_print_errors("Unable to allocate DLL path!");
			break;
		}

		SIZE_T nBytes;

		if (!WriteProcessMemory(
			sh->procInfo.hProcess,
			pStr,
			&dll[0],
			dll.length() + 1,
			&nBytes
		))
		{
			shelldev_print_errors("Unable to write DLL path!");
			break;
		}

		DWORD dwThreadId;

		HANDLE hThread = CreateRemoteThread(
			sh->procInfo.hProcess,
			NULL,
			0,
			(LPTHREAD_START_ROUTINE)LoadLibraryA,
			pStr,
			0,
			&dwThreadId);

		if (hThread == INVALID_HANDLE_VALUE)
		{
			shelldev_print_errors("Failed to call LoadLibraryA().");
			break;
		}

		shelldev_print_good("LoadLibraryA() called for %s!", dll.c_str());

	} while (0);

	return TRUE;
}

BOOL shelldev_command_registers(shell_t* sh, std::vector<std::string> parts)
{
	shelldev_print_registers_all(sh);
	return TRUE;
}

static BOOL shelldev_command_reset_assemblies(std::vector<asm_t>* assemblies)
{
	shelldev_print_good("Resetting the assemblies");
	assemblies->clear();
	return TRUE;
}

static BOOL shelldev_command_reset(shell_t* sh)
{
	shelldev_print_good("Resetting the environment");
	TerminateProcess(sh->procInfo.hProcess, 0);
	DebugActiveProcessStop(sh->procInfo.dwProcessId);
	return TRUE;
}

static BOOL shelldev_command_fixip(shell_t* sh)
{
	shelldev_print_good("Trying to fix the instruction pointer");
	CONTEXT ctx = { 0 };
	ctx.ContextFlags = CONTEXT_ALL;
	GetThreadContext(sh->procInfo.hThread, &ctx);

#ifdef _M_X64
	ctx.Rip = ctx.Rip - 1;
#elif defined(_M_IX86)
	ctx.Eip = ctx.Eip - 1;
#endif
	SetThreadContext(sh->procInfo.hThread, &ctx);
	return TRUE;
}

static BOOL shelldev_list(std::vector<asm_t>* assemblies)
{
	int count = 0;
	for (asm_t assembly : *assemblies)
	{
		std::cout << std::dec << dye::light_green(count) << ".\t";
		std::cout << assembly.instruction;

		for (int i = 0; i < (24 - assembly.instruction.size()); i++)
			std::cout << " ";

		std::cout << dye::light_green("|\t");

		for (unsigned char byte : assembly.bytes)
			if (byte == 0x0)
				std::cout << std::hex << dye::red("0x") << dye::red(static_cast<int>(byte)) << " ";
			else
				std::cout << std::hex << "0x" << static_cast<int>(byte) << " ";

		std::cout << std::endl;
		count++;
	}

	if (count == 0) {
		shelldev_print_errors("No instructions inserted");
	}

	return TRUE;
}

static BOOL shelldev_edit(shell_t* sh, std::vector<asm_t>* assemblies, std::vector<std::string> parts)
{
	if (!is_number(parts[0]))
		return FALSE;

	std::cout << "Editing line: " << dye::light_green(parts[0]) << std::endl;
	std::cout << "Editing instruction: " << dye::light_green(assemblies->at(std::stoi(parts[0])).instruction) << std::endl;
	std::cout << "Type '-' to quit editing" << std::endl;

	std::string input = shelldev_read();
	if (input == "-")
		return TRUE;

	assemblies->at(std::stoi(parts[0])).instruction = input;

	if (!shelldev_run_shellcode(sh, assemblies))
		return FALSE;

	return TRUE;
}

static BOOL shelldev_swap(shell_t* sh, std::vector<asm_t>* assemblies, std::vector<std::string> parts)
{
	if (parts.size() != 2) {
		shelldev_print_errors("Usage: .swap <src> <dst>");
		return FALSE;
	}

	if (!is_number(parts[0])) {
		return FALSE;
	}

	if (!is_number(parts[1])) {
		return FALSE;
	}

	std::string src_instr = assemblies->at(std::stoi(parts[0])).instruction;
	std::string dst_instr = assemblies->at(std::stoi(parts[1])).instruction;

	std::cout << "[*] " << dye::light_purple(src_instr)<< " <-> " << dye::purple_on_black(src_instr) << std::endl;

	assemblies->at(std::stoi(parts[0])).instruction = dst_instr;
	assemblies->at(std::stoi(parts[1])).instruction = src_instr;

	if (!shelldev_run_shellcode(sh, assemblies)) {
		return FALSE;
	}


	return TRUE;
}

static BOOL shelldev_toshell(std::vector<asm_t>* assemblies, std::vector<std::string> parts)
{
	if (parts[0] == "c")
	{
		int count = 0;
		std::cout << "unsigned char shellcode[] = {" << std::endl;
		for (int i = 0; i < assemblies->size(); i++)
		{
			for (int j = 0; j < assemblies->at(i).instruction.size(); j++)
			{
				if (count % 12 == 0)
					printf("\n");
				else
					printf("0x%x, ", assemblies->at(i).instruction[j]);

				count++;
			}
		}
		std::cout << "};" << std::endl;
	}
	else if (parts[0] == "cs")
	{
		int count = 0;
		std::cout << "byte[] shellcode = {" << std::endl;
		for (int i = 0; i < assemblies->size(); i++)
		{
			for (int j = 0; j < assemblies->at(i).instruction.size(); j++)
			{
				if (count % 12 == 0)
					printf("\n");
				else
					printf("0x%x, ", assemblies->at(i).instruction[j]);

				count++;
			}
		}
		std::cout << "};" << std::endl;
	}
	else if (parts[0] == "py")
	{
		int count = 0;
		std::cout << "shellcode = (b\"";
		for (int i = 0; i < assemblies->size(); i++)
		{
			for (int j = 0; j < assemblies->at(i).instruction.size(); j++)
			{
				printf("\\x%x", assemblies->at(i).instruction[j]);
			}
		}
		std::cout << "\")" << std::endl;
	}
	else if (parts[0] == "raw")
	{
		for (int i = 0; i < assemblies->size(); i++) {
			for (int j = 0; j < assemblies->at(i).instruction.size(); j++) {
				printf("%X", assemblies->at(i).instruction[j]);
			}
		}
		printf("\n");
	}

	return TRUE;
}

static BOOL shelldev_command_delete(shell_t* sh, std::vector<asm_t>* assemblies, std::vector<std::string> parts) 
{
	assemblies->erase(assemblies->begin() + std::stoi(parts[0]));

	shelldev_run_shellcode(sh, assemblies);

	return TRUE;
}

static BOOL shelldev_command_insert(shell_t* sh, std::vector<asm_t>* assemblies, std::vector<std::string> parts)
{
	if (!is_number(parts[0])) 
	{
		shelldev_print_errors("Please specify index after which insertion should happen");
		return FALSE;
	}
	int base_insert_idx = std::stoi(parts[0]);

	std::cout << "Inserting at position: " << dye::light_green(std::stoi(parts[0]) + 1) << std::endl;
	std::cout << "Type '-' to quit editing" << std::endl;

	std::string input = shelldev_read();
	if (input == "-") {
		return TRUE;
	}

	asm_t temp;
	temp.instruction = input;
	assemblies->insert(assemblies->begin() + base_insert_idx, temp);

	base_insert_idx += 1;

	shelldev_run_shellcode(sh, assemblies);

	return TRUE;
}

static BOOL shelldev_xoring()
{
	if (xorNulls) {
		xorNulls = FALSE;
		std::cout << "Xoring is " << dye::red("disabled") << std::endl;
	}
	else {
		xorNulls = TRUE;
		std::cout << "Xoring is " << dye::green("enabled") << std::endl;
	}
	return TRUE;
}

static BOOL shelldev_command_stackframe(shell_t* sh, std::vector<asm_t>* assemblies)
{
#ifdef _M_X64
	std::string instructions = "push rbp;mov rbp, rsp";
#elif defined(_M_IX86)
	std::string instructions = "push ebp;mov ebp, esp";
#endif
	shelldev_run_shellcode(sh, instructions, assemblies);

	return TRUE;
}

//TODO: Finish counting up difference between pushes and pops and appending a proper number of pops
static BOOL shelldev_command_stackreset(shell_t* sh, std::vector<asm_t>* assemblies)
{
/*	int numpush = 0;
	int numpop = 0;
	std::srand(std::time(0)); 
#ifdef _M_X64
	int randpos = std::rand() % gregs_x64.size();  
	std::string randreg = gregs_x64[randpos];
#elif defined(_M_IX86)
	int randpos = std::rand() % gregs_x32.size();
	std::string randreg = gregs_x32[randpos];
#endif
	std::string popsled = "";
 	std::string pushsled = "";
  	popsled += std::format(";pop %s", randreg);
	pushsled += std::format(";push %s", randreg);
	for (asm_t assembly : *assemblies) {
		std::string first_mnemonic = split(assembly.instruction, " ")[0];
		if ( first_mnemonic == "push" ) {
			numpop += 1;
			popsled += std::format(";pop %s", randreg);
		}
  		if ( first_mnemonic == "pop" ) {
			numpush += 1;
			pushsled += std::format(";push %s", randreg);
		}
	}
 	if (numpush == numpop) {

  	} else  if( numpush > numpop ) {

   	} else {
    
	}
	shelldev_print_good("Resetting stack using %d POP instructions to register %s", numpop, randreg);
	shelldev_run_shellcode(sh, popsled, assemblies);
	*/
	
	return TRUE;
}

static BOOL shelldev_command_clearstackframe(shell_t* sh, std::vector<asm_t>* assemblies)
{
	std::srand(std::time(0));
	std::string instructions = "";
	int randbool = std::rand() % 2;
	if (randbool) {
#ifdef _M_X64
		instructions = "mov rsp, rbp; pop rbp";
#elif defined(_M_IX86)
		instructions = "mov esp, ebp; pop ebp";
#endif
	} else {
		instructions = "ret";
	}
	shelldev_run_shellcode(sh, instructions, assemblies);

	return TRUE;
}

static BOOL winrepl_command_help()
{
	std::cout << ".help\t\t\tShow this help screen." << std::endl;
	std::cout << ".registers\t\tShow more detailed register info" << std::endl;
	std::cout << ".list\t\t\tShow list of previously executed assembly instructions" << std::endl;
	std::cout << ".ins <line>\t\tInsert instructions after index" << std::endl;
	std::cout << ".edit <line>\t\tEdit specified line in list" << std::endl;
	std::cout << ".del <line>\t\tDelete specified line from list" << std::endl;
	std::cout << ".xor\t\t\tEnable or disable and show status of nullbyte xoring" << std::endl;
	std::cout << ".nsf\t\t\tEstablish new stackframe" << std::endl;
	std::cout << ".csf\t\t\tClear stackframe and load previous frame" << std::endl;
	std::cout << ".rsf\t\t\tFully reset stack by ensuring equivalent number of LIFO operations" << std::endl;
	std::cout << ".read <addr> <size>\tRead from a memory address" << std::endl;
	std::cout << ".swap <src> <dst>\tSwap source with destination lines" << std::endl;
	std::cout << ".write <addr> <hexdata>\tWrite to a memory address" << std::endl;
	std::cout << ".toshell <format>\tConvert list to selected shellcode format. Available formats: c, cs, raw, py" << std::endl;
	std::cout << ".inject <pid>\t\tTest shellcode by injecting it into the process. Works currently only on x86!" << std::endl;
	std::cout << ".alloc <size>\t\tAllocate a memory buffer" << std::endl;
	std::cout << ".loadlibrary <path>\tLoad a DLL into the process" << std::endl;
	std::cout << ".kernel32 <func>\tGet address of a kernel32 export" << std::endl;
	std::cout << ".shellcode <hexdata>\tExecute raw shellcode" << std::endl;
	std::cout << ".peb\t\t\tLoads PEB into accumulator" << std::endl;
	std::cout << ".fixip\t\t\tFix instruction pointer when 0xCC or 0xC3 is encountered" << std::endl;
	std::cout << ".reset\t\t\tStart a new environment" << std::endl;
	std::cout << ".abort\t\t\tInsert logic check and quit if AX != 0" << std::endl;
	std::cout << ".quit\t\t\tExit the program" << std::endl;

	return TRUE;
}


BOOL shelldev_run_command(shell_t* sh, std::string command, std::vector<asm_t>* assemblies)
{
	std::vector<std::string> parts = split(command, " ");
	std::string mainCmd = parts[0];
	parts.erase(parts.begin());

	if (mainCmd == ".registers")
		return shelldev_command_registers(sh, parts);
	else if (mainCmd == ".list")
		return shelldev_list(assemblies);
	else if (mainCmd == ".edit")
		return shelldev_edit(sh, assemblies, parts);
	else if (mainCmd == ".swap")
		return shelldev_swap(sh, assemblies, parts);
	else if (mainCmd == ".toshell")
		return shelldev_toshell(assemblies, parts);
	else if (mainCmd == ".inject")
		return shelldev_inject_shellcode(assemblies, parts[0]);
	else if (mainCmd == ".read")
		return shelldev_command_read(sh, parts);
	else if (mainCmd == ".nsf")
		return shelldev_command_stackframe(sh, assemblies);
	else if (mainCmd == ".csf")
		return shelldev_command_clearstackframe(sh, assemblies);
	else if (mainCmd == ".rsf")
		return shelldev_command_stackreset(sh, assemblies);
	else if (mainCmd == ".del")
		return shelldev_command_delete(sh, assemblies, parts);
	else if (mainCmd == ".ins")
		return shelldev_command_insert(sh, assemblies, parts);
	else if (mainCmd == ".abort")
		return shelldev_command_abort(sh, assemblies);
	else if (mainCmd == ".xor")
		return shelldev_xoring();
	else if (mainCmd == ".write")
		return shelldev_command_write(sh, parts);
	else if (mainCmd == ".fixip")
		return shelldev_command_fixip(sh);
	else if (mainCmd == ".alloc")
		return shelldev_command_allocate(sh, parts);
	else if (mainCmd == ".loadlibrary")
		return shelldev_command_loadlibrary(sh, parts);
	else if (mainCmd == ".kernel32")
		return shelldev_command_kernel32(sh, parts);
	else if (mainCmd == ".reset")
		return (shelldev_command_reset_assemblies(assemblies) && shelldev_command_reset(sh));
	else if (mainCmd == ".shellcode")
		return shelldev_command_shellcode(sh, parts);
	else if (mainCmd == ".peb")
		return shelldev_command_peb(sh, parts, assemblies);
	else if (mainCmd == ".quit" || mainCmd == ".exit")
		ExitProcess(0);
	else
	{
		if (mainCmd != ".help")
			shelldev_print_errors("Command not found!");
		return winrepl_command_help();
	}

	return TRUE;
}
