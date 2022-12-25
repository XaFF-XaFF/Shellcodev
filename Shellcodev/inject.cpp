#include "repl.h"

static std::vector<unsigned char> get_shellcode(std::vector<asm_t>* assemblies)
{
	std::vector<unsigned char> bytes;

	for (asm_t assembly : *assemblies)
		bytes.insert(bytes.end(), assembly.bytes.begin(), assembly.bytes.end());
	
	return bytes;
}

BOOL shelldev_inject_shellcode(std::vector<asm_t>* assemblies, std::string pid)
{
	DWORD PID = std::stoi(pid);
	shelldev_print_good("Injecting shellcode into %d", PID);

	std::vector<unsigned char> bytes = get_shellcode(assemblies);

	return TRUE;
}