#pragma once

#include <Windows.h>
#include <winnt.h>
#include <stdio.h>
#include <iostream>
#include <vector>

#define _AMD64_ 1

#ifdef EBEXPORT
#define EBDECL __declspec(dllexport)
#else
#define EBDECL __declspec(dllimport)
#endif

#if defined (__cplusplus)
extern "C"
{
#endif

	__declspec(dllexport) const char* AssembleInstructions(const char* instruction);

#if defined (__cplusplus)
}
#endif
