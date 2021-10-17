using System;
using System.Windows.Forms;

namespace Shellcodev.Forms
{
    public partial class Generator : Form
    {
        public Generator()
        {
            InitializeComponent();
            this.Show();
        }

        public Generator(string bytes, bool format) : this()
        {
            if (format)
                CSFormat(bytes);
            else
                CFormat(bytes);
        }

        private void CSFormat(string bytes)
        {
            string[] byteArray = bytes.Split(new char[] {' ','\n'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in byteArray)
                shellTxt.Text += "0x" + str + " ";
        }

        private void CFormat(string bytes)
        {
            string[] byteArray = bytes.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //Make unsigned char bytes = 15 bytes in line in double quotes
            foreach (string str in byteArray)
                shellTxt.Text += @"\x" + str;
        }
    }
}
