#include <Windows.h>

#include <string>
#include <vector>
#include "repl.h"

static std::vector<asm_t> assemblies;

BOOL shelldev_loop(shell_t *sh)
{
	if (!shelldev_init(sh))
		return FALSE;

	shelldev_print_pids(sh);
	shelldev_print_registers(sh);

	while (TRUE)
	{
		std::string command = shelldev_read();

		if (command.size() == 0)
			continue;

		if (!shelldev_eval(sh, command, &assemblies))
		{
			shelldev_print_errors("An unrecoverable error occurred, resetting environment!");
			break;
		}
	}

	return TRUE;
}