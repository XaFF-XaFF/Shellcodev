using Shellcodev.Core;
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
            var parser = new Snippet();
            var main = Forms.Main.ReturnInstance();

            string[] bytes = null;
            bool array = false;
            string tempBytes = null;

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
                parser.SnippetParser(main, register, bytes);
                parser.SnippetAppender(main, register, bytes);
                array = true;
            }
            else
            {
                int rows = main.instructionGrid.Rows.Add(rowId);
                DataGridViewRow row = main.instructionGrid.Rows[rows];

                row.Cells["Instruction"].Value = instruction;
                row.HeaderCell.Value = (row.Index + 1).ToString();

                tempBytes = handler.Assembler(instruction);
                main.ByteAppender(tempBytes);
            }
        
            //if(array)
            //{
            //    parser.SnippetParser(main, register, bytes);
            //    parser.SnippetAppender(main, register, bytes);
            //}
            //else
            //{
            //    int rows = main.instructionGrid.Rows.Add(rowId);
            //    DataGridViewRow row = main.instructionGrid.Rows[rows];

            //    row.Cells["Instruction"].Value = instruction;
            //    row.HeaderCell.Value = (row.Index + 1).ToString();

            //    tempBytes = handler.Assembler(instruction);
            //    main.ByteAppender(tempBytes);
            //}   
        }
    }
}