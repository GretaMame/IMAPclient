using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace IMAPclientApp
{
    class Validate
    {
        public Validate()
        {

        }

        public static bool ValidateEmail(string email)
        {
            try
            {
                var emailAddr = new MailAddress(email);
                return email == emailAddr.Address;
            }
            catch
            {
                return false;
            }
        }
    }
}
