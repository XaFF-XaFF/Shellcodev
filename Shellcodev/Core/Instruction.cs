using Shellcodev.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace Shellcodev
{
    public class InstrctuionValidator
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

    public class Instruction
    {
        public string instruction;
        private static int rowId = 1;

        public Instruction(string instruction)
        {
            this.instruction = instruction;

            Main main = Main.ReturnInstance();
            int rows = main.instructionGrid.Rows.Add(rowId);
            DataGridViewRow row = main.instructionGrid.Rows[rows];

            row.Cells["Instruction"].Value = instruction;
            row.HeaderCell.Value = (row.Index + 1).ToString();

            AssemblyHandler handler = new AssemblyHandler();
            string bytes = handler.Assembler(instruction);

            ByteAppender(main, bytes);
        }

        private void ByteAppender(Main main, string bytes)
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
