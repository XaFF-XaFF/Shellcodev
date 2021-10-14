using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Shellcodev
{
    public class InstrctuionValidator
    {
        //Validate instructions
        public bool ValidateInstruction()
        {

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

            row.Cells["Instructions"].Value = instruction;
            row.HeaderCell.Value = (row.Index + 1).ToString();

            AssemblyHandler handler = new AssemblyHandler();
            string bytes = handler.Assembler(instruction);

            main.bytesBox.AppendText(bytes);
        }
    }
}
