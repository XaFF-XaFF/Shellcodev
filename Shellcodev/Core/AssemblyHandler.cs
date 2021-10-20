using System;
using System.Runtime.InteropServices;

namespace Shellcodev
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct STARTUPINFO
    {
        public Int32 cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
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
    public struct PROCESS_BASIC_INFORMATION
    {
        public IntPtr ExitStatus;
        public IntPtr PebAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniquePID;
        public IntPtr InheritedFromUniqueProcessId;
    }
    public enum ProcessCreationFlags : UInt32
    {
        CREATE_NO_WINDOW = 0x08000000,
        CREATE_SUSPENDED = 0x00000004,
        DETACHED_PROCESS = 0x00000008
    }
    class API
    {
        [DllImport("instrHandler_x86.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procname);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, ProcessCreationFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern UInt32 ZwQueryInformationProcess(IntPtr hProcess, int procInformationClass, ref PROCESS_BASIC_INFORMATION procInformation, UInt32 ProcInfoLen, ref UInt32 retlen);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr hThread);
    }

    public class AssemblyHandler
    {
        public string Assembler(string instructions)
        {
            IntPtr pointer = API.AssembleInstructions(instructions);
            string bytes = Marshal.PtrToStringAnsi(pointer);
            //TODO: Add error checker

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
    }

    public class ShellcodeLoader
    {
        public ShellcodeLoader(byte[] shellcode)
        {
            // Create a new process in a suspended state
            STARTUPINFO lpStartupInfo = new STARTUPINFO();
            PROCESS_INFORMATION lpProcessInformation = new PROCESS_INFORMATION();
            API.CreateProcess(null, "C:\\Windows\\System32\\svchost.exe", IntPtr.Zero, IntPtr.Zero, false, ProcessCreationFlags.CREATE_SUSPENDED | ProcessCreationFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref lpStartupInfo, out lpProcessInformation);

            // locate the PEB inside the process
            PROCESS_BASIC_INFORMATION procInformation = new PROCESS_BASIC_INFORMATION();
            uint tmp = 0;
            IntPtr hProcess = lpProcessInformation.hProcess;
            API.ZwQueryInformationProcess(hProcess, 0x0, ref procInformation, (uint)(IntPtr.Size * 6), ref tmp);

            // locate the image base - PEB + 0x10
            IntPtr ptrToImageBase = (IntPtr)((Int64)procInformation.PebAddress + 0x10);

            // read the process memory
            byte[] addrBuf = new byte[IntPtr.Size];
            IntPtr nRead = IntPtr.Zero;
            API.ReadProcessMemory(hProcess, ptrToImageBase, addrBuf, addrBuf.Length, out nRead);

            // locate svchost base, converted to a 64-bit integer then cast to an IntPtr
            IntPtr svchostBase = (IntPtr)(BitConverter.ToInt64(addrBuf, 0));

            // read the memory location to get the entry point from the PE header
            byte[] data = new byte[0x200];
            //byte[] data = new byte[49696];
            API.ReadProcessMemory(hProcess, svchostBase, data, data.Length, out nRead);

            uint e_lfanew_offset = BitConverter.ToUInt32(data, 0x3C);
            uint opthdr = e_lfanew_offset + 0x28;
            uint entrypoint_rva = BitConverter.ToUInt32(data, (int)opthdr);
            IntPtr addressOfEntryPoint = (IntPtr)(entrypoint_rva + (UInt64)svchostBase);

            API.WriteProcessMemory(hProcess, addressOfEntryPoint, shellcode, shellcode.Length, out nRead);

            API.ResumeThread(lpProcessInformation.hThread);
        }
    }
}
