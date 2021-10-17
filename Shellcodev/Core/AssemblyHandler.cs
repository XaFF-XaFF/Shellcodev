using System;
using System.Runtime.InteropServices;

namespace Shellcodev
{
    class API
    {
        [DllImport("instrhandle.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procname);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string name);
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
}
