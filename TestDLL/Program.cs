using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL
{
    internal class Program
    {
        [DllImport("InstructionHandler.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);

        static void Main(string[] args)
        {
            string instruction = "xor eax,eax";

            IntPtr mem = AssembleInstructions(instruction);
            string v = Marshal.PtrToStringAnsi(mem);
            Console.WriteLine(v);
            Console.ReadLine();
        }
    }
}
