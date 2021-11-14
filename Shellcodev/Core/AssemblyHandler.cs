using Shellcodev.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Shellcodev
{
    public class API
    {
        #region Structures
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
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 dwProcessID;
            public Int32 dwThreadID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public Int32 Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        public enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
        #endregion

        [DllImport("instrHandler_x86.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);

        [DllImport("instrHandler_x86.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern IntPtr GetRegisters(string instruction, PROCESS_INFORMATION* pi);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes,
        bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment,
        string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

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

            SetRegisters(instructions, Main.pi);

            //Starting from 0, place space every second byte
            string temp = null;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % 2 != 0)
                    temp += bytes[i] + " ";
                else
                    temp += bytes[i];
            }

            return temp;
        }

        #region Registers
        private void AppendPointers(object[] pointers)
        {
            List<string> list = new List<string>();
            string[] registers = { "EIP: ", "ESP: ", "EBP: " };
            var main = Main.ReturnInstance();

            for (int i = 0; i < pointers.Length; i++)
            {
                int toHex = Convert.ToInt32(pointers[i]);
                string hex = toHex.ToString("X8");
                list.Add(registers[i] + hex);
            }

            string str = string.Join(" ", list);
            main.pointersBox.Text = str;
        }

        private void AppendIndexes(object[] indexes)
        {
            List<string> list = new List<string>();
            string[] registers = { "EDI: ", "ESI: " };
            var main = Main.ReturnInstance();

            for (int i = 0; i < indexes.Length; i++)
            {
                int toHex = Convert.ToInt32(indexes[i]);
                string hex = toHex.ToString("X8");
                list.Add(registers[i] + hex);
            }

            string str = string.Join(" ", list);
            main.indexesBox.Text = str;
        }

        private void AppendRegisters(object[] registers)
        {
            List<string> list = new List<string>();
            string[] regs = { "EAX: ", "EBX: ", "ECX: ", "EDX: " };
            var main = Main.ReturnInstance();

            for (int i = 0; i < registers.Length; i++)
            {
                int toHex = Convert.ToInt32(registers[i]);
                string hex = toHex.ToString("X8");
                list.Add(regs[i] + hex);
            }

            string str = string.Join(" ", list);
            main.registersBox.Text = str;
        }

        private void Appender(API.Registers registers)
        {
            object[] pointers = { registers.eip, registers.esp, registers.ebp };
            object[] indexes = { registers.edi, registers.esi };
            object[] regs = { registers.eax, registers.ebx, registers.ecx, registers.edx };

            AppendPointers(pointers);
            AppendIndexes(indexes);
            AppendRegisters(regs);
        }

        public unsafe void SetRegisters(string instruction, API.PROCESS_INFORMATION pi)
        {
            IntPtr pointer = API.GetRegisters(instruction, &pi);
            API.Registers registers = Marshal.PtrToStructure<API.Registers>(pointer);
            Appender(registers);
        }
        #endregion
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