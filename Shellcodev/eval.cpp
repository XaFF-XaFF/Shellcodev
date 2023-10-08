#undef min
#undef max
#include <asmjit/asmjit.h>
#include <asmtk/asmtk.h>
#include <regex>
#include "repl.h"

static std::string get_register(std::string instruction)
{
	std::string reg;
	for (int i = 4; i < instruction.size(); i++)
		if (instruction[i] == ',')
			break;
		else reg += instruction[i];

	return reg;
}

static inline unsigned int value(char c)
{
	if (c >= '0' && c <= '9') { return c - '0'; }
	if (c >= 'a' && c <= 'f') { return c - 'a' + 10; }
	if (c >= 'A' && c <= 'F') { return c - 'A' + 10; }
	return -1;
}

std::string str_xor(std::string const& s1, std::string const& s2)
{
	static char const alphabet[] = "0123456789abcdef";

	std::string result;
	result.reserve(s1.length());

	for (std::size_t i = 0; i != s1.length(); ++i)
	{
		unsigned int v = value(s1[i]) ^ value(s2[i]);

		result.push_back(alphabet[v]);
	}

	return result;
}

std::vector<std::string> shelldev_parse_string(std::string reg, std::string value) // Currently only works on x86!
{
	std::string key = "11111111";

	std::vector<std::string> stringParts;
	for (size_t i = 0; i < value.size(); i += 4)
		stringParts.push_back(value.substr(i, 4));

	std::vector<std::string> hex;
	for (std::string part : stringParts)
	{
		std::stringstream ss;
		for (int i = part.size() - 1; i >= 0; i--)
			ss << std::hex << static_cast<int>(part[i]);

		hex.push_back(ss.str());
	}

	if(xorNulls == TRUE)
		for (int i = 0; i < hex.size(); i++)
			if (hex[i].size() < 8)
				for (int j = 0; j < (8 - hex[i].size()); j++)
					hex[i].insert(0, "00");

	std::vector<_str_parser_t> parsers;
	for (int i = 0; i < hex.size(); i++)
	{
		_str_parser_t parser;
		if (xorNulls == TRUE && hex[i].find("0") != std::string::npos)
		{
			parser.instruction = str_xor(hex[i], key);
			parser.xored = TRUE;
			parsers.push_back(parser);
		}
		else
		{
			parser.instruction = hex[i];
			parser.xored = FALSE;
			parsers.push_back(parser);
		}
	}
		
	std::vector<std::string> instructions;
	for (int i = parsers.size() - 1; i >= 0; i--)
	{
		if (parsers[i].xored)
		{
			instructions.push_back("mov " + reg + ", 0x" + parsers[i].instruction);
			instructions.push_back("xor " + reg + ", 0x" + key);
			instructions.push_back("push " + reg);
		}
		else
		{
			instructions.push_back("push 0x" + parsers[i].instruction);
		}
	}

#ifdef _M_X64
	instructions.push_back("mov " + reg + ", rsp");
#elif defined(_M_IX86)
	instructions.push_back("mov " + reg + ", esp");
#endif
	
	return instructions;
}

static void shelldev_fix_rip(shell_t* sh)
{
	// fix RIP because of \xcc
	CONTEXT ctx = { 0 };
	ctx.ContextFlags = CONTEXT_ALL;
	GetThreadContext(sh->procInfo.hThread, &ctx);

#ifdef _M_X64
	ctx.Rip = ctx.Rip - 1;
#elif defined(_M_IX86)
	ctx.Eip = ctx.Eip - 1;
#endif
	SetThreadContext(sh->procInfo.hThread, &ctx);
}

BOOL shelldev_write_shellcode(shell_t* sh, unsigned char* encode, size_t size)
{
	DWORD dwOldProtect = 0;
	SIZE_T nBytes;
	CONTEXT ctx = { 0 };

	shelldev_print_assembly(encode, size);

	ctx.ContextFlags = CONTEXT_ALL;
	if (!GetThreadContext(sh->procInfo.hThread, &ctx))
		return FALSE;


#ifdef _M_X64
	LPVOID addr = (LPVOID)ctx.Rip;
#elif defined(_M_IX86)
	LPVOID addr = (LPVOID)ctx.Eip;
#endif

	if (!VirtualProtectEx(sh->procInfo.hProcess, (LPVOID)addr, size + 1, PAGE_READWRITE, &dwOldProtect))
		return FALSE;

	if (!WriteProcessMemory(sh->procInfo.hProcess, (LPVOID)addr, (LPCVOID)encode, size, &nBytes))
		return FALSE;

	if (!WriteProcessMemory(sh->procInfo.hProcess, (LPVOID)((LPBYTE)addr + size), (LPCVOID)"\xcc", 1, &nBytes))
		return FALSE;

	if (!VirtualProtectEx(sh->procInfo.hProcess, (LPVOID)addr, size + 1, dwOldProtect, &dwOldProtect))
		return FALSE;

	FlushInstructionCache(sh->procInfo.hProcess, (LPCVOID)addr, size + 1);

	return TRUE;
}

void shelldev_debug_shellcode(shell_t* sh)
{
	BOOL go = TRUE;
	while (go)
	{
		ContinueDebugEvent(sh->procInfo.dwProcessId, sh->procInfo.dwThreadId, DBG_CONTINUE);

		DEBUG_EVENT dbg = { 0 };
		if (!WaitForDebugEvent(&dbg, INFINITE))
			break;

		if (dbg.dwThreadId != sh->procInfo.dwThreadId)
		{
			ContinueDebugEvent(dbg.dwProcessId, dbg.dwThreadId, DBG_CONTINUE);
			continue;
		}

		if (dbg.dwDebugEventCode == EXCEPTION_DEBUG_EVENT && dbg.dwThreadId == sh->procInfo.dwThreadId)
		{
			go = FALSE;

			switch (dbg.u.Exception.ExceptionRecord.ExceptionCode)
			{
			case EXCEPTION_ACCESS_VIOLATION:
				break;

			case EXCEPTION_PRIV_INSTRUCTION:
				break;

			case EXCEPTION_BREAKPOINT:
				break;
			default:
				break;
			}
		}

		if (dbg.dwDebugEventCode == LOAD_DLL_DEBUG_EVENT)
		{
			if (dbg.u.LoadDll.hFile)
				CloseHandle(dbg.u.LoadDll.hFile);
		}
	}

	shelldev_fix_rip(sh);

	CONTEXT ctx = { 0 };
	ctx.ContextFlags = CONTEXT_ALL;
	GetThreadContext(sh->procInfo.hThread, &ctx);

	memcpy(&sh->prev, &sh->curr, sizeof(CONTEXT));
	memcpy(&sh->curr, &ctx, sizeof(CONTEXT));
}

static BOOL shelldev_assemble(const char* instruction, std::vector<unsigned char>& data, size_t address)
{
	using namespace asmjit;
	using namespace asmtk;

	// Setup CodeInfo
	JitRuntime jr;

	// Setup CodeHolder
	CodeHolder code;
	Error err = code.init(jr.environment());
	if (err != kErrorOk)
	{
		printf("ERROR: %s\n", DebugUtils::errorAsString(err));
		return FALSE;
	}

	// Attach an assembler to the CodeHolder.
	x86::Assembler a(&code);

	// Create AsmParser that will emit to X86Assembler.
	AsmParser p(&a);

	// Parse some assembly.
	err = p.parse(instruction);

	// Error handling
	if (err != kErrorOk)
	{
		printf("ERROR: %s (instruction: \"%s\")\n", DebugUtils::errorAsString(err), instruction);
		return FALSE;
	}

	// If we are done, you must detach the Assembler from CodeHolder or sync
	// it, so its internal state and position is synced with CodeHolder.
	code.detach(&a);

	// Now you can print the code, which is stored in the first section (.text).
	CodeBuffer& buffer = code.sectionById(0)->buffer();
	for (size_t i = 0; i < buffer.size(); i++)
		data.push_back(buffer.data()[i]);

	return TRUE;
}

static BOOL shelldev_jump(asmjit::Label loop, asmjit::x86::Assembler* a, std::string instruction)
{
	// Jump instruction checker
	std::string jump;
	for (int i = 0; i < instruction.size(); i++)
		if (instruction[i] != ' ')
			jump += instruction[i];
		else break;

	if (jump == "jmp")
		a->jmp(loop);
	else if (jump == "je")
		a->je(loop);
	else if (jump == "jz")
		a->jz(loop);
	else if (jump == "jne")
		a->jne(loop);
	else if (jump == "jnz")
		a->jnz(loop);
	else if (jump == "jg")
		a->jg(loop);
	else if (jump == "jnle")
		a->jnle(loop);
	else if (jump == "jge")
		a->jge(loop);
	else if (jump == "jnl")
		a->jnl(loop);
	else if (jump == "jl")
		a->jl(loop);
	else if (jump == "jnge")
		a->jnge(loop);
	else if (jump == "jle")
		a->jle(loop);
	else if (jump == "jng")
		a->jng(loop);
	else if (jump == "ja")
		a->ja(loop);
	else if (jump == "jnbe")
		a->jnbe(loop);
	else if (jump == "jae")
		a->jae(loop);
	else if (jump == "jnb")
		a->jnb(loop);
	// Add more options
	else
		return FALSE;

	return TRUE;
}

// If jump instruction detected, reassemble everything
BOOL shelldev_assemble_loop(std::vector<asm_t>* assemblies, std::vector<unsigned char>& data, size_t address)
{
	using namespace asmjit;
	using namespace asmtk;

	struct Loop
	{
		std::string name;
		Label label;
	};

	// Setup CodeInfo
	JitRuntime jr;

	// Setup CodeHolder
	CodeHolder code;
	Error err = code.init(jr.environment());
	if (err != kErrorOk)
	{
		printf("ERROR: %s\n", DebugUtils::errorAsString(err));
		return FALSE;
	}

	// Attach an assembler to the CodeHolder.
	x86::Assembler a(&code);

	std::vector<Loop> loops;
	AsmParser p(&a);

	for (int i = 0; i < assemblies->size(); i++)
	{
		std::string instruction = assemblies->at(i).instruction;

		if (instruction[instruction.size() - 1] == ':')
		{
			Loop loop;
			loop.name = instruction.erase(instruction.size() - 1, 1); // Remove : from label
			loop.label = a.newLabel();

			a.bind(loop.label);
			loops.push_back(loop);
		}
		else if (instruction[0] == 'j')
		{
			std::string labelName;
			for (int i = instruction.size() - 1; i >= 0; i--)
			{
				if (instruction[i] != ' ')
					labelName += instruction[i];
				else break;
			}

			std::reverse(labelName.begin(), labelName.end());

			Label label; 
			for (int i = 0; i < loops.size(); i++)
				if (loops.at(i).name == labelName)
					label = loops.at(i).label;

			if (!shelldev_jump(label, &a, instruction))
				return FALSE;
		}
		else
		{
			err = p.parse(instruction.c_str());
		}
	}

	code.detach(&a);

	// Now you can print the code, which is stored in the first section (.text).
	CodeBuffer& buffer = code.sectionById(0)->buffer();
	for (size_t i = 0; i < buffer.size(); i++)
		data.push_back(buffer.data()[i]);

	return TRUE;
}

BOOL shelldev_run_shellcode(shell_t* sh, std::vector<asm_t>* assemblies)
{
#ifdef _M_X64
	size_t addr = sh->curr.Rip;
#elif defined(_M_IX86)
	size_t addr = sh->curr.Eip;
#endif

	for (int i = 0; i < assemblies->capacity(); i++)
	{
		if (assemblies->at(i).size == 0)
			i++;

		std::vector<unsigned char> data;
		if (!shelldev_assemble(assemblies->at(i).instruction.c_str(), data, addr + data.size()))
			return TRUE;

		assemblies->at(i).bytes = data;
		assemblies->at(i).size = sizeof(data);

		if (!shelldev_write_shellcode(sh, data.data(), data.size()))
			return FALSE;

		shelldev_debug_shellcode(sh);
	}

	shelldev_print_registers(sh);

	return TRUE;
}

BOOL shelldev_run_shellcode(shell_t* sh, std::string assembly, std::vector<asm_t>* assemblies)
{
	std::vector<std::string> instructions = split(assembly, ";");
	std::vector<unsigned char> data;

#ifdef _M_X64
	size_t addr = sh->curr.Rip;
#elif defined(_M_IX86)
	size_t addr = sh->curr.Eip;
#endif

	for (int i = 0; i < instructions.size(); i++)
	{
		std::vector<std::string> itms = split(instructions[i], "\"");
		for (std::vector<std::string>::iterator it = itms.begin() + 1; it != itms.end(); it += 2)
		{
			std::string reg = get_register(instructions[i]);
			std::vector<std::string> parse = shelldev_parse_string(reg, *it);

			instructions.insert(instructions.end(), parse.begin(), parse.end());
			instructions.erase(instructions.begin() + i);
		}
	}

	for (std::string& instruction : instructions)
	{
		std::vector<unsigned char> temp;

		if(instruction[instruction.size() - 1] != ':')
			if (!shelldev_assemble(instruction.c_str(), temp, addr + temp.size()))
				return FALSE;

		asm_t a;
		a.instruction = instruction;
		a.bytes = temp;
		a.size = sizeof(temp);

		assemblies->push_back(a);
		data.insert(data.end(), temp.begin(), temp.end());
	}

	if (!shelldev_write_shellcode(sh, data.data(), data.size()))
		return FALSE;

	shelldev_debug_shellcode(sh);

	shelldev_print_registers(sh);

	return TRUE;
}

BOOL shelldev_loop_eval(std::string jump, shell_t* sh, std::vector<asm_t>* assemblies)
{
#ifdef _M_X64
	size_t addr = sh->curr.Rip;
#elif defined(_M_IX86)
	size_t addr = sh->curr.Eip;
#endif

	std::vector<unsigned char> data;

	asm_t asmt;
	asmt.instruction = jump;

	assemblies->push_back(asmt);

	if (!shelldev_assemble_loop(assemblies, data, addr + data.size()))
		return FALSE;

	// assemblies->at(assemblies->size() - 1).bytes;

	if (!shelldev_write_shellcode(sh, data.data(), data.size()))
		return FALSE;

	shelldev_debug_shellcode(sh);

	shelldev_print_registers(sh);


	return TRUE;
}

BOOL shelldev_eval(shell_t* sh, std::string command, std::vector<asm_t>* assemblies)
{
	try
	{
		if (command.at(0) == '.')
			return shelldev_run_command(sh, command, assemblies);
		else if (command.at(0) == 'j')
			return shelldev_loop_eval(command, sh, assemblies);

		return shelldev_run_shellcode(sh, command, assemblies);
	}
	catch (...)
	{
		shelldev_print_errors("An unhandled C++ exception occurred.");
	}

	return TRUE;
}