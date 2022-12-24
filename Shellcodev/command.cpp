#include "repl.h"
#include "color.hpp"

void shelldev_print_assembly(unsigned char* encode, size_t size)
{
	printf("assembled (%zu bytes): ", size);

	for (size_t i = 0; i < size; ++i)
		if (encode[i] == 0x0)
			std::cout << std::hex << dye::light_red("0x") << dye::light_red(static_cast<int>(encode[i])) << " ";
		else 
			std::cout << std::hex << "0x" << static_cast<int>(encode[i]) << " ";

	printf("\n");
}

static BOOL shelldev_command_kernel32(shell_t* sh, std::vector<std::string> parts)
{
	do
	{
		if (parts.size() != 1)
		{
			shelldev_print_errors("Usage: .kernel32 func");
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

static BOOL shelldev_command_peb(shell_t* sh, std::vector<std::string> parts)
{
#ifdef _M_X64
	// xor eax, eax
	// mov rax, gs:[eax+0x60]
	unsigned char bytes[] = { 0x31, 0xc0, 0x65, 0x48, 0x8b, 0x40, 0x60 };
#elif defined(_M_IX86)
	// xor eax, eax
	// mov eax, fs:[eax+0x30]
	unsigned char bytes[] = { 0x31, 0xC0, 0x64, 0x8B, 0x40, 0x30 };
#endif
	if (!shelldev_write_shellcode(sh, bytes, sizeof(bytes)))
	{
		shelldev_print_errors("Unable to allocate shellcode!");
		return TRUE;
	}

	shelldev_debug_shellcode(sh);
	shelldev_print_registers(sh);

	return TRUE;
}

static BOOL shelldev_command_allocate(shell_t* sh, std::vector<std::string> parts)
{
	do
	{
		if (parts.size() != 1)
		{
			shelldev_print_errors("Usage: .allocate size");
			break;
		}

		size_t size = atol(parts[0].c_str());

		if (size == 0)
		{
			shelldev_print_errors("Usage: .allocate size");
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

static BOOL shelldev_command_reset(shell_t* sh, std::vector<asm_t>* assemblies)
{
	shelldev_print_good("Resetting the environment.");
	TerminateProcess(sh->procInfo.hProcess, 0);
	DebugActiveProcessStop(sh->procInfo.dwProcessId);
	assemblies->clear();
	return FALSE;
}

static BOOL shelldev_list(std::vector<asm_t>* assemblies)
{
	int count = 0;
	for (asm_t assembly : *assemblies)
	{
		std::cout << dye::light_green(count) << ".\t";
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

	shelldev_run_shellcode(sh, assemblies);

	return TRUE;
}

static BOOL shelldev_toshell(std::vector<asm_t>* assemblies, std::vector<std::string> parts)
{
	if (parts[0] == "c" || parts[0] == "C")
	{
		std::cout << "unsigned char shellcode[] = {" << std::endl;
		for (asm_t assembly : *assemblies)
		{
			for (unsigned char byte : assembly.bytes)
				printf("0x%x, ", byte);
		}
		std::cout << "};" << std::endl;
	}

	return TRUE;
}

static BOOL shelldev_command_delete(shell_t* sh, std::vector<asm_t>* assemblies, std::vector<std::string> parts) 
{
	assemblies->erase(assemblies->begin() + std::stoi(parts[0]));

	shelldev_run_shellcode(sh, assemblies);

	return TRUE;
}


static BOOL winrepl_command_help()
{
	std::cout << ".help\t\t\tShow this help screen." << std::endl;
	std::cout << ".registers\t\tShow more detailed register info." << std::endl;
	std::cout << ".list\t\t\tShow list of previously executed assembly instructions." << std::endl;
	std::cout << ".edit line\t\tEdit specified line in list." << std::endl;
	std::cout << ".del line\t\tDelete specified line from list." << std::endl;
	std::cout << ".read addr size\t\tRead from a memory address." << std::endl;
	std::cout << ".write addr hexdata\tWrite to a memory address." << std::endl;
	std::cout << ".toshell format\t\tConvert list to selected shellcode format. Available formats: c" << std::endl;
	std::cout << ".allocate size\t\tAllocate a memory buffer." << std::endl;
	std::cout << ".loadlibrary path\tLoad a DLL into the process." << std::endl;
	std::cout << ".kernel32 func\t\tGet address of a kernel32 export." << std::endl;
	std::cout << ".shellcode hexdata\tExecute raw shellcode." << std::endl;
	std::cout << ".peb\t\t\tLoads PEB into accumulator." << std::endl;
	std::cout << ".reset\t\t\tStart a new environment." << std::endl;
	std::cout << ".quit\t\t\tExit the program." << std::endl;

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
	else if (mainCmd == ".toshell")
		return shelldev_toshell(assemblies, parts);
	else if (mainCmd == ".read")
		return shelldev_command_read(sh, parts);
	else if (mainCmd == ".del")
		return shelldev_command_delete(sh, assemblies, parts);
	else if (mainCmd == ".write")
		return shelldev_command_write(sh, parts);
	else if (mainCmd == ".allocate")
		return shelldev_command_allocate(sh, parts);
	else if (mainCmd == ".loadlibrary")
		return shelldev_command_loadlibrary(sh, parts);
	else if (mainCmd == ".kernel32")
		return shelldev_command_kernel32(sh, parts);
	else if (mainCmd == ".reset")
		return shelldev_command_reset(sh, assemblies);
	else if (mainCmd == ".shellcode")
		return shelldev_command_shellcode(sh, parts);
	else if (mainCmd == ".peb")
		return shelldev_command_peb(sh, parts);
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