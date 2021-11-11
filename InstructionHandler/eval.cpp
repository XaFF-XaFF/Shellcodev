#include "ihandler.h"

BOOL Runner(contexts_t* ctx)
{
	if (!set_debugee(ctx))
		return FALSE;

	if (!contexts_allocmem(ctx))
		return FALSE;

	if (!contexts(ctx))
		return FALSE;

	return TRUE;
}

void debug_proc(contexts_t* ctx)
{
	BOOL go = TRUE;
	while (go)
	{
		ContinueDebugEvent(ctx->pi.dwProcessId, ctx->pi.dwThreadId, DBG_CONTINUE);

		DEBUG_EVENT dbg = { 0 };
		if (!WaitForDebugEvent(&dbg, INFINITE))
			break;

		if (dbg.dwThreadId != ctx->pi.dwThreadId)
		{
			ContinueDebugEvent(dbg.dwProcessId, dbg.dwThreadId, DBG_CONTINUE);
			continue;
		}

		if (dbg.dwDebugEventCode == EXCEPTION_DEBUG_EVENT && dbg.dwThreadId == ctx->pi.dwThreadId)
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

	CONTEXT context = { 0 };
	context.ContextFlags = CONTEXT_ALL;
	GetThreadContext(ctx->pi.hThread, &context);

	memcpy(&ctx->prev, &ctx->curr, sizeof(CONTEXT));
	memcpy(&ctx->curr, &context, sizeof(CONTEXT));
}

BOOL SetProcess(contexts_t* ctx, unsigned char* encode, size_t size)
{
	DWORD dwOldProtect = 0;
	SIZE_T nBytes;
	CONTEXT context = { 0 };

	//print asm

	context.ContextFlags = CONTEXT_ALL;
	if (!GetThreadContext(ctx->pi.hThread, &context))
		return FALSE;

	LPVOID addr = (LPVOID)context.Eip;

	if (!VirtualProtectEx(ctx->pi.hProcess, (LPVOID)addr, size + 1, PAGE_READWRITE, &dwOldProtect))
		return FALSE;

	if (!WriteProcessMemory(ctx->pi.hProcess, (LPVOID)addr, (LPCVOID)encode, size, &nBytes))
		return FALSE;

	if (!WriteProcessMemory(ctx->pi.hProcess, (LPVOID)((LPBYTE)addr + size), (LPCVOID)"\xcc", 1, &nBytes))
		return FALSE;

	if (!VirtualProtectEx(ctx->pi.hProcess, (LPVOID)addr, size + 1, dwOldProtect, &dwOldProtect))
		return FALSE;

	FlushInstructionCache(ctx->pi.hProcess, (LPCVOID)addr, size + 1);

	return TRUE;
}

BOOL Assemble(const char* instruction, std::vector<unsigned char>& data, size_t address)
{
	using namespace asmjit;
	using namespace asmtk;

	JitRuntime rt;
	CodeHolder code;

	code.init(rt.environment(), address);

	x86::Assembler a(&code);
	AsmParser p(&a);

	Error error = p.parse(instruction);

	if (error != kErrorOk)
		return FALSE;

	if (code._relocations.size())
		return FALSE;

	CodeBuffer& buffer = code.sectionById(0)->buffer();
	for (size_t i = 0; i < buffer.size(); i++)
		data.push_back(buffer.data()[i]);

	return TRUE;
}

BOOL Instructions(contexts_t* ctx, const char* instruction)
{
	std::vector<unsigned char> data;
	size_t addr = ctx->curr.Eip;

	if (!Assemble(instruction, data, addr + data.size()))
		return FALSE;

	if (!SetProcess(ctx, data.data(), data.size()))
		return FALSE;

	debug_proc(ctx);
}