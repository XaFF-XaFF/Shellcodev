# Shellcodev
Shellcodev is a tool designed to help and automate the process of shellcode creation.

### Features
  1. Snippets
  2. Instruction assembling in real time
  3. [TODO] Showing registers values in real time
  4. Testing shellcode by injecting it into the process
  5. Testing shellcode by embedding it into the executable
  6. Dll function address extractor
  7. Converting bytes into chosen format (C / C#)

### Snippets
```
"string"          Automatically converts string into hex and encodes it with little endian. 
                  If string contains nullbytes it's being XORed to avoid shellcode termination. 
                  Stack is build vice versa
                  
.dll.function     Automatically extracts function address from dll. Address is getting converted 
                  into hex and encoded with little endian         
```
