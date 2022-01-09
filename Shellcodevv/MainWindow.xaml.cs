using Shellcodev;
using Shellcodevv.Core;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Shellcodevv
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public static API.PROCESS_INFORMATION pi;
        public static API.Registers registers;

        private static MainWindow instance;
        public static MainWindow ReturnInstance()
        {
            return instance;
        }

        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            AssemblyHandler handler = new AssemblyHandler();
            InitProcess();

            handler.SetRegisters("xor eax,eax", pi);
            handler.SetRegisters("xor ebx,ebx", pi);
            handler.SetRegisters("xor ecx,ecx", pi);
        }

        private void InitProcess()
        {
            API.STARTUPINFO si = new API.STARTUPINFO();
            pi = new API.PROCESS_INFORMATION();

            bool createproc = API.CreateProcess(
                AppDomain.CurrentDomain.FriendlyName,
                null,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0x00000002,
                IntPtr.Zero,
                null,
                ref si, out pi);

            if (!createproc)
            {
                MessageBox.Show("ERROR! CreateProcess Failed");
                this.Close();
            }
        }

        public void ByteAppender(string bytes)
        {
            string[] split = bytes.Split(' ');

            foreach (string line in split)
            {
                // Make red instructions that have nullbytes
                if (line == "00")
                {
                    AppendText(line + " ", "red");
                }
                else
                {
                    AppendText(line + " ", "black");
                }
            }
            AppendText("\r", "black");
        }

            #region Buttons
            private void MinimizeBtn_Checked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitBtn_Checked(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        // Movable windows
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            Instructions instructions = new Instructions();
            AssemblyHandler handler = new AssemblyHandler();

            string instruction = instructionTxt.Text;
            if (string.IsNullOrEmpty(instruction))
                return;

            instructions.instruction = instruction;
            string bytes = handler.Assembler(instruction);

            if(bytes == null)
            {
                MessageBox.Show("Invalid instruction!");
                return;
            }

            instructionGrid.Items.Add(instructions);

            instructionTxt.SelectAll();

            new Instruction(instruction);
        }

        private void instructionTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                addBtn_Click(sender, e);
        }

        private void instructionGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        public void AppendText(string text, string color)
        {
            BrushConverter brush = new BrushConverter();
            TextRange range = new TextRange(bytesBox.Document.ContentEnd, bytesBox.Document.ContentEnd);
            range.Text = text;

            range.ApplyPropertyValue(TextElement.ForegroundProperty, brush.ConvertFromString(color));
        }
    }

    public class Instructions
    {
        public string instruction { get; set; }
    }
}
