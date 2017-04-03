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
using System.Diagnostics;

namespace IMAPclientApp
{
    /// <summary>
    /// Interaction logic for displayWindow.xaml
    /// </summary>
    public partial class displayWindow : Window
    {
        private string username;
        public displayWindow()
        {
            InitializeComponent();
        }

        public displayWindow(string email):this()
        {
            username = email;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usernameTextBlock.Text = username + "INBOX";
        }
    }
}
