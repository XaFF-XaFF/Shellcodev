using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Shellcodev
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

        private void addinstructionBtn_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(instructionTxt.Text))
                return;

            new Instruction(instructionTxt.Text);
        }

        private void makeBtn_Click(object sender, EventArgs e)
        {
            string inst = null;
            foreach(DataGridViewRow row in instructionGrid.Rows)
            {
                for(int i = 0; i < row.Cells.Count; i++)
                {
                    if (row.Cells[i].ColumnIndex == 1 && row.Cells[i + 1].Value != null)
                    {
                        inst += row.Cells[i].Value.ToString() + ", ";
                    }
                    else if (row.Cells[i].Value != null)
                    {
                        inst += row.Cells[i].Value.ToString() + " ";
                    }
                    else continue;
                }
                Console.WriteLine(inst);
                inst = null;
            }
        }

        private void instructionTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                addinstructionBtn_Click(sender, e);
                instructionTxt.SelectAll();

                //Disabling annoying bimbows ding sound on enter
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
