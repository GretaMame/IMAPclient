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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IMAPclientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            // IMAPclient newSession = new IMAPclient("imap.gmail.com", 993, emailTextBox.Text, passwordBox.Password);
            IMAPclient newSession = new IMAPclient("imap.gmail.com", 993, "gretuka27@gmail.com", "pasgausiujeiatspesi");
            newSession.Connect();
            newSession.LogIn();
            newSession.LogOut();
            OpenDisplayWindow();

        }

        private void OpenDisplayWindow()
        {
            //displayWindow window = new displayWindow(emailTextBox.Text);
            displayWindow window = new displayWindow("gretuka27@gmail.com");
            window.Show();
            this.Close();
        }
    }
}
