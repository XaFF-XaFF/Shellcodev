#pragma once

#include <iostream>

#include <Windows.h>

#include "str.h"

#define WINREPL_INIT_MEM_SIZE 0x10000

typedef struct _shell_context_t {
	PROCESS_INFORMATION procInfo;
	LPVOID lpStartAddress;
	SIZE_T nMemSize;
	CONTEXT prev;
	CONTEXT curr;
} shell_t;

typedef struct _asm_context_t {
	std::string instruction;
	std::vector<unsigned char> bytes;
	int size;
} asm_t;

typedef struct _parser_context_t {
	std::string instruction;
	BOOL xored;
} _str_parser_t;

BOOL shelldev_init(shell_t* sh);
BOOL shelldev_loop(shell_t* sh);

std::string shelldev_read();

BOOL shelldev_eval(shell_t* sh, std::string command, std::vector<asm_t>* assemblies);
BOOL shelldev_write_shellcode(shell_t* sh, unsigned char* encode, size_t size);
void shelldev_debug_shellcode(shell_t* sh);

std::vector<std::string> shelldev_parse_string(std::string value);
BOOL shelldev_run_shellcode(shell_t* sh, std::vector<asm_t>* assemblies);
BOOL shelldev_run_command(shell_t* sh, std::string command, std::vector<asm_t>* assemblies);

void shelldev_print_pids(shell_t* sh);
void shelldev_print_registers(shell_t* sh);
void shelldev_print_registers_all(shell_t* sh);
void shelldev_print_assembly(unsigned char* encode, size_t size);
void shelldev_print_bytes(unsigned char* addr, int len, unsigned long long start_addr = 0);
void shelldev_print_good(const char* format, ...);
void shelldev_print_errors(const char* format, ...);

BOOL shelldev_inject_shellcode(std::vector<asm_t>* assemblies, std::string pid);