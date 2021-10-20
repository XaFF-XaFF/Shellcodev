using System;
using System.Runtime.InteropServices;

namespace Shellcodev
{
    class API
    {
        [DllImport("instrHandler_x86.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procname);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32")]
        public static extern IntPtr VirtualAlloc(
            IntPtr lpAddress, uint dwSize, 
            uint flAllocationType, uint flProtect);

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        public static extern IntPtr CreateThread(
            IntPtr lpThreadAttributes, uint dwStackSize, 
            IntPtr lpStartAddress, IntPtr lpParameter, 
            uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern uint WaitForSingleObject(
            IntPtr hHandle,
            uint dwMilliseconds);
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
        public ShellcodeLoader(byte[] bytes)
        {
            IntPtr pointer = API.VirtualAlloc(IntPtr.Zero, (uint)bytes.Length, 0x1000, 0x40);
            Marshal.Copy(bytes, 0, pointer, bytes.Length);

            IntPtr hThread = API.CreateThread(IntPtr.Zero, 0, pointer, IntPtr.Zero, 0, IntPtr.Zero);
            API.WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
    }
}
