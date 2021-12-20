using Shellcodev;
using System;
using System.Windows;
using System.Windows.Input;

namespace Shellcodevv
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
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

            if(!createproc)
            {
                MessageBox.Show("ERROR! CreateProcess Failed");
                this.Close();
            }
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
            string instruction = instructionTxt.Text;
            if (string.IsNullOrEmpty(instruction))
                return;

            Instructions instructions = new Instructions();
            AssemblyHandler handler = new AssemblyHandler();

            instructions.instruction = instruction;
            instructionGrid.Items.Add(instructions);

            handler.SetRegisters(instruction, pi);

            instructionTxt.SelectAll();
        }

        private void instructionTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                addBtn_Click(sender, e);
        }
    }

    public class Instructions
    {
        public string instruction { get; set; }
    }
}
