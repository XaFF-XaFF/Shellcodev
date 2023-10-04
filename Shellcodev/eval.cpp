#include "repl.h"

#undef min
#undef max
#include <asmjit/asmjit.h>
#include <asmtk/asmtk.h>
#include <regex>

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

	std::string key = "11111111";
	if(xorNulls == TRUE)
	{
		for (int i = 0; i < hex.size(); i++)
			if (hex[i].size() < 8)
				for (int j = 0; j < (8 - hex[i].size()); j++)
					hex[i].insert(0, "00");
	}

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


//static BOOL shelldev_assemble(const char* instruction, std::vector<unsigned char>& data, size_t address)
//{
//	using namespace asmjit;
//	using namespace asmtk;
//
//	const char* i1 = "mov eax, 0x760a9350";
//	const char* i2 = "xor edx, edx";
//	const char* i3 = "push edx";
//	const char* i4 = "call eax";
//
//	// Setup CodeInfo
//	JitRuntime jr;
//
//	// Setup CodeHolder
//	CodeHolder code;
//	Error err = code.init(jr.environment());
//	if (err != kErrorOk)
//	{
//		printf("ERROR: %s\n", DebugUtils::errorAsString(err));
//		return FALSE;
//	}
//
//	// Attach an assembler to the CodeHolder.
//	x86::Assembler a(&code);
//
//	Label loop = a.newLabel();
//	AsmParser p(&a);
//
//	a.bind(loop);
//	p.parse(i1);
//	p.parse(i2);
//	p.parse(i3);
//	p.parse(i3);
//	p.parse(i3);
//	p.parse(i3);
//	p.parse(i4);
//
//	a.jmp(loop);
//
//	code.detach(&a);
//
//	// Now you can print the code, which is stored in the first section (.text).
//	CodeBuffer& buffer = code.sectionById(0)->buffer();
//	for (size_t i = 0; i < buffer.size(); i++)
//		data.push_back(buffer.data()[i]);
//
//	return TRUE;
//}

BOOL shelldev_run_shellcode(shell_t* sh, std::vector<asm_t>* assemblies)
{
#ifdef _M_X64
	size_t addr = sh->curr.Rip;
#elif defined(_M_IX86)
	size_t addr = sh->curr.Eip;
#endif

	for (int i = 0; i < assemblies->capacity(); i++)
	{
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
		if (!shelldev_assemble(instruction.c_str(), temp, addr + temp.size()))
			return TRUE;

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

BOOL shelldev_eval(shell_t* sh, std::string command, std::vector<asm_t>* assemblies)
{
	try
	{
		if (command.at(0) == '.')
			return shelldev_run_command(sh, command, assemblies);

		return shelldev_run_shellcode(sh, command, assemblies);
	}
	catch (...)
	{
		shelldev_print_errors("An unhandled C++ exception occurred.");
	}

	return TRUE;
}