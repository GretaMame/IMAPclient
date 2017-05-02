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
using System.Collections;

namespace IMAPclientApp
{
    /// <summary>
    /// Interaction logic for displayWindow.xaml
    /// </summary>
    public partial class displayWindow : Window
    {
        private static int DEFAULT_MESSAGE_DISPLAY_AMOUNT = 20;
        private string username;
        private IMAPclient imap;
        public displayWindow()
        {
            InitializeComponent();
        }

        public displayWindow(string email, IMAPclient imap):this()
        {
            username = email;
            this.imap = imap;
            List<EmailInfo> info = new List<EmailInfo>();
            loadEmails(ref info);
            emails.ItemsSource = info;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            usernameTextBlock.Text = username + " INBOX";
        }

        private void loadEmails(ref List<EmailInfo> info)
        {
            for (int i=0;i<DEFAULT_MESSAGE_DISPLAY_AMOUNT;i++)
            {
                ArrayList rawInfo = new ArrayList();
                rawInfo = imap.FetchMessage();
                
                //reikia paimt laisko info ir pasiimt is jo ko reikia tik
                EmailInfo email= purifyInfo(rawInfo);
                info.Add(email);
            }
        }

        private EmailInfo purifyInfo(ArrayList rawInfo)
        {
            EmailInfo pure = new EmailInfo();
            foreach(string s in rawInfo)
            {
                if (s.StartsWith("Subject:"))
                {
                    int index = s.IndexOf(":") + 1;
                    pure.Subject = s.Substring(index);
                    continue;
                }
                if (s.StartsWith("From:"))
                {
                    int index = s.IndexOf(":") + 1;
                    pure.Sender = s.Substring(index);
                    continue;
                }
                if (s.StartsWith("Date:"))
                {
                    int index = s.IndexOf(":") + 1;
                    pure.Date = s.Substring(index);
                    continue;
                }


            }
            return pure;
        }

    private void signOutBtn_Click(object sender, RoutedEventArgs e)
        {
            imap.LogOut();
            this.Close();
        }
    }

    public class EmailInfo
    {
        public string Date { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
    }
}
