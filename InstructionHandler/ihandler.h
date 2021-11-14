#pragma once

#include <Windows.h>
#include <winnt.h>
#include <asmjit/asmjit.h>
#include <asmtk/asmtk.h>
#include <stdio.h>
#include <iostream>
#include <vector>

#define _AMD64_ 1
#define MEM_SIZE 0x10000;

typedef struct _contexts_t
{
	PROCESS_INFORMATION pi;
	LPVOID lpStartAddress;
	SIZE_T memSize;
	CONTEXT prev;
	CONTEXT curr;
} contexts_t;

typedef struct _registers_t
{
	int eax;
	int ebx;
	int ecx;
	int edx;
	int esi;
	int edi;
	int eip;
	int esp;
	int ebp;
} registers_t;

#if defined (__cplusplus)
extern "C"
{
#endif
	__declspec(dllexport) const char* AssembleInstructions(const char* instruction);
	__declspec(dllexport) registers_t* GetRegisters(const char* instruction, PROCESS_INFORMATION* pi);
#if defined (__cplusplus)
}
#endif

BOOL contexts_allocmem(contexts_t* ctx);
BOOL contexts(contexts_t* ctx);
BOOL Runner(contexts_t* ctx);
BOOL Instructions(contexts_t* ctx, const char* instruction);

VOID InitRegisters(contexts_t* ctx, registers_t* registers);