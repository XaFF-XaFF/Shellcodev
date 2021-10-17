using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shellcodev
{
    public class InstructionValidator
    {
        //Validate instructions
        public bool ValidateInstruction()
        {
            //TODO
            //Make instruction validator

            //string[] split = instructionTxt.Text.Split(new string[] { ",", " "}, StringSplitOptions.None);
            //split = split.Where(x => !string.IsNullOrEmpty(x)).ToArray(); //Remove empty values from array if someused comma and space
            return true;
        }
    }

    public class InstructionConverter
    {
        private string EncodeValues(string instructionPart, bool small)
        {
            byte[] bytes = Encoding.Default.GetBytes(instructionPart);
            var hexString = BitConverter.ToString(bytes);
            string[] splited = hexString.Split('-');

            List<string> result = new List<string>();
            for(int i = splited.Length - 1; i >= 0; i--)
                result.Add(splited[i]);

            string temp = null;
            if(!small)
                foreach (string str in result)
                    temp += str;
            else
                foreach(string str in result)
                {
                    string temp2 = str;

                    for(int i = 0; i < 8 - str.Length; i++)
                        temp2 = "0" + temp2;

                    temp += temp2;
                }

            return "0x" + temp;
        }

        public string[] StringAssembler(string instruction)
        {
            AssemblyHandler handler = new AssemblyHandler();
            List<string> list = new List<string>();
            double partSize = 4;
            int k = 0;

            //Extracting strings in double quotes
            var stringArray = instruction.Split('"');

            //Splitting string
            var output = stringArray[1]
                .ToLookup(c => Math.Floor(k++ / partSize))
                .Select(e => new String(e.ToArray()));

            List<string> result = new List<string>();
            foreach(string str in output)
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
        public string instruction;
        private static int rowId = 1;

        public Instruction(string instruction)
        {
            string[] arrBytes = null;
            bool arr = false;
            var converter = new InstructionConverter();

            string register = instruction.Substring(3, 4);

            if (instruction.Contains("\""))
            {
                arrBytes = converter.StringAssembler(instruction);
                arr = true;
            }

            this.instruction = instruction;

            AssemblyHandler handler = new AssemblyHandler();
            Forms.Main main = Forms.Main.ReturnInstance();

            if (arr == true)
            {
                string lastValue = arrBytes.Last();
                foreach (string bt in arrBytes)
                {
                    int rows = main.instructionGrid.Rows.Add(rowId);
                    DataGridViewRow row = main.instructionGrid.Rows[rows];

                    row.Cells["Instruction"].Value = "push " + bt;
                    row.HeaderCell.Value = (row.Index + 1).ToString();

                    if(lastValue == bt)
                    {
                        int rows1 = main.instructionGrid.Rows.Add(rowId);
                        DataGridViewRow row1 = main.instructionGrid.Rows[rows1];

                        row1.Cells["Instruction"].Value = "mov " + register + ", esp";
                        row1.HeaderCell.Value = (row.Index + 2).ToString();
                    }
                }
            }
            else
            {
                int rows = main.instructionGrid.Rows.Add(rowId);
                DataGridViewRow row = main.instructionGrid.Rows[rows];

                row.Cells["Instruction"].Value = instruction;
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }

            string bytes = null;
            if(arr == true)
            {
                foreach(string bt in arrBytes)
                {
                    bytes = handler.Assembler("push " + bt);
                    ByteAppender(main, bytes);
                }

                //Append pointer
                bytes = handler.Assembler("mov " + register + ", esp");
                ByteAppender(main, bytes);
            }
            else
            {
                bytes = handler.Assembler(instruction);
                ByteAppender(main, bytes);
            }
        }

        private void ByteAppender(Forms.Main main, string bytes)
        {
            var box = main.bytesBox;
            string[] split = bytes.Split(' ');

            foreach(string line in split)
            {
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