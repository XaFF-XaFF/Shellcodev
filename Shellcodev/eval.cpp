#include "repl.h"

#undef min
#undef max
#include <asmjit/asmjit.h>
#include <asmtk/asmtk.h>

static void winrepl_fix_rip(shell_t* sh)
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

	winrepl_fix_rip(sh);

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


static BOOL shelldev_run_shellcode(shell_t* sh, std::string assembly, std::vector<asm_t>* assemblies)
{
	std::vector<std::string> instructions = split(assembly, ";");
	std::vector<unsigned char> data;

#ifdef _M_X64
	size_t addr = sh->curr.Rip;
#elif defined(_M_IX86)
	size_t addr = sh->curr.Eip;
#endif

	for (std::string& instruction : instructions)
	{
		if (!shelldev_assemble(instruction.c_str(), data, addr + data.size()))
			return TRUE;

		asm_t a;
		a.instruction = instruction;
		a.bytes = data;
		a.size = sizeof(data);

		assemblies->push_back(a);
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