using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoFunctions;
using System.IO;
using System.Text.RegularExpressions;

namespace MysteryIslandTrainer
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("Console Mystery Island AutoTrainer");
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
            Log(acc.Username);

            data = acc.Get("/altador/colosseum/2015/prizes.phtml", "http://www.neopets.com/");

            
            Regex pointRegex = new Regex(@"Prize Points Left: <b>([0-9]{0,2}),?([0-9]{0,3})");
            Regex totalRegex = new Regex(@"Earned: <b>([0-9]{0,2}),?([0-9]{0,3})");
            string ps = pointRegex.ToString();
            string ss = totalRegex.ToString();
            Match mm = Regex.Match(data, ss);
            Match m = Regex.Match(data, ps);
            int thousand = 0;
            int one = 0;
            int total = 0;
            int.TryParse(mm.Groups[1].Value, out thousand);
            int.TryParse(mm.Groups[2].Value, out one);
            total = thousand * 1000 + one - 45;
            int.TryParse(m.Groups[1].Value, out thousand);
            int.TryParse(m.Groups[2].Value, out one);
            int points = thousand * 1000 + one;
            Console.WriteLine("Buying {0} gyros", total);
            int count = total - points + 1;
            int lastpoints = points;
            int sessionCount = 1;
            Random rand = new Random();
            while(points > 0)
            {
                
                Log(points + " points left");
                Log("Buying Gyro #" + count);
                data = acc.Post("/altador/colosseum/2015/prizes.phtml", "purchase=60965&_ref_ck=e79c6245fc90ff41900c47467b365b66", "http://www.neopets.com/altador/colosseum/2015/prizes.phtml");

                m = Regex.Match(data, ps);
                int.TryParse(m.Groups[1].Value, out thousand);
                int.TryParse(m.Groups[2].Value, out one);

                points = thousand * 1000 + one;
                if (points == 0)
                {
                    data = acc.Get("/altador/colosseum/2015/prizes.phtml");
                    m = Regex.Match(data, ps);
                    int.TryParse(m.Groups[1].Value, out thousand);
                    int.TryParse(m.Groups[2].Value, out one);
                    points = thousand * 1000 + one;
                }
                System.Threading.Thread.Sleep(rand.Next(1200, 1700));
                Console.WriteLine();
                if (count % 200 == 0)
                {
                    Console.Clear();
                }
                count = total - points + 1;
                sessionCount++;
            }
        }

        //UTILITY FUNCTIONS
        /// <summary>
        /// Logs messages to log.txt
        /// </summary>
        /// <param name="msg">Message to log</param>
        private static void Log(string msg)
        {
            msg = DateTime.Now.ToLongTimeString() + " - " + msg;
            System.IO.File.AppendAllText(".\\log.txt", msg + Environment.NewLine);
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
