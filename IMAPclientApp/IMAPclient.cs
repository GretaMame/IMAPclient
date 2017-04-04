using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMAPclientApp
{
    class IMAPclient:IMAPclientBase
    {
        private string email;
        private string password;

        public IMAPclient(string host, int port, string email, string password): base(host, port)
        {
            this.email = email;
            this.password = password;
        }

        public void LogIn()
        {
            Connect();
            ReceiveResponse("LOGIN " + email + " " + password + "  " + EOL);

            ReceiveResponse("LIST " + "\"\"" + " \"*\"" + "\r\n");

            ReceiveResponse("SELECT INBOX\r\n");

            ReceiveResponse("STATUS INBOX (MESSAGES)\r\n");



            int number = 4196;

            ReceiveResponse("FETCH " + number + " body[header]\r\n");
            ReceiveResponse("FETCH " + number + " body[text]\r\n");
        }

        public void LogOut()
        {
            Disconnect();
        }
    }
}
