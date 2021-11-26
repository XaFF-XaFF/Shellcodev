#define _CRT_SECURE_NO_WARNINGS
#include "ihandler.h"

using namespace asmjit;
using namespace asmtk;

typedef int (*Func)(void);
static contexts_t ctx = { 0 };
static registers_t reg = { 0 };

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

// CODE USED FROM WinREPL
// https://github.com/zerosum0x0/WinREPL
registers_t* GetRegisters(const char* instruction, PROCESS_INFORMATION* pi)
{
	ctx.pi.hProcess = pi->hProcess;
	ctx.pi.hThread = pi->hThread;
	ctx.pi.dwProcessId = pi->dwProcessId;
	ctx.pi.dwThreadId = pi->dwThreadId;

	if (!Runner(&ctx))
		return NULL;

	if (instruction == NULL)
	{
		InitRegisters(&ctx, &reg);
		return &reg;
	}

	Instructions(&ctx, instruction);
	InitRegisters(&ctx, &reg);

	return &reg;
}