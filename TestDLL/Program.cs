using System;
using System.Runtime.InteropServices;

namespace TestDLL
{
    internal class Program
    {
        [DllImport("instrHandler_x86.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);

        static void Main(string[] args)
        {
            string instruction = "mov eax,0x10";
            //IntPtr mem = AssembleInstructions(instruction);
            //string v = Marshal.PtrToStringAnsi(mem);
            //Console.WriteLine(v);
            Console.ReadLine();
        }
    }
}
