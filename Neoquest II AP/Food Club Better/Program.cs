using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using NeoFunctions;

namespace Food_Club_Better
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteCentered("Food Club Auto Better by me");
            Console.WriteLine();
            Account acc = null;
            string user = "user";
            string pass = "pass";

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
                Console.WriteLine("Fixing Account.txt");
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

            Console.WriteLine("Logged in as " + user);
            Console.WriteLine();

            //list all competitors for each arena
            //Shipwreck = 1, Lagoon = 2, Treasure Island = 3, Hidden Cove = 4, Harpoon Harry's = 5
            //find their odds
            //grab each bet from page
            //search for round regex to divide into rows
            //<b>5868</b>

            string data = acc.Get("/pirates/foodclub.phtml?type=bet");
            int maxBet = 0;

            Match betMatch = Regex.Match(data, @"max_bet = ([0-9]{2,4})");
            if (betMatch.Success)
            {
                Int32.TryParse(betMatch.Groups[1].Value, out maxBet);
            }

            //create list of pirates for each arena
            int index1 = data.IndexOf("<input type='checkbox' name='matches[]' value='1'");
            int index2 = data.IndexOf("<input type='checkbox' name='matches[]' value='2'");
            int index3 = data.IndexOf("<input type='checkbox' name='matches[]' value='3'");
            int index4 = data.IndexOf("<input type='checkbox' name='matches[]' value='4'");
            int index5 = data.IndexOf("<input type='checkbox' name='matches[]' value='5'");
            List<int> shipwreck = new List<int>(4);
            List<int> lagoon = new List<int>(4);
            List<int> treasureIsland = new List<int>(4);
            List<int> hiddenCove = new List<int>(4);
            List<int> harpoonHarry = new List<int>(4);
            int[] odds = new int[20];
            int totalOdds = 1;
            int winnings = 0;

            for (int i = 1; i <= 20; i++)
            {
                int pirate = data.IndexOf("pirate_odds[" + i + "]");
                if (pirate < index1)
                    shipwreck.Add(i);
                else if (pirate < index2)
                    lagoon.Add(i);
                else if (pirate < index3)
                    treasureIsland.Add(i);
                else if (pirate < index4)
                    hiddenCove.Add(i);
                else
                    harpoonHarry.Add(i);

                //get their odds
                int length = 2;
                while (!Int32.TryParse(data.Substring(data.IndexOf("pirate_odds[" + i + "]\t= ") + ("pirate_odds[" + i + "]\t= ").Length, length), out odds[i - 1]))
                {
                    length--;
                }
            }

            Regex roundRegex = new Regex(@"<b>[0-9]{4}</b>");
            string roundString = roundRegex.ToString();
            string petpage;
            if (File.ReadAllLines(".\\petpage.txt")[0] != null)
                petpage = File.ReadAllLines(".\\petpage.txt")[0];
            else
                petpage = "/~boochi_target";
            string bettingPage = acc.Get(petpage);
            int[] betIndex = new int[10];

            Match m = Regex.Match(bettingPage, roundString);
            MatchCollection mm = Regex.Matches(bettingPage, roundString);
            for (int i = 0; i < 10; i++)
            {
                betIndex[i] = mm[i].Index;
            }

            MatchCollection[] pirateIndex = new MatchCollection[20];
            string[] pirates = File.ReadAllLines(".\\pirates.list");
            for (int i = 0; i < 20; i++)
            {
                pirateIndex[i] = Regex.Matches(bettingPage, pirates[i]);
            }

            List<List<int>> bets = new List<List<int>>(10);
            for (int i = 0; i < 10; i++)
            {
                bets.Add(new List<int>(5));
            }
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < pirateIndex[i].Count; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        if (pirateIndex[i][j].Index < betIndex[k + 1])
                        {
                            bets[k].Add(i + 1);
                            break;
                        }
                        else if (pirateIndex[i][j].Index > betIndex[9])
                        {
                            bets[9].Add(i + 1);
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Bet {0}: ", (i + 1));
                bool first = true;
                for (int j = 0; j < bets[i].Count; j++)
                {
                    if (!first)
                    {
                        Console.Write(", ");
                    }
                    if (bets[i][j] != 0)
                    {
                        Console.Write(pirates[bets[i][j] - 1]);
                        first = false;
                    }
                }
                Console.WriteLine();
            }

            //calculate odds and winnings
            //place bets

            for (int j = 0; j < 10; j++)
            {
                string win1 = "winner1=";
                string win2 = "winner2=";
                string win3 = "winner3=";
                string win4 = "winner4=";
                string win5 = "winner5=";
                totalOdds = 1;
                for (int k = 0; k < bets[j].Count; k++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (shipwreck[i] == bets[j][k])
                        {
                            win1 = "matches%5B%5D=1&winner1=" + bets[j][k];
                            totalOdds *= odds[bets[j][k] - 1];
                        }
                        else if (lagoon[i] == bets[j][k])
                        {
                            win2 = "matches%5B%5D=2&winner2=" + bets[j][k];
                            totalOdds *= odds[bets[j][k] - 1];
                        }
                        else if (treasureIsland[i] == bets[j][k])
                        {
                            win3 = "matches%5B%5D=3&winner3=" + bets[j][k];
                            totalOdds *= odds[bets[j][k] - 1];
                        }
                        else if (hiddenCove[i] == bets[j][k])
                        {
                            win4 = "matches%5B%5D=4&winner4=" + bets[j][k];
                            totalOdds *= odds[bets[j][k] - 1];
                        }
                        else if (harpoonHarry[i] == bets[j][k])
                        {
                            win5 = "matches%5B%5D=5&winner5=" + bets[j][k];
                            totalOdds *= odds[bets[j][k] - 1];
                        }
                    }
                    winnings = totalOdds * maxBet;
                }
                string postString = win1 + "&" + win2 + "&" + win3 + "&" + win4 + "&" + win5 + "&bet_amount=" + maxBet + "&total_odds=" + totalOdds + "%3A1&winnings=" + winnings + "&type=bet";
                acc.Post("/pirates/process_foodclub.phtml", postString, "http://www.neopets.com/pirates/foodclub.phtml?type=bet");
                acc.Get("/pirates/foodclub.phtml?type=current_bets", "http://www.neopets.com/pirates/process_foodclub.phtml");
                acc.Get("/pirates/foodclub.phtml?type=bet", "http://www.neopets.com/pirates/foodclub.phtml?type=current_bets");
                Console.WriteLine("Placed bet #{0}!", j + 1);
            }
            Console.WriteLine("Finished placing bets!\nPress enter to exit");
            Console.ReadLine();
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

        private static bool FindString(string search, string data)
        {
            return (data.IndexOf(search) != -1);
        }
    }
}
