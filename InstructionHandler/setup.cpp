#include "ihandler.h"

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