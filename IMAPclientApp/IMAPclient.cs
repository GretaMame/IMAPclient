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
    public class IMAPclient:IMAPclientBase
    {
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

                for(int i=0;i<10;i++)
                {
                    FetchMessage();
                }
                //int number = 4196;

                //SendReceive("FETCH " + number + " body[header]\r\n");
                //SendReceive("FETCH " + number + " body[text]\r\n");
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

        public ArrayList FetchMessage()
        {
            ArrayList messageInfo = new ArrayList();
            
            {
                Debug.WriteLine("{0} message", currentMessageCount);
                messageInfo = getMessageBasicInfo(currentMessageCount--);

               // reikia isspausdint WPF'e
                foreach (string s in messageInfo)
                {
                    Debug.WriteLine("INFO: " + s);
                }
                return messageInfo;

            }
        }

        private ArrayList getMessageBasicInfo(int uid)
        {
            ArrayList messageInfo = new ArrayList();
            SendReceive(IMAP_FETCH + uid + " (FLAGS BODY[HEADER.FIELDS (SUBJECT DATE FROM)])"+EOL, ref messageInfo);
            return messageInfo;    
        }

        private void getMessageCount(ArrayList res)
        {
            string pattern = "\\d EXISTS";
            Regex rg = new Regex(pattern);
            foreach(string s in res)
            {
               if(rg.IsMatch(s))
                {
                    messageCount = Int32.Parse(Regex.Replace(s, "[^0-9]+", string.Empty));
                    currentMessageCount = messageCount;
                    Debug.WriteLine("Amount of messages: {0}", messageCount);
                    return;
                }
            }
        }
    }
}
