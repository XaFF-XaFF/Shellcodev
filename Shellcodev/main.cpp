#include "repl.h"

static shell_t sh = { 0 };

BOOL CALLBACK winrepl_exit(DWORD dwCtrlCode)
{
	DebugActiveProcessStop(sh.procInfo.dwProcessId);
	ExitProcess(0);
}

int main(int argc, char *argv[])
{
	SetConsoleCtrlHandler(winrepl_exit, TRUE);

	std::cout << "Shellcodev v2.2 by XaFF based on WinREPL\n";
	std::cout << "Input assembly instructions, or \".help\" for a list of commands.\n" << std::endl;

	while (TRUE)
	{	
		if (!shelldev_loop(&sh))
			break;
	}

	return 0;
}