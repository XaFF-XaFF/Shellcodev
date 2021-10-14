using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shellcodev
{
    class API
    {
        [DllImport("instrhandle.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AssembleInstructions(string instruction);
    }

    public class AssemblyHandler
    {
        public string Assembler(string instructions)
        {
            IntPtr pointer = API.AssembleInstructions(instructions);
            string bytes = Marshal.PtrToStringAnsi(pointer);

            string temp = null;
            for(int i = 0; i < bytes.Length; i++)
            {
                if(i % 2 != 0) //Starting from 0, place space every second byte
                    temp += bytes[i] + " ";
                else
                    temp += bytes[i];
            }

            return temp;
        }
    }
}
