using Shellcodev.Core;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Shellcodev.Forms
{
    public partial class Main : Form
    {
        private static int previousIndex;
        private static Main instance;
        public static Main ReturnInstance()
        {
            return instance;
        }

        //TODO: Show registers value at runtime
        //      Repair gridview index lenght, 2 digit number looks like 1 digit number
        //      After applying changes select the last row
        //      https://github.com/asmjit/asmjit/issues/27

        public Main()
        {
            InitializeComponent();
            instance = this;
            instructionGrid.AllowUserToAddRows = false;
        }

        public void ByteAppender(string bytes)
        {
            var box = this.bytesBox;
            string[] split = bytes.Split(' ');

            foreach (string line in split)
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

        private void getAddrBtn_Click(object sender, EventArgs e)
        {
            string dll = dllAddrBox.Text;
            string function = funcTxt.Text;

            var lib = API.LoadLibrary(dll);
            var address = API.GetProcAddress(lib, function);
            string hexValue = address.ToString("X");

            if (MessageBox.Show("0x" + hexValue, "Function address (Press OK to copy)", MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                Clipboard.SetText("0x" + hexValue);
        }

        private void generateBtn_Click(object sender, EventArgs e)
        {
            string bytes = bytesBox.Text;
            if (csRBtn.Checked)
                new Generator(bytes, true);
            else if (cRBtn.Checked)
                new Generator(bytes, false);
            else
                return;
        }

        #region InstructionRegion
        private void addInstructionBtn_Click(object sender, EventArgs e)
        {
            //Disable row sorting
            instructionGrid.Columns.Cast<DataGridViewColumn>().ToList().ForEach(f => f.SortMode = DataGridViewColumnSortMode.NotSortable);

            if (String.IsNullOrEmpty(instructionTxt.Text))
                return;

            new Instruction(instructionTxt.Text);
        }

        private void instructionTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                addInstructionBtn_Click(sender, e);
                instructionTxt.SelectAll();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        //Repair
        private void instructionGrid_SelectionChanged(object sender, EventArgs e)
        {
            int index = instructionGrid.CurrentCell.RowIndex;
            Console.WriteLine("Selection number: " + index);

            //Repair
            for(int i = 0; i < bytesBox.Lines.Count(); i++)
            {
                if(index == i)
                {
                    if(previousIndex != i)
                    {
                        int searchForPrevius = bytesBox.Text.IndexOf(bytesBox.Lines[previousIndex]);
                        bytesBox.Select(searchForPrevius, bytesBox.Lines[previousIndex].Length);
                        bytesBox.SelectionColor = Color.Black;

                        if(bytesBox.Lines[previousIndex].Contains("00"))
                        {
                            int selectStart = bytesBox.SelectionStart;

                            while ((index = bytesBox.Text.IndexOf("00", (index + 1))) != -1)
                            {
                                bytesBox.Select((index + 0), "00".Length);
                                bytesBox.SelectionColor = Color.Red;
                                bytesBox.Select(selectStart, 0);
                                bytesBox.SelectionColor = Color.Black;
                            }
                        }
                    }

                    previousIndex = i;
                    int search = bytesBox.Text.IndexOf(bytesBox.Lines[i]);
                    bytesBox.Select(search, bytesBox.Lines[i].Length);
                    bytesBox.SelectionColor = Color.Blue;
                }
            }

            int lastRow = instructionGrid.Rows.Count - 1;
            instructionGrid.Rows[lastRow].Cells["Instruction"].Selected = true;
        }

        private void instructionGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AssemblyHandler handler = new AssemblyHandler();

            int editedRow = e.RowIndex;
            DataGridViewRow row = instructionGrid.Rows[editedRow];

            var rowValue = row.Cells[0].Value;
            if (rowValue == null) //Remove row from grid
            {
                try //Editing empty row and setting it to null
                { instructionGrid.Rows.Remove(row); }
                catch (Exception)
                { return; }  
                
                //Resort indexes
                for(int i = editedRow;  i < instructionGrid.Rows.Count; i++)
                {
                    DataGridViewRow dgvr = instructionGrid.Rows[i];
                    if(dgvr.Cells[0].Value != null)
                        dgvr.HeaderCell.Value = (dgvr.Index + 1).ToString();
                }

                //Remove line from textbox
                int startIndex = bytesBox.GetFirstCharIndexFromLine(editedRow);
                int count = bytesBox.Lines[editedRow].Length;

                if(editedRow < bytesBox.Lines.Length - 1)
                {
                    count += bytesBox.GetFirstCharIndexFromLine(editedRow + 1) -
                        ((startIndex + count - 1) + 1);
                }
                bytesBox.Text = bytesBox.Text.Remove(startIndex, count);

                return;
            }

            string bytes = handler.Assembler(rowValue.ToString());
            int search = bytesBox.Text.IndexOf(bytesBox.Lines[editedRow]);

            bytesBox.Select(search, bytesBox.Lines[editedRow].Length);
            bytesBox.SelectedText = bytes;
        }
        #endregion

        #region RadioButtons
        private void cRBtn_CheckedChanged(object sender, EventArgs e)
        {
            if(csRBtn.Checked)
                csRBtn.Checked = false;
        }

        private void csRBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (cRBtn.Checked)
                cRBtn.Checked = false;
        }
        #endregion

        #region Testing
        private void shlcTestBtn_Click(object sender, EventArgs e)
        {
            string text = bytesBox.Text;
            string[] byteArray = text.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string temp = null;
            for (int i = 0; i < byteArray.Length; i++)
            {
                if (i == byteArray.Length - 1)
                    temp += "0x" + byteArray[i];
                else
                    temp += "0x" + byteArray[i] + ", ";
            }

            byte[] converted = temp.Split(new[] { ", " }, StringSplitOptions.None)
                                .Select(str => Convert.ToByte(str, 16))
                                .ToArray();

            new ShellcodeLoader(converted);
        }

        private void testerBldBtn_Click(object sender, EventArgs e)
        {
            string text = bytesBox.Text;
            string[] byteArray = text.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string temp = null;
            for (int i = 0; i < byteArray.Length; i++)
            {
                if (i == byteArray.Length - 1)
                    temp += "0x" + byteArray[i];
                else
                    temp += "0x" + byteArray[i] + ", ";
            }

            byte[] converted = temp.Split(new[] { ", " }, StringSplitOptions.None)
                                .Select(str => Convert.ToByte(str, 16))
                                .ToArray();

            Builder builder = new Builder();
            builder.Build(converted);
        }
        #endregion
    }
}