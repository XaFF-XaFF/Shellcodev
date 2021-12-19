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

            Console.WriteLine(instruction);

            Instructions instructions = new Instructions();
            AssemblyHandler handler = new AssemblyHandler();

            instructions.instruction = instruction;

            instructionGrid.Items.Add(instructions);

            Console.WriteLine(handler.Assembler(instruction));
        }
    }

    public class Instructions
    {
        public string instruction { get; set; }
    }
}
