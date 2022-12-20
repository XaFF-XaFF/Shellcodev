#include "repl.h"

static BOOL winrepl_create_debuggee(shell_t *sh)
{
	STARTUPINFO si = { 0 };
	TCHAR fileName[MAX_PATH] = { 0 };

	GetModuleFileName(NULL, fileName, MAX_PATH);

	si.dwFlags = STARTF_USESHOWWINDOW;
	si.wShowWindow = SW_HIDE;	// already 0
	si.cb = sizeof(si);

	if (!CreateProcess(
		fileName,
		NULL,
		NULL,
		NULL,
		FALSE,
		DEBUG_ONLY_THIS_PROCESS,
		NULL,
		NULL,
		&si,
		&sh->procInfo
	))
	{
		return FALSE;
	}

	// workaround for a bug on startup (Windows 8.1 x64), SetThreadContext would fail for some reason
	CloseHandle(sh->procInfo.hThread);
	if (!(sh->procInfo.hThread = OpenThread(
		THREAD_SET_CONTEXT | THREAD_GET_CONTEXT | THREAD_QUERY_INFORMATION,
		FALSE,
		sh->procInfo.dwThreadId
	)))
	{
		return FALSE;
	}

	// swallow initial debug events
	while (TRUE)
	{
		DEBUG_EVENT dbg = { 0 };
		if (!WaitForDebugEvent(&dbg, 1000))
			break;

		if (dbg.dwDebugEventCode == CREATE_PROCESS_DEBUG_EVENT)
			CloseHandle(dbg.u.CreateProcessInfo.hFile);

		if (dbg.dwDebugEventCode == LOAD_DLL_DEBUG_EVENT)
		{
			if (dbg.u.LoadDll.hFile)
				CloseHandle(dbg.u.LoadDll.hFile);
		}
	
		if (dbg.dwDebugEventCode == EXCEPTION_DEBUG_EVENT &&
			dbg.dwThreadId == sh->procInfo.dwThreadId)
			break;

		ContinueDebugEvent(dbg.dwProcessId, dbg.dwThreadId, DBG_CONTINUE);
	}

	return TRUE;
}

static BOOL winrepl_alloc_mem(shell_t *sh)
{
	if (sh->nMemSize == 0)
		sh->nMemSize = WINREPL_INIT_MEM_SIZE;

	sh->lpStartAddress = VirtualAllocEx(
		sh->procInfo.hProcess,
		NULL,
		sh->nMemSize,
		MEM_COMMIT,
		PAGE_EXECUTE_READ);

	return sh->lpStartAddress != NULL;
}

static BOOL winrepl_reset_context(shell_t *sh)
{
	CONTEXT ctx = { 0 };
	ctx.ContextFlags = CONTEXT_ALL;

	if (!GetThreadContext(sh->procInfo.hThread, &ctx))
		return FALSE;

#ifdef _M_X64
	ctx.Rip = (DWORD64)sh->lpStartAddress;

	ctx.Rax = 0;
	ctx.Rbx = 0;
	ctx.Rcx = 0;
	ctx.Rdx = 0;

	ctx.Rsi = 0;
	ctx.Rdi = 0;
	ctx.Rbp = 0;

	ctx.R8 = 0;
	ctx.R9 = 0;
	ctx.R10 = 0;
	ctx.R11 = 0;
	ctx.R12 = 0;
	ctx.R13 = 0;
	ctx.R14 = 0;
	ctx.R15 = 0;

	ctx.EFlags = 0;
#elif defined(_M_IX86)
	ctx.Eip = (DWORD)sh->lpStartAddress;

	ctx.Eax = 0;
	ctx.Ebx = 0;
	ctx.Ecx = 0;
	ctx.Edx = 0;
	
	ctx.Esi = 0;
	ctx.Edi = 0;
	ctx.Ebp = 0;

	ctx.EFlags = 0;
#elif defined(_M_ARM)
	// todo: ARM?
	return FALSE;
#else
	return FALSE;
#endif

	sh->prev = ctx;
	sh->curr = ctx;

	return SetThreadContext(sh->procInfo.hThread, &ctx);
}

BOOL shelldev_init(shell_t *sh)
{
	if (!winrepl_create_debuggee(sh))
		return FALSE;

	if (!winrepl_alloc_mem(sh))
		return FALSE;

	if (!winrepl_reset_context(sh))
		return FALSE;

	return TRUE;
}