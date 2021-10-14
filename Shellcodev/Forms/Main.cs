using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Shellcodev.Forms
{
    public partial class Main : Form
    {
        private static Main instance;
        public static Main ReturnInstance()
        {
            return instance;
        }

        public Main()
        {
            InitializeComponent();
            instance = this;
        }

        private void addInstructionBtn_Click(object sender, EventArgs e)
        {
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

        private static int previousIndex;
        private void instructionGrid_SelectionChanged(object sender, EventArgs e)
        {
            int index = instructionGrid.CurrentCell.RowIndex;

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
        }

        private void instructionGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //On edit finish, update bytes in textbox
        }
    }
}
