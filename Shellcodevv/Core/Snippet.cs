using Shellcodevv;
using System.Collections.Generic;

namespace Shellcodev.Core
{
    class Snippet
    {
        public string GetAddress(string dll, string function)
        {
            var lib = API.LoadLibrary(dll + ".dll");
            var procaddr = API.GetProcAddress(lib, function);
            string hexValue = procaddr.ToString("X");

            return "0x" + hexValue;
        }

        public void SnippetParser(MainWindow main, string register, string[] bytes)
        {
            var items = new List<Instructions>();
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i].StartsWith("xor"))
                {
                    items.Add(new Instructions { instruction = "mov " + register + ", " + bytes[i].Substring(3) });
                    main.instructionGrid.Items.Add(items);

                    items.Add(new Instructions { instruction = "xor " + register + ", 0x11111111" });
                    main.instructionGrid.Items.Add(items);

                    items.Add(new Instructions { instruction = "push " + register });
                    main.instructionGrid.Items.Add(items);
                }
                else
                {
                    items.Add(new Instructions { instruction = "push " + bytes[i] });
                    main.instructionGrid.Items.Add(items);
                }
            }
            items.Add(new Instructions { instruction = "mov " + register + ", esp" });
            main.instructionGrid.Items.Add(items);
        }

        public void SnippetAppender(MainWindow instance, string register, string[] bytes)
        {
            var handler = new AssemblyHandler();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i].StartsWith("xor"))
                {
                    instance.ByteAppender(handler.Assembler("mov " + register + ", " + bytes[i].Substring(3)));
                    instance.ByteAppender(handler.Assembler("xor " + register + ", 0x11111111"));
                    instance.ByteAppender(handler.Assembler("push " + register));
                }
                else
                {
                    instance.ByteAppender(handler.Assembler("push " + bytes[i]));
                }
            }

            instance.ByteAppender(handler.Assembler("mov " + register + ", esp"));
        }
    }
}