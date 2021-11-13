using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Shellcodev
{
    class API
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Registers
        {
            public int eax;
            public int ebx;
            public int ecx;
            public int edx;
            public int esi;
            public int edi;
            public int eip;
            public int esp;
            public int ebp;
        }

        [DllImport("instrHandler_x86.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);

        [DllImport("instrHandler_x86.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetRegisters(string instruction);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
    }

    public class AssemblyHandler
    {
        public string Assembler(string instructions)
        {
            IntPtr pointer = API.AssembleInstructions(instructions);
            string bytes = Marshal.PtrToStringAnsi(pointer);
            if (bytes == "InvalidInstruction")
                return "Error!: Invalid instruction.";

            //Starting from 0, place space every second byte
            string temp = null;
            for(int i = 0; i < bytes.Length; i++)
            {
                if(i % 2 != 0)
                    temp += bytes[i] + " ";
                else
                    temp += bytes[i];
            }

            return temp;
        }

        private void AppendRegisters(API.Registers registers)
        {

        }

        public void SetRegisters(string instruction)
        {
            IntPtr pointer = API.GetRegisters(instruction);
            var registers = Marshal.PtrToStructure<API.Registers>(pointer);

            Console.WriteLine("EIP REGISTER : {0}", registers.eip);
        }
    }

    public class ShellcodeLoader
    {
        public ShellcodeLoader(byte[] shellcode)
        {
            int pid = Process.Start("notepad.exe").Id;
            IntPtr pHandle = API.OpenProcess(0x1F0FFF, false, pid);

            IntPtr memAlloc = API.VirtualAllocEx(pHandle, IntPtr.Zero, (uint)shellcode.Length, 0x00001000, 0x40);

            UIntPtr bytesWritten;
            API.WriteProcessMemory(pHandle, memAlloc, shellcode, (uint)shellcode.Length, out bytesWritten);

            API.CreateRemoteThread(pHandle, IntPtr.Zero, 0, memAlloc, IntPtr.Zero, 0, IntPtr.Zero);
        }
    }
}
