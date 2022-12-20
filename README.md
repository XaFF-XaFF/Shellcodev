# Shellcodev
Shellcodev is a tool designed to help and automate the process of shellcode creation. 

[![Test](https://img.shields.io/badge/Tested-x86-brightgreen?style=flat-square)]() [![Test](https://img.shields.io/badge/Not%20tested-x64-red?style=flat-square)]() [![Release](https://img.shields.io/badge/Release-v2.0.0-blue?style=flat-square)](https://github.com/XaFF-XaFF/Shellcodev/releases/tag/v2.0.0)

![1](https://raw.githubusercontent.com/XaFF-XaFF/Shellcodev/master/screenshots/1.png?raw=true)

## Attribution 
This project is based on [WinREPL](https://github.com/XaFF-XaFF/WinREPL) by zerosum0x0

### Commands 

```
.help                   Show this help screen.
.registers              Show more detailed register info.
.list                   Show list of previously executed assembly instructions.
.edit line              Edit specified line in list.
.del line               Delete specified line from list.
.read addr size         Read from a memory address.
.write addr hexdata     Write to a memory address.
.toshell format         Convert list to selected shellcode format. Available formats: c
.allocate size          Allocate a memory buffer.
.loadlibrary path       Load a DLL into the process.
.kernel32 func          Get address of a kernel32 export.
.shellcode hexdata      Execute raw shellcode.
.peb                    Loads PEB into accumulator.
.reset                  Start a new environment.
.quit                   Exit the program.
```

### Added features

All the instructions provided by user are now stored. User is now able to list, edit and delete instructions which makes
shellcodes much easier to modify. Everything is in real-time, so any changes made in list also changes the register values. 

![2](https://raw.githubusercontent.com/XaFF-XaFF/Shellcodev/master/screenshots/2.png?raw=true)

### Goal features

- String converter: String provided by user will be automatically converted to hex and encoded with little endian. In case of nullbytes, they
will be removed by encrypting data with xor. 
- Shellcode runner: User will be able to test shellcode by injecting it into the process.
- More formats.

### References 
Libraries used to assemble instructions:
- [AsmJit](https://github.com/asmjit/asmjit)
- [AsmTK](https://github.com/asmjit/asmtk)
