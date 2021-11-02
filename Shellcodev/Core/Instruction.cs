using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shellcodev
{
    public class InstructionConverter
    {
        public bool bitwise = false;
        private string EncodeValues(string instructionPart, bool small)
        {
            byte[] bytes = Encoding.Default.GetBytes(instructionPart);
            var hexString = BitConverter.ToString(bytes);
            string[] splited = hexString.Split('-');

            List<string> result = new List<string>();
            for(int i = splited.Length - 1; i >= 0; i--)
                result.Add(splited[i]);

            string temp = null;
            foreach (string str in result)
                temp += str;

            //Testing if push contains nullbytes
            AssemblyHandler handler = new AssemblyHandler();
            string test = handler.Assembler("push 0x" + temp);
            string temp1 = null;
            for(int i = 0; i < test.Length; i++)
            {
                if(temp1 != null && temp1.Length % 2 == 0)
                {
                    if (temp1 == "00")
                    {
                        int value = Convert.ToInt32("0x" + temp, 16);
                        int key = Convert.ToInt32("0x11111111", 16);

                        int res = value ^ key;
                        string hexResult = res.ToString("X");

                        bitwise = true;
                        return "xor" + "0x" + hexResult;
                    }
                    else
                        temp1 = null;
                }
                temp1 += test[i];
            }

            return "0x" + temp;
        }

        public string[] StringAssembler(string instruction)
        {
            AssemblyHandler handler = new AssemblyHandler();
            List<string> list = new List<string>();
            double partSize = 4;
            int k = 0;

            // Extracting string from double quotes
            var stringArray = instruction.Split('"');

            // Splitting string
            var output = stringArray[1]
                .ToLookup(c => Math.Floor(k++ / partSize))
                .Select(e => new String(e.ToArray()));

            List<string> result = new List<string>();
            foreach (string str in output)
            {
                if (str.Length < 4)
                    result.Add(EncodeValues(str, true));
                else
                    result.Add(EncodeValues(str, false));
            }

            return result.ToArray();
        }
    }

    public class Instruction
    {
        public string register;
        private static int rowId = 1;

        public Instruction(string instruction)
        {
            var converter = new InstructionConverter();
            var handler = new AssemblyHandler();
            var main = Forms.Main.ReturnInstance();

            string[] bytes = null;
            bool array = false;

            // Extract register from command
            try { this.register = instruction.Substring(3, 4); }
            catch(Exception)
            { }

            // Check if instruction contains double quotes and if yes execute StringAssembler
            // This function is used to automate process of string appendance into the shellcode.
            // Features: Stack is built vice versa. Strings are splitted to 4 chars each and encoded with little endian.
            // Strings that contain nullbytes are xored to avoid shellcode from termination
            if (instruction.Contains("\""))
            {
                bytes = converter.StringAssembler(instruction);
                array = true;
            }

            int counter = 1;
            if(array)
            {
                string lastValue = bytes.Last();

                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    int rows = main.instructionGrid.Rows.Add(rowId);
                    DataGridViewRow row = main.instructionGrid.Rows[rows];

                    if (bytes[i].StartsWith("xor"))
                    {
                        row.Cells["Instruction"].Value = "mov " + register + ", " + bytes[i].Substring(3);
                        row.HeaderCell.Value = (row.Index + counter++).ToString();

                        int _rows = main.instructionGrid.Rows.Add(rowId);
                        DataGridViewRow _row = main.instructionGrid.Rows[_rows];

                        _row.Cells["Instruction"].Value = "xor " + register + ", 0x11111111";
                        _row.HeaderCell.Value = (row.Index + counter++).ToString();

                        _rows = main.instructionGrid.Rows.Add(rowId);
                        _row = main.instructionGrid.Rows[_rows];

                        _row.Cells["Instruction"].Value = "push " + register;
                        _row.HeaderCell.Value = (row.Index + counter++).ToString();
                    }
                    else
                    {
                        row.Cells["Instruction"].Value = "push " + bytes[i];
                        row.HeaderCell.Value = (row.Index + 2).ToString();
                    }
                }

                int _rows1 = main.instructionGrid.Rows.Add(rowId);
                DataGridViewRow _row1 = main.instructionGrid.Rows[_rows1];

                _row1.Cells["Instruction"].Value = "mov " + register + ", esp";
                _row1.HeaderCell.Value = (_row1.Index + counter++).ToString();
            }
            else
            {
                int rows = main.instructionGrid.Rows.Add(rowId);
                DataGridViewRow row = main.instructionGrid.Rows[rows];

                row.Cells["Instruction"].Value = instruction;
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }

            string tempBytes = null;
            if (array)
            {
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    if (bytes[i].StartsWith("xor"))
                    {
                        tempBytes = handler.Assembler("mov " + register + ", " + bytes[i].Substring(3));
                        ByteAppender(main, tempBytes);

                        tempBytes = handler.Assembler("xor " + register + ", 0x11111111");
                        ByteAppender(main, tempBytes);

                        tempBytes = handler.Assembler("push " + register);
                        ByteAppender(main, tempBytes);
                    }
                    else
                    {
                        tempBytes = handler.Assembler("push " + bytes[i]);
                        ByteAppender(main, tempBytes);
                    }
                }

                tempBytes = handler.Assembler("mov " + register + ", esp");
                ByteAppender(main, tempBytes);
            }
            else
            {
                tempBytes = handler.Assembler(instruction);
                ByteAppender(main, tempBytes);
            }    
        }

        private void ByteAppender(Forms.Main main, string bytes)
        {
            var box = main.bytesBox;
            string[] split = bytes.Split(' ');

            foreach(string line in split)
            {
                // Make red instructions that have nullbytes
                if (line == "00")
                {
                    box.SelectionColor = Color.Red;
                    box.AppendText(line + " ");
                }
                else
                    box.AppendText(line + " ");
            }
            box.AppendText("\n");
        }
    }
}