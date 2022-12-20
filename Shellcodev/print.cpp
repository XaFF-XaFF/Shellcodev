#include "repl.h"
#include <stdio.h>

static inline BOOL check_bit(DWORD var, char pos)
{
	return !!((var) & (1 << (pos)));
}

void shelldev_print_assembly(unsigned char *encode, size_t size)
{
	printf("assembled (%zu bytes): ", size);
	
	for (size_t i = 0; i < size; ++i)
		printf("%02x ", encode[i]);

	printf("\n");
}

static void winrepl_reset_console_color()
{
	static HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(hConsole, FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE);
}

static void winrepl_print_console_color(WORD attributes, const char *format, ...)
{
	static HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

	if (attributes != 0)
		SetConsoleTextAttribute(hConsole, attributes);
	
	va_list argptr;
	va_start(argptr, format);
	vfprintf(stderr, format, argptr);
	va_end(argptr);

	winrepl_reset_console_color();
}


static void winrepl_print_register_32(const char *reg, DWORD64 value, DWORD64 prev)
{
	winrepl_print_console_color(FOREGROUND_GREEN | FOREGROUND_INTENSITY, "%s: ", reg);

	WORD color = (prev == value) ? 0 : FOREGROUND_RED | FOREGROUND_INTENSITY;
	winrepl_print_console_color(color, "%08llx ", value);
}

static void winrepl_print_register_64(const char *reg, DWORD64 value, DWORD64 prev)
{
	winrepl_print_console_color(FOREGROUND_GREEN | FOREGROUND_INTENSITY, "%s: ", reg);

	WORD color = (prev == value) ? 0 : FOREGROUND_RED | FOREGROUND_INTENSITY;
	winrepl_print_console_color(color, "%016llx ", value);
}


static void winrepl_print_register_flag(const char *flag, BOOL value, BOOL prev)
{
	winrepl_print_console_color(FOREGROUND_BLUE | FOREGROUND_GREEN, "%s: ", flag);

	WORD color = (prev == value) ? 0 : FOREGROUND_RED | FOREGROUND_INTENSITY;
	winrepl_print_console_color(color, "%d  ", value);
}

#ifdef _M_X64
static void winrepl_print_register_xmm(const char *reg, M128A value, M128A prev)
{

	winrepl_print_console_color(FOREGROUND_GREEN | FOREGROUND_INTENSITY, "%s: ", reg);

	printf("{ ");
	WORD color = (prev.High == value.High) ? 0 : FOREGROUND_RED | FOREGROUND_INTENSITY;
	winrepl_print_console_color(color, "%10.10e", value.High);

	printf(", ");

	color = (prev.Low == value.Low) ? 0 : FOREGROUND_RED | FOREGROUND_INTENSITY;
	winrepl_print_console_color(color, "%10.10e", value.Low);


	printf(" }\t");

	color = (prev.High == value.High) ? 0 : FOREGROUND_RED | FOREGROUND_INTENSITY;
	winrepl_print_console_color(color, "%016llx", value.High);

	color = (prev.Low == value.Low) ? 0 : FOREGROUND_RED | FOREGROUND_INTENSITY;
	winrepl_print_console_color(color, "%016llx", value.Low);

	printf("\n");

}
#elif defined(_M_IX86)
// ??????????????
static void winrepl_print_register_xmm(const char *reg, int a, int b)
{}
#else
// ?!!!!!!!!!
static void winrepl_print_register_xmm(const char *reg, int a, int b)
{}
#endif


void shelldev_print_errors(const char *format, ...)
{
	static HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

	winrepl_print_console_color(FOREGROUND_RED | FOREGROUND_INTENSITY, "%s", "[-] ");

	va_list argptr;
	va_start(argptr, format);
	vfprintf(stderr, format, argptr);
	va_end(argptr);

	DWORD dwErr = GetLastError();
	if (dwErr != 0)
		printf(" (errno: %d)", dwErr);

	printf("\n");
}

void shelldev_print_bytes(unsigned char *addr, int len, unsigned long long start_addr)
{
	int i;
	unsigned char buff[17];
	unsigned char *pc = (unsigned char*)addr;

	for (i = 0; i < len; i++)
	{
		if ((i % 16) == 0)
		{
			if (i != 0)
				printf("  %s\n", buff);

			printf("  %04llx ", start_addr + i);
		}

		printf(" %02x", pc[i]);

		if ((pc[i] < 0x20) || (pc[i] > 0x7e))
			buff[i % 16] = '.';
		else
			buff[i % 16] = pc[i];
		buff[(i % 16) + 1] = '\0';
	}

	while ((i % 16) != 0)
	{
		printf("   ");
		++i;
	}

	printf("  %s\n", buff);
}

void shelldev_print_good(const char *format, ...)
{
	winrepl_print_console_color(FOREGROUND_GREEN | FOREGROUND_INTENSITY, "%s", "[+] ");
	va_list argptr;
	va_start(argptr, format);
	vfprintf(stderr, format, argptr);
	va_end(argptr);
	printf("\n");
}

void shelldev_print_registers(shell_t *sh)
{
	CONTEXT ctx = { 0 };
	ctx.ContextFlags = CONTEXT_ALL;

	GetThreadContext(sh->procInfo.hThread, &ctx);

#ifdef _M_X64
	winrepl_print_register_64("rax", ctx.Rax, sh->prev.Rax);
	winrepl_print_register_64("rbx", ctx.Rbx, sh->prev.Rbx);
	winrepl_print_register_64("rcx", ctx.Rcx, sh->prev.Rcx);
	winrepl_print_register_64("rdx", ctx.Rdx, sh->prev.Rdx);
	printf("\n");

	winrepl_print_register_64("r8 ", ctx.R8, sh->prev.R8);
	winrepl_print_register_64("r9 ", ctx.R9, sh->prev.R9);
	winrepl_print_register_64("r10", ctx.R10, sh->prev.R10);
	winrepl_print_register_64("r11", ctx.R11, sh->prev.R11);
	printf("\n");

	winrepl_print_register_64("r12", ctx.R12, sh->prev.R12);
	winrepl_print_register_64("r13", ctx.R13, sh->prev.R13);
	winrepl_print_register_64("r14", ctx.R14, sh->prev.R14);
	winrepl_print_register_64("r15", ctx.R15, sh->prev.R15);
	printf("\n");

	
	winrepl_print_register_64("rsi", ctx.Rsi, sh->prev.Rsi);
	winrepl_print_register_64("rdi", ctx.Rdi, sh->prev.Rdi);
	printf("\n");

	winrepl_print_register_64("rip", ctx.Rip, sh->prev.Rip);
	winrepl_print_register_64("rsp", ctx.Rsp, sh->prev.Rsp);
	winrepl_print_register_64("rbp", ctx.Rbp, sh->prev.Rbp);
	printf("\n");
#elif defined(_M_IX86)
	winrepl_print_register_32("eax", ctx.Eax, sh->prev.Eax);
	winrepl_print_register_32("ebx", ctx.Ebx, sh->prev.Ebx);
	winrepl_print_register_32("ecx", ctx.Ecx, sh->prev.Ecx);
	winrepl_print_register_32("edx", ctx.Edx, sh->prev.Edx);
	printf("\n");

	winrepl_print_register_32("esi", ctx.Esi, sh->prev.Esi);
	winrepl_print_register_32("edi", ctx.Edi, sh->prev.Edi);
	printf("\n");

	winrepl_print_register_32("eip", ctx.Eip, sh->prev.Eip);
	winrepl_print_register_32("esp", ctx.Esp, sh->prev.Esp);
	winrepl_print_register_32("ebp", ctx.Ebp, sh->prev.Ebp);
	printf("\n");
#endif

#if defined(_M_X64) || defined(_M_IX86)
	printf("flags: %08x  ", ctx.EFlags);

	winrepl_print_register_flag("CF", check_bit(ctx.EFlags, 0), check_bit(sh->prev.EFlags, 0));
	winrepl_print_register_flag("PF", check_bit(ctx.EFlags, 2), check_bit(sh->prev.EFlags, 2));
	winrepl_print_register_flag("AF", check_bit(ctx.EFlags, 3), check_bit(sh->prev.EFlags, 3));
	winrepl_print_register_flag("ZF", check_bit(ctx.EFlags, 6), check_bit(sh->prev.EFlags, 6));
	winrepl_print_register_flag("SF", check_bit(ctx.EFlags, 7), check_bit(sh->prev.EFlags, 7));
	winrepl_print_register_flag("DF", check_bit(ctx.EFlags, 10), check_bit(sh->prev.EFlags, 10));
	winrepl_print_register_flag("OF", check_bit(ctx.EFlags, 11), check_bit(sh->prev.EFlags, 11));

	/*

	printf("cf: %d, ", check_bit(ctx.EFlags, 0));
	printf("pf: %d, ", check_bit(ctx.EFlags, 2));
	printf("af: %d, ", check_bit(ctx.EFlags, 4));
	printf("zf: %d, ", check_bit(ctx.EFlags, 6));
	printf("sf: %d, ", check_bit(ctx.EFlags, 7));
	printf("df: %d, ", check_bit(ctx.EFlags, 10));
	printf("of: %d]", check_bit(ctx.EFlags, 11));
	*/
	printf("\n");
#endif
}

void shelldev_print_registers_all(shell_t *sh)
{
	CONTEXT ctx = { 0 };
	ctx.ContextFlags = CONTEXT_ALL;

	GetThreadContext(sh->procInfo.hThread, &ctx);

#ifdef _M_X64
	winrepl_print_register_xmm("xmm0 ", ctx.Xmm0, sh->prev.Xmm0);
	winrepl_print_register_xmm("xmm1 ", ctx.Xmm1, sh->prev.Xmm1);
	winrepl_print_register_xmm("xmm2 ", ctx.Xmm2, sh->prev.Xmm2);
	winrepl_print_register_xmm("xmm3 ", ctx.Xmm3, sh->prev.Xmm3);
	winrepl_print_register_xmm("xmm4 ", ctx.Xmm4, sh->prev.Xmm4);
	winrepl_print_register_xmm("xmm5 ", ctx.Xmm5, sh->prev.Xmm5);
	winrepl_print_register_xmm("xmm6 ", ctx.Xmm6, sh->prev.Xmm6);
	winrepl_print_register_xmm("xmm7 ", ctx.Xmm7, sh->prev.Xmm7);
	winrepl_print_register_xmm("xmm8 ", ctx.Xmm8, sh->prev.Xmm8);
	winrepl_print_register_xmm("xmm9 ", ctx.Xmm9, sh->prev.Xmm9);
	winrepl_print_register_xmm("xmm10", ctx.Xmm10, sh->prev.Xmm10);
	winrepl_print_register_xmm("xmm11", ctx.Xmm11, sh->prev.Xmm11);
	winrepl_print_register_xmm("xmm12", ctx.Xmm12, sh->prev.Xmm12);
	winrepl_print_register_xmm("xmm13", ctx.Xmm13, sh->prev.Xmm13);
	winrepl_print_register_xmm("xmm14", ctx.Xmm14, sh->prev.Xmm14);
	winrepl_print_register_xmm("xmm15", ctx.Xmm15, sh->prev.Xmm15);
#endif

	shelldev_print_registers(sh);
}

void shelldev_print_pids(shell_t *sh)
{
	DWORD dwPPID = GetCurrentProcessId();
	DWORD dwPTID = GetCurrentThreadId();
	DWORD dwCPID = sh->procInfo.dwProcessId;
	DWORD dwCTID = sh->procInfo.dwThreadId;
	printf("PPID: %d\tPTID: %d\tCPID: %d\tCTID: %d\n", dwPPID, dwPTID, dwCPID, dwCTID);
}