#include "ihandler.h"

BOOL set_debugee(contexts_t* ctx)
{
    STARTUPINFO si = { 0 };
    TCHAR fileName[MAX_PATH] = { 0 };

    GetModuleFileName(NULL, fileName, MAX_PATH);

    si.dwFlags = STARTF_USESHOWWINDOW;
    si.wShowWindow = SW_HIDE;
    si.cb = sizeof(si);

    BOOL crtp = CreateProcess(fileName, NULL, NULL, NULL, FALSE, DEBUG_ONLY_THIS_PROCESS, NULL, NULL, &si, &ctx->pi);
    if (!crtp)
        return FALSE;

    CloseHandle(ctx->pi.hThread);
    if (!(ctx->pi.hThread = OpenThread(THREAD_SET_CONTEXT | THREAD_GET_CONTEXT | THREAD_QUERY_INFORMATION, FALSE, ctx->pi.dwThreadId)))
        return FALSE;

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

        if (dbg.dwDebugEventCode == EXCEPTION_DEBUG_EVENT && dbg.dwThreadId == ctx->pi.dwThreadId)
            break;

        ContinueDebugEvent(dbg.dwProcessId, dbg.dwThreadId, DBG_CONTINUE);
    }

    return TRUE;
}

BOOL contexts_allocmem(contexts_t* ctx)
{
    if (ctx->memSize == 0)
        ctx->memSize = MEM_SIZE;

    ctx->lpStartAddress = VirtualAllocEx(
        ctx->pi.hProcess,
        NULL,
        ctx->memSize,
        MEM_COMMIT,
        PAGE_EXECUTE_READ);

    return ctx->lpStartAddress != NULL;
}

BOOL contexts(contexts_t* ctx)
{
    CONTEXT context = { 0 };
    context.ContextFlags = CONTEXT_ALL;

    if (!GetThreadContext(ctx->pi.hThread, &context))
        return FALSE;

    context.Eip = (DWORD)ctx->lpStartAddress;

    context.Eax = 0;
    context.Ebx = 0;
    context.Ecx = 0;
    context.Edx = 0;

    context.Esi = 0;
    context.Edi = 0;
    context.Ebp = 0;

    context.EFlags = 0;

    ctx->prev = context;
    ctx->curr = context;

    return SetThreadContext(ctx->pi.hThread, &context);
}

VOID InitRegisters(contexts_t* ctx,registers_t* registers)
{
    registers->eax = ctx->curr.Eax;
    registers->ebx = ctx->curr.Ebx;
    registers->ecx = ctx->curr.Ecx;
    registers->edx = ctx->curr.Edx;

    registers->esi = ctx->curr.Esi;
    registers->edi = ctx->curr.Edi;

    registers->esp = ctx->curr.Esp;
    registers->eip = ctx->curr.Eip;
    registers->ebp = ctx->curr.Ebp;
}