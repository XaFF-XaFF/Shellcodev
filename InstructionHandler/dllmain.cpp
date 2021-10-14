// dllmain.cpp : Defines the entry point for the DLL application.
#define _CRT_SECURE_NO_WARNINGS
#include "ihandler.h"
#include <asmjit/asmjit.h>
#include <asmtk/asmtk.h>
#include <fstream>

using namespace asmjit;
using namespace asmtk;

typedef int (*Func)(void);

const char* AssembleInstructions(const char* instruction)
{
	std::vector<unsigned char> data;
	JitRuntime rt;
	CodeHolder code;

	code.init(rt.environment());

	x86::Assembler a(&code);
	AsmParser p(&a);

	Error err = p.parse(instruction);

	if (err != kErrorOk)
		return (const char*)"Error: %s", DebugUtils::errorAsString(err);

	if (code._relocations.size())
		return (const char*)"Error: unresolved relocations";

	code.detach(&a);

	CodeBuffer& buffer = code.sectionById(0)->buffer();
	for (size_t i = 0; i < buffer.size(); i++)
		data.push_back(buffer.data()[i]);

	unsigned char* result = new unsigned char[64];
	for (size_t i = 0; i < data.size(); ++i)
		sprintf((char*)&result[i * 2], "%02x", data[i]);

	return (const char*)result;
}