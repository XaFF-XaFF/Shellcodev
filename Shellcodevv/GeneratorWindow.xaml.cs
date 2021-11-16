using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Shellcodevv
{
    /// <summary>
    /// Logika interakcji dla klasy GeneratorWindow.xaml
    /// </summary>
    public partial class GeneratorWindow : Window
    {
        public GeneratorWindow()
        {
            InitializeComponent();
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
    }
}
