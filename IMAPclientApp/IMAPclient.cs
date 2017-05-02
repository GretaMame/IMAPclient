using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.

namespace IMAPclientApp
{
    class IMAPclient:IMAPclientBase
    {
        private static int DEFAULT_MESSAGES_AMOUNT = 20;
        private string email;
        private string password;

        private int messageCount;
        private int currentMessageCount;

        public IMAPclient(string host, int port, string email, string password): base(host, port)
        {
            this.email = email;
            this.password = password;
        }

        public void LogIn()
        {
            if (Connect())
            {
                SendReceive(IMAP_LOGIN_CMD + " " + email + " " + password + EOL);

                SendReceive("LIST " + "\"\"" + " \"*\"" + EOL);
                SelectMailbox(DEFAULT_MAILBOX);



                int number = 4196;

                SendReceive("FETCH " + number + " body[header]\r\n");
                SendReceive("FETCH " + number + " body[text]\r\n");
            } else
            {
                System.Windows.MessageBox.Show("Unable to connect to server");
            }
        }

        public void LogOut()
        {
            SendReceive(IMAP_LOGOUT_CMD+EOL);
            Disconnect();
        }

        public void SelectMailbox(string mailbox)
        {
            ArrayList result = new ArrayList();
            SendReceive(IMAP_SELECT + mailbox + EOL, ref result);
            getMessageCount(result);
        }

        public void fetchMessages(int amount)
        {
            ArrayList messages = new ArrayList();

        }

        private void getMessageCount(ArrayList res)
        {
            string pattern = "\\d EXISTS";
            Regex rg = new Regex(pattern);
            foreach(string s in res)
            {
               if(rg.IsMatch(s))
                {
                    //cia dar reiktu megint parsint, tikrinimo reiktu
                    messageCount = Int32.Parse(Regex.Replace(s, "[^0-9]+", string.Empty));
                    currentMessageCount = messageCount;
                    Debug.WriteLine("Amount of messages: {0}", messageCount);
                    return;
                }
            }
        }
    }
}
