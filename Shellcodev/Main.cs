using System;
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
            if (String.IsNullOrEmpty(instructionTxt.Text))
                return;

            new Instruction(instructionTxt.Text);
        }

        private void instructionTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
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
