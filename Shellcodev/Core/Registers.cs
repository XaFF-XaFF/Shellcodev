using Shellcodev.Forms;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Shellcodev.CONTEXTS
{
    class API
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT lpContext);


        public enum CONTEXT_FLAGS : uint
        {
            CONTEXT_i386 = 0x10000,
            CONTEXT_i486 = 0x10000,   //  same as i386
            CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
            CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
            CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
            CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
            CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
            CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
            CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
            CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLOATING_SAVE_AREA
        {
            public uint ControlWord;
            public uint StatusWord;
            public uint TagWord;
            public uint ErrorOffset;
            public uint ErrorSelector;
            public uint DataOffset;
            public uint DataSelector;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] RegisterArea;
            public uint Cr0NpxState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public uint ContextFlags; //set this to an appropriate value
                                      // Retrieved by CONTEXT_DEBUG_REGISTERS
            public uint Dr0;
            public uint Dr1;
            public uint Dr2;
            public uint Dr3;
            public uint Dr6;
            public uint Dr7;
            // Retrieved by CONTEXT_FLOATING_POINT
            public FLOATING_SAVE_AREA FloatSave;
            // Retrieved by CONTEXT_SEGMENTS
            public uint SegGs;
            public uint SegFs;
            public uint SegEs;
            public uint SegDs;
            // Retrieved by CONTEXT_INTEGER
            public uint Edi;
            public uint Esi;
            public uint Ebx;
            public uint Edx;
            public uint Ecx;
            public uint Eax;
            // Retrieved by CONTEXT_CONTROL
            public uint Ebp;
            public uint Eip;
            public uint SegCs;
            public uint EFlags;
            public uint Esp;
            public uint SegSs;
            // Retrieved by CONTEXT_EXTENDED_REGISTERS
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] ExtendedRegisters;
        }
    }
}

namespace Shellcodev.Core
{

    class Registers
    {
        private void RegisterParser(CONTEXTS.API.CONTEXT context, string[] split)
        {
            var main = Main.ReturnInstance();

            object[] vs = { context.Eip, context.Esp, context.Ebp };

            for (int i = 0; i < split.Length; i++)
            {
                if (i % 2 != 0 && i < 4)
                {
                    int hex = Convert.ToInt32(vs[i - 1]);
                    string tohex = hex.ToString("X8");

                    split[i] = tohex;
                }
            }

            string str = string.Join(" ", split);
            main.pointersBox.Text = str;
        }

        public void SetRegisters()
        {
            var main = Main.ReturnInstance();
            var context = new CONTEXTS.API.CONTEXT();

            context.ContextFlags = (uint)CONTEXTS.API.CONTEXT_FLAGS.CONTEXT_ALL;
            IntPtr hThread = CONTEXTS.API.GetCurrentThread();

            string[] split = main.pointersBox.Text.Split(' ');

            if (CONTEXTS.API.GetThreadContext(hThread, ref context))
            {
                RegisterParser(context, split);
            }
            else //Add GetLastError api call
            {
                MessageBox.Show("Error occurred while extracting pointers values.");
            }
        }
    }
}
