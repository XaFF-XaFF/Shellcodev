using Shellcodev.Forms;
using System.Windows.Forms;

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

        private static int rowId = 1;
        public void SnippetParser(Main instance, string register, string[] bytes)
        {
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                int rows = instance.instructionGrid.Rows.Add(rowId);
                DataGridViewRow row = instance.instructionGrid.Rows[rows];

                if (bytes[i].StartsWith("xor"))
                {
                    row.Cells["Instruction"].Value = "mov " + register + ", " + bytes[i].Substring(3);

                    int _rows = instance.instructionGrid.Rows.Add(rowId);
                    DataGridViewRow _row = instance.instructionGrid.Rows[_rows];

                    _row.Cells["Instruction"].Value = "xor " + register + ", 0x11111111";

                    _rows = instance.instructionGrid.Rows.Add(rowId);
                    _row = instance.instructionGrid.Rows[_rows];

                    _row.Cells["Instruction"].Value = "push " + register;
                }
                else
                {
                    row.Cells["Instruction"].Value = "push " + bytes[i];
                }
            }

            int _rows1 = instance.instructionGrid.Rows.Add(rowId);
            DataGridViewRow _row1 = instance.instructionGrid.Rows[_rows1];

            _row1.Cells["Instruction"].Value = "mov " + register + ", esp";
        }

        public void SnippetAppender(Main instance, string register, string[] bytes)
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