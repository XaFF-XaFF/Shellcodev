using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestDLL
{
    public static class Test
    {
        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (int i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }

    internal class Program
    {
        [DllImport("instrhandle.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);


        static void Main(string[] args)
        {
            string instruction = Console.ReadLine();
            string temp = null;
            int len = 8 - instruction.Length;
            for(int i = 0; i < len; i++)
            {
                instruction = "0" + instruction;
            }
            Console.WriteLine(instruction);
            Console.ReadLine();

            //string instruction = "xor eax,eax";

            //IntPtr mem = AssembleInstructions(instruction);
            //string v = Marshal.PtrToStringAnsi(mem);
            //Console.WriteLine(v);
            //Console.ReadLine();

            //byte[] bytes = Encoding.Default.GetBytes("lol");
            //var hexString = BitConverter.ToString(bytes);
            //Console.WriteLine(hexString);
            //Console.ReadLine();
        }
    }
}
