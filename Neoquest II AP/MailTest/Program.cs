using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MailTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            int speed = 200;
            int cmod = 400;
            StringBuilder str = new StringBuilder();
            for (int i = 9; i < speed; ++i)
            {
                str.AppendLine("C" + cmod + "=C" + cmod);
                cmod += 5;
            }
            Clipboard.SetText(str.ToString());
        }
    }
}
