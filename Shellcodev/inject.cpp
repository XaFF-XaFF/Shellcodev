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

	HANDLE processHandle;
	HANDLE remoteThread;
	PVOID remoteBuffer;

	processHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, PID);
	remoteBuffer = VirtualAllocEx(processHandle, NULL, bytes.size(), (MEM_RESERVE | MEM_COMMIT), PAGE_EXECUTE_READWRITE);
	WriteProcessMemory(processHandle, remoteBuffer, bytes.data(), bytes.size(), NULL);
	remoteThread = CreateRemoteThread(processHandle, NULL, 0, (LPTHREAD_START_ROUTINE)remoteBuffer, NULL, 0, NULL);
	CloseHandle(processHandle);

	return TRUE;
}