using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using NeoFunctions;

namespace DailyDoer
{
    public partial class DailyList : Form
    {
        Account acc;
        private delegate void DELEGATE();
        private delegate void LOG(string msg);
        private delegate void BUTTON(bool state);
        private delegate void UI(Control c, string value);
        Delegate logDel;
        Delegate button;
        Delegate ui;
        bool pin;
        public DailyList(Account account)
        {
            InitializeComponent();
            acc = account;
            this.FormClosed += new FormClosedEventHandler(DailyList_FormClosed);
            logDel = new LOG(Log);
            button = new BUTTON(ChangeButton);
            ui = new UI(UpdateUI);
            pin = boxPIN.Checked;
            UpdateMoney();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void boxBank_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();  
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke(button, false);
            string data;
            
            if (boxOmelette.Checked)
            {
                data = acc.Get("/prehistoric/omelette.phtml");
                data = acc.Post("/prehistoric/omelette.phtml", "type=get_omelette", "http://www.neopets.com/prehistoric/omelette.phtml");
                this.Invoke(logDel, "Grabbed Omelette");
            }
            if (boxJelly.Checked)
            {
                data = acc.Get("/jelly/jelly.phtml");
                data = acc.Post("/jelly/jelly.phtml", "type=get_jelly", "http://www.neopets.com/jelly/jelly.phtml");
                this.Invoke(logDel, "Collected Jelly");
            }
            if (boxBank.Checked)
            {
                data = acc.Post("/process_bank.phtml", "type=interest", "http://www.neopets.com/bank.phtml");
                acc.Get("/bank.phtml", "http://www.neopets.com/bank.phtml");
                this.Invoke(logDel, "Collected Bank Interest");
            }

            if (boxScratchcard.Checked)
            {
                string logString = "";
                if (radioWinter.Checked)
                {
                    if (GetMoney() < 600)
                        BankWithdraw(600);
                    data = acc.Get("/winter/kiosk.phtml");
                    data = acc.Post("/winter/process_kiosk.phtml", "", "http://www.neopets.com/winter/kiosk.phtml");
                    logString = "Purchased Winter Scratchcard";
                }
                else if (radioHaunted.Checked)
                {
                    if (GetMoney() < 1200)
                        BankWithdraw(1200);
                    data = acc.Get("/halloween/scratch.phtml");
                    data = acc.Post("/halloween/process_scratch.phtml", "", "http://www.neopets.com/halloween/scratch.phtml");
                    logString = "Purchased Spooky Scratchcard";
                }
                else if (radioDesert.Checked)
                {
                    if (GetMoney() < 500)
                        BankWithdraw(500);
                    data = acc.Get("/desert/sc/kiosk.phtml");
                    //ref_ck is currently always 32 char
                    Match refCk = Regex.Match(data, @"name='_ref_ck' value='([a-z0-9]{32})'");
                    data = acc.Post("/desert/sc/kiosk.phtml", "buy=1&_ref_ck=" + refCk.Groups[1].Value, "http://www.neopets.com/desert/sc/kiosk.phtml");
                    logString = "Purchased Desert Scratchcard";
                }

                this.Invoke(logDel, logString);
                UpdateMoney();
            }

            if (boxStocks.Checked)
            {
                if (GetMoney() < 17000)
                    BankWithdraw(17000);
                Match cheapStock;
                data = acc.Get("/stockmarket.phtml?type=buy");

                cheapStock = Regex.Match(data, @"([A-Z]{2,5}) 15 [\+|\-][0-9]{1,3}");
                if (Regex.IsMatch(data, @"([A-Z]{2,5}) 15 [\+|\-][0-9]{1,3}"))
                {
                    cheapStock = Regex.Match(data, @"([A-Z]{2,5}) 15 [\+|\-][0-9]{1,3}");
                }
                else if (Regex.IsMatch(data, @"([A-Z]{2,5}) 16 [\+|\-][0-9]{1,3}"))
                {
                    cheapStock = Regex.Match(data, @"([A-Z]{2,5}) 16 [\+|\-][0-9]{1,3}");
                }
                else
                {
                    cheapStock = Regex.Match(data, @"([A-Z]{2,5}) 17 [\+|\-][0-9]{1,3}");
                }

                Match refCk = Regex.Match(data, @"&_ref_ck=([a-z0-9]{32})");
                string ticker = cheapStock.Groups[1].Value;

                if (ticker != "")
                {
                    data = acc.Post("/process_stockmarket.phtml", "_ref_ck=" + refCk.Groups[1].Value + "&type=buy&ticker_symbol=" + ticker + "&amount_shares=1000", "http://www.neopets.com/stockmarket.phtml?type=buy");
                    this.Invoke(logDel, "Bought 1000 stocks of " + ticker);
                }
                else
                    this.Invoke(logDel, "No cheap stocks, try again later");

                UpdateMoney();
            }

            if (boxBuriedTreasure.Checked)
            {
                data = acc.Get("/pirates/buriedtreasure/index.phtml");
                data = acc.Get("/pirates/buriedtreasure/buriedtreasure.phtml?", "http://www.neopets.com/pirates/buriedtreasure/index.phtml");
                Random r = new Random();
                int x = r.Next(30, 470);
                int y = r.Next(30, 470);
                string get = "/pirates/buriedtreasure/buriedtreasure.phtml?" + x + "," + y;
                data = acc.Get(get, "http://www.neopets.com/pirates/buriedtreasure/buriedtreasure.phtml?");
                //TODO: Check for avatar, log prize
                this.Invoke(logDel, "Did buried treasure, figure out log later");
                UpdateMoney();
            }

            if (boxFaerieCaverns.Checked)
            {
                data = acc.Get("/faerieland/caverns/index.phtml");
                data = acc.Post("/faerieland/caverns/index.phtml", "play=1", "http://www.neopets.com/faerieland/caverns/index.phtml");
                Random rand = new Random();
                int count = 0;
                do
                {
                    if (count > 0)
                        this.Invoke(logDel, "Made it through cave #" + count);
                    string dir;
                    if (rand.Next() % 2 == 0)
                        dir = "&goLeft=Left";
                    else
                        dir = "&goRight=Right";

                    data = acc.Post("/faerieland/caverns/index.phtml", "play=1" + dir, "http://www.neopets.com/faerieland/caverns/index.phtml");
                    count++;
                }
                while (!Regex.IsMatch(data, "value=\"Return to Faerieland\"") && count != 3);
                if (count == 3)
                {
                    this.Invoke(logDel, "Made it through cave #" + count);
                    this.Invoke(logDel, "YOU WON FAERIE CAVERNS TREASURE YAY");
                    data = acc.Post("/faerieland/caverns/index.phtml", "play=1", "http://www.neopets.com/faerieland/caverns/index.phtml");
                }
                else
                    this.Invoke(logDel, "Faerie caverns fucked ya");


            }

            if (boxAnchorManagement.Checked)
            {
                data = acc.Get("/pirates/anchormanagement.phtml");
                Match action = Regex.Match(data, "type=\"hidden\" value=\"([a-z0-9]{32})\"");
                data = acc.Post("/pirates/anchormanagement.phtml", "action=" + action.Groups[1].Value, "http://www.neopets.com/pirates/anchormanagement.phtml");
                this.Invoke(logDel, "Anchored");
                //TODO: Log item
            }

            if (boxApple.Checked)
            {
                data = acc.Get("/halloween/applebobbing.phtml?bobbing=1", "http://www.neopets.com/halloween/applebobbing.phtml?");
                //TODO: check for avatar
                this.Invoke(logDel, "Apple Bobbed");
            }

            if (boxPlushie.Checked)
            {
                data = acc.Post("/faerieland/tdmbgpop.phtml", "talkto=1", "http://www.neopets.com/faerieland/tdmbgpop.phtml");
                //TODO: check for avatar
                this.Invoke(logDel, "Plushie talked");
                UpdateMoney();
            }

            if (boxMystic.Checked)
            {
                data = acc.Get("/island/mystichut.phtml");
                //TODO: check for avatar and log
                this.Invoke(logDel, "Mystified");
            }

            //TODO: Avatar wheels and figure out how they work

            if (boxFoodWinnings.Checked)
            {
                string npWin = @"Total Winnings</b></td><td align='center'><b>([0-9]{1,9}) NP";
                data = acc.Get("/pirates/foodclub.phtml?type=collect");

                if (!Regex.IsMatch(data, "You do not have any winning bets"))
                {
                    string winString = "Collected " + Regex.Match(data, npWin).Groups[1].Value + " NP in food club winnings";
                    data = acc.Post("/pirates/process_foodclub.phtml", "type=collect", "http://www.neopets.com/pirates/foodclub.phtml?type=collect");

                    this.Invoke(logDel, winString);
                    UpdateMoney();
                }
                else
                {
                    this.Invoke(logDel, "No food club winnings to collect :(");
                }
            }

            /*************************************************
            ************* ALWAYS DO THIS LAST ****************
            *************************************************/

            if (boxFoodClub.Checked)
            {
                string noBet = @"You do not have any bets placed for this round!";
                data = acc.Get("/pirates/foodclub.phtml?type=current_bets");
                if (!Regex.IsMatch(data, noBet))
                {
                    Exit(false, "You have already placed Food Club bets this round!");
                    return;
                }

                this.Invoke(logDel, "Checking if bets on page...");

                Regex roundRegex = new Regex(@"<b>[0-9]{4}</b>");
                string roundString = roundRegex.ToString();

                string petpage = "/~boochi_target";
                string bettingPage = acc.Get(petpage);
                int[] betIndex = new int[10];

                MatchCollection mm = Regex.Matches(bettingPage, roundString);
                if (mm.Count < 5)
                {
                    Exit(false, "No food club bets on page");
                    return;
                }

                this.Invoke(logDel, "Checking if bets are up to date...");

                string pirateRegex = @"pirates&id=[1-9][0-9]?'><b>([A-Z][a-z]+'?\s?\-?[A-Z]?[a-z]*\s?\-?[A-Z]?[a-z]*\s?[A-Z]?[a-z]*)";

                data = acc.Get("/pirates/foodclub.phtml?type=current&id=1");
                string[] shipwreckList = Regex.Matches(data, pirateRegex).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

                data = acc.Get("/pirates/foodclub.phtml?type=current&id=2");
                string[] lagoonList = Regex.Matches(data, pirateRegex).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

                data = acc.Get("/pirates/foodclub.phtml?type=current&id=3");
                string[] treasureList = Regex.Matches(data, pirateRegex).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

                data = acc.Get("/pirates/foodclub.phtml?type=current&id=4");
                string[] coveList = Regex.Matches(data, pirateRegex).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

                data = acc.Get("/pirates/foodclub.phtml?type=current&id=5");
                string[] harpoonList = Regex.Matches(data, pirateRegex).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

                data = acc.Get(petpage);
                string shipwreckPirate = @"<b>Shipwreck</b>: ([A-Z][a-z]+'?\s?\-?[A-Z]?[a-z]*\s?\-?[A-Z]?[a-z]*\s?[A-Z]?[a-z]*)";
                string treasurePirate = @"<b>Treasure Island</b>: ([A-Z][a-z]+'?\s?\-?[A-Z]?[a-z]*\s?\-?[A-Z]?[a-z]*\s?[A-Z]?[a-z]*)";
                string hiddenCovePirate = @"<b>Hidden Cove</b>: ([A-Z][a-z]+'?\s?\-?[A-Z]?[a-z]*\s?\-?[A-Z]?[a-z]*\s?[A-Z]?[a-z]*)";
                string lagoonPirate = @"<b>Lagoon</b>: ([A-Z][a-z]+'?\s?\-?[A-Z]?[a-z]*\s?\-?[A-Z]?[a-z]*\s?[A-Z]?[a-z]*)";
                string harpoonPirate = @"<b>Harpoon Harry's</b>: ([A-Z][a-z]+'?\s?\-?[A-Z]?[a-z]*\s?\-?[A-Z]?[a-z]*\s?[A-Z]?[a-z]*)";

                string[] shipwreckBetPirates = Regex.Matches(data, shipwreckPirate).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                string[] treasureBetPirates = Regex.Matches(data, treasurePirate).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                string[] hiddenCoveBetPirates = Regex.Matches(data, hiddenCovePirate).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                string[] lagoonBetPirates = Regex.Matches(data, lagoonPirate).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                string[] harpoonBetPirates = Regex.Matches(data, harpoonPirate).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

                for (int i = 0; i < shipwreckBetPirates.Length; ++i)
                {
                    int count = 0;
                    for (int j = 0; j < shipwreckList.Length; ++j)
                    {
                        if (!shipwreckBetPirates[i].Equals(shipwreckList[j]))
                        {
                            count++;
                        }
                        if (count == 4)
                        {
                            Exit(false);
                            return;
                        }
                    }
                }

                for (int i = 0; i < treasureBetPirates.Length; ++i)
                {
                    int count = 0;
                    for (int j = 0; j < treasureList.Length; ++j)
                    {
                        if (!treasureBetPirates[i].Equals(treasureList[j]))
                        {
                            count++;
                        }
                        if (count == 4)
                        {
                            Exit(false);
                            return;
                        }
                    }
                }

                for (int i = 0; i < hiddenCoveBetPirates.Length; ++i)
                {
                    int count = 0;
                    for (int j = 0; j < coveList.Length; ++j)
                    {
                        if (!hiddenCoveBetPirates[i].Equals(coveList[j]))
                        {
                            count++;
                        }
                        if (count == 4)
                        {
                            Exit(false);
                            return;
                        }
                    }
                }

                for (int i = 0; i < lagoonBetPirates.Length; ++i)
                {
                    int count = 0;
                    for (int j = 0; j < lagoonList.Length; ++j)
                    {
                        if (!lagoonBetPirates[i].Equals(lagoonList[j]))
                        {
                            count++;
                        }
                        if (count == 4)
                        {
                            Exit(false);
                            return;
                        }
                    }
                }

                for (int i = 0; i < harpoonBetPirates.Length; ++i)
                {
                    int count = 0;
                    for (int j = 0; j < harpoonList.Length; ++j)
                    {
                        if (!harpoonBetPirates[i].Equals(harpoonList[j]))
                        {
                            count++;
                        }
                        if (count == 4)
                        {
                            Exit(false);
                            return;
                        }
                    }
                }

                this.Invoke(logDel, "Petpage up to date! Grabbing bets...");

                data = acc.Get("/pirates/foodclub.phtml?type=bet");
                int maxBet = 0;

                Match betMatch = Regex.Match(data, @"max_bet = ([0-9]{2,4})");
                if (betMatch.Success)
                {
                    Int32.TryParse(betMatch.Groups[1].Value, out maxBet);
                }

                Match moneyOnHand = Regex.Match(data, @"\/inventory.phtml\"">([0-9]{1,3}),?([0-9]{0,3}),?([0-9]{0,3}),?([0-9]{0,3})<\/a>");

                int money = GetMoney();
                if (money < maxBet * 10)
                {
                    BankWithdraw(maxBet * 10 - money);
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

                
                for (int i = 0; i < 10; i++)
                {
                    betIndex[i] = mm[i].Index;
                }


                MatchCollection[] pirateIndex = new MatchCollection[20];
                string[] pirates =
                    {   "Scurvy Dan the Blade",
                        "Young Sproggie",
                        "Orvinn the First Mate",
                        "Lucky McKyriggan",
                        "Sir Edmund Ogletree",
                        "Peg Leg Percival",
                        "Bonnie Pip Culliford",
                        "Puffo the Waister",
                        "Stuff-A-Roo",
                        "Squire Venable",
                        "Captain Crossblades",
                        "Ol' Stripey",
                        "Ned the Skipper",
                        "Fairfax the Deckhand",
                        "Gooblah the Grarrl",
                        "Franchisco Corvallio",
                        "Federismo Corvallio",
                        "Admiral Blackbeard",
                        "Buck Cutlass",
                        "The Tailhook Kid" };
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

                Console.WriteLine();
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
                    this.Invoke(logDel, "Placed bet #" + (j + 1) + "!");
                }
                this.Invoke(logDel, "Finished placing bets!");
                UpdateMoney();
            }

            Exit(true);
        }

        void DailyList_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form f = Application.OpenForms["frmLogin"];
            if (f == null)
                Application.Exit();
        }

        void Log(string msg)
        {
            msg = DateTime.Now.ToLongTimeString() + " - " + msg;
            //System.IO.File.AppendAllText(".\\Log - " + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + ".txt", msg + Environment.NewLine);
            textboxLog.Text += msg + "\n";
        }

        void Exit(bool complete, string msg="Food Club bets outdated, bets not placed")
        {
            if (!complete)
            {
                this.Invoke(logDel, msg);
            }
            this.Invoke(logDel, "Completed Dailies!");
            this.Invoke(button, true);
            UpdateMoney();
        }

        void BankWithdraw(int amt)
        {
            string bankPost = "type=withdraw&amount=" + amt;
            if (pin)
                bankPost = "pin=" + txtPIN.Text + "&type=withdraw&amount=" + amt;
            //withdraw from bank
            this.Invoke(logDel, "Not enough NP on hand, withdrawing from bank");
            acc.Get("/bank.phtml");
            acc.Post("/process_bank.phtml", bankPost, "http://www.neopets.com/bank.phtml");
            this.Invoke(logDel, "Withdrew " + amt + " NP.");
            UpdateMoney();
        }

        int GetMoney()
        {
            string data = acc.Get("/inventory.phtml");

            Match moneyOnHand = Regex.Match(data, @"\/inventory.phtml\"">([0-9]{1,3}),?([0-9]{0,3}),?([0-9]{0,3}),?([0-9]{0,3})<\/a>");

            int b, mi, t, o;
            b = mi = t = o = 0;
            if (moneyOnHand.Groups[4].Value != "")
            {
                Int32.TryParse(moneyOnHand.Groups[1].Value, out b);
                Int32.TryParse(moneyOnHand.Groups[2].Value, out mi);
                Int32.TryParse(moneyOnHand.Groups[3].Value, out t);
                Int32.TryParse(moneyOnHand.Groups[4].Value, out o);
            }

            else if (moneyOnHand.Groups[3].Value != "")
            {
                Int32.TryParse(moneyOnHand.Groups[1].Value, out mi);
                Int32.TryParse(moneyOnHand.Groups[2].Value, out t);
                Int32.TryParse(moneyOnHand.Groups[3].Value, out o);
            }
            else if (moneyOnHand.Groups[2].Value != "")
            {
                Int32.TryParse(moneyOnHand.Groups[1].Value, out t);
                Int32.TryParse(moneyOnHand.Groups[2].Value, out o);
            }
            else
            {
                Int32.TryParse(moneyOnHand.Groups[1].Value, out o);
            }

            return b * 1000000000 + mi * 1000000 + t * 1000 + o;
        }

        void UpdateMoney()
        {
            string data = acc.Get("/inventory.phtml");
            Match moneyOnHand = Regex.Match(data, @"\/inventory.phtml"">([0-9]{1,3},?[0-9]{0,3},?[0-9]{0,3},?[0-9]{0,3})<\/a>");
            if (backgroundWorker1.IsBusy)
                this.Invoke(ui, labelNeopoints, "Neopoints on Hand: " + moneyOnHand.Groups[1].Value);
            else
                UpdateUI(labelNeopoints, "Neopoints on Hand: " + moneyOnHand.Groups[1].Value);
        }

        void UpdateUI(Control c, string value)
        {
            c.Text = value;
        }

        void ChangeButton(bool state)
        {
            button1.Enabled = state;
            btnChange.Enabled = state;
        }

        void CheckCost()
        {
            int cost = 0;
            if (boxScratchcard.Checked)
            {
                if (radioWinter.Checked)
                    cost += 600;
                else if (radioHaunted.Checked)
                    cost += 1200;
                else if (radioDesert.Checked)
                    cost += 500;
            }
            if (boxBuriedTreasure.Checked)
            {
                cost += 300;
            }
            if (boxStocks.Checked)
            {
                cost += 17000;
            }
            if (boxFoodClub.Checked)
            {
                string data = acc.Get("/pirates/foodclub.phtml?type=bet");
                int maxBet = 0;

                Match betMatch = Regex.Match(data, @"max_bet = ([0-9]{2,4})");
                if (betMatch.Success)
                {
                    Int32.TryParse(betMatch.Groups[1].Value, out maxBet);
                }
                cost += maxBet * 10;
            }

            int money = GetMoney();
            if (money < cost)
            {
                BankWithdraw(cost - money);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void boxFaerieCaverns_CheckedChanged(object sender, EventArgs e)
        {

        }

        void Delay(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            new frmLogin().Show();
            this.Close();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            bool pin = boxPIN.Checked;
            CheckAll(this);
            boxPIN.Checked = pin;
        }

        void CheckAll(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c.HasChildren)
                    CheckAll(c);
                if (c is CheckBox)
                    ((CheckBox)c).Checked = boxCheckAll.Checked;
            }
        }
    }
}
