#pragma once

#include <stdio.h>
#include <iostream>
#include <vector>

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
