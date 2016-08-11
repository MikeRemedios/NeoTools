using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using NeoFunctions;

namespace Neoquest_II_AP
{
    class NQLoop
    {
        const string neoQuest = "/games/nq2/nq2.phtml"; // for get and posts
        const string refer = "http://www.neopets.com/games/nq2/nq2.phtml";
        const string inventory = neoQuest + "?act=inv";
        const string hunting = neoQuest + "?act=travel&mode=2";
        const string normal = neoQuest + "?act=travel&mode=1";
        string data;
        string user = "user";
        string pass = "password";
        string currentRoute = "";
        int stage = -1;
        int offset = 0;
        int[] levels = new int[4];
        bool trainingMode = false;
        bool fromSave = false;
        bool running;
        Account acc;

        public void StartGame()
        {
            running = true;
            Init();
        }
        private void Init()
        {
            WriteCentered("Neoquest II Console APer");
            Console.WriteLine();
            acc = null;

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
            data = acc.Get(neoQuest);

            //if no save file, create save and new game
            string savePath = ".\\saves\\" + user + ".save";
            if (File.Exists(savePath))
            {
                //get stage
                //get levels
                Console.WriteLine("Getting save data");
                string[] lines = File.ReadAllLines(savePath);
                Int32.TryParse(lines[0], out stage);
                for (int i = 0; i < 4; i++)
                {
                    Int32.TryParse(lines[2 + i], out levels[i]);
                }
                Int32.TryParse(lines[6], out offset);

                //hard part is figuring out how to get the route progress perfect
                currentRoute = lines[1];

                Console.WriteLine("Current Stage: " + stage);
                Console.WriteLine("Stage progress: " + (currentRoute.Length) + "/" + TotalLines(".\\scripts\\" + stage + ".txt"));
                Console.WriteLine("Rohane is level " + levels[0]);
                Console.WriteLine("Mipsy is level " + levels[1]);
                Console.WriteLine("Velm is level " + levels[2]);
                Console.WriteLine("Talinia is level " + levels[3]);
                Console.WriteLine("\n\n");
                if (FindString("Next turn:", data))
                {
                    fromSave = true;
                    Console.WriteLine("\nCurrently in battle, finishing that");
                }

            }
            else
            {
                FileStream saveFile = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite);
                saveFile.Close();
                for (int i = 0; i < 4; i++)
                {
                    levels[i] = 1;
                }
                using (StreamWriter w = new StreamWriter(savePath))
                {
                    w.WriteLine(stage);
                    w.WriteLine();
                    for (int i = 0; i < 4; i++)
                    {
                        w.WriteLine(levels[i]);
                    }
                    w.WriteLine(offset);
                }
            }

            GameLoop();
        }

        private void GameLoop()
        {
            while (running)
            {
                if (fromSave)
                {
                    BattleLoop();
                }
                if (stage == -1)
                {
                    data = acc.Get("/games/nq2/nq2.phtml?restart=1", "http://www.neopets.com/games/nq2/nq2.phtml?act=opt");
                    data = acc.Get("/games/nq2/nq2.phtml?startgame=1", "http://www.neopets.com/games/nq2/nq2.phtml?intro_page=3");
                    Console.WriteLine("New game created!");
                    SetStage(100);
                }

                if (stage == 100)
                {
                    trainingMode = true;
                    data = acc.Get("/games/nq2/nq2.phtml?act=travel&mode=2", refer); // hunting mode
                    Move(3, "http://www.neopets.com/games/nq2/nq2.phtml?act=travel&mode=2");
                    Move(3);
                    Move(3);
                    Move(3);

                    Console.WriteLine("Grind to level 3");

                    while (levels[0] < 3)
                    {
                        TrainingMode(3);
                    }
                    SetStage(101);
                    Console.WriteLine("Starting travel to Dark Cave");
                }

                if (stage == 101)
                {
                    MovementScript(101);
                    Console.WriteLine("Arrived at Dark Cave!");
                    SetStage(102);
                }

                //Hit level 5 before entering cave
                if (stage == 102)
                {
                    Console.WriteLine("Grinding to level 5 before entering cave");
                    TrainingMode(5);
                    SetStage(103);
                }

                //Travel to Miner Foreman
                if (stage == 103)
                {
                    MovementScript(103);
                    Console.WriteLine("Defeated Miner Foreman!");
                    Console.ReadLine();
                    Console.WriteLine("Equipping new armour");
                    data = acc.Get(inventory, refer + "?finish=1");
                    data = acc.Get("/games/nq2/nq2.phtml?act=inv&iact=equip&targ_item=20010&targ_char=1", refer + "?act=inv");
                    data = acc.Get(neoQuest, refer + "?act=inv&iact=equip&targ_item=20010&targ_char=1");
                    Console.WriteLine("Exiting Dark Cave");
                    Move(2);
                    Move(2);
                    Move(2);
                    SetStage(104);
                    Environment.Exit(1);
                }
                if (stage == 104)
                {
                    //Travel to White River City
                    MovementScript(104);
                    //heal at inn, set checkpoint
                    Console.WriteLine("Healing at White River City Inn");
                    data = acc.Get("/games/nq2/nq2.phtml?act=talk&targ=10404", refer);
                    data = acc.Get("/games/nq2/nq2.phtml?act=talk&targ=10404&say=rest", refer + "?act=talk&targ=10404");
                    SetStage(105);
                }
                if (stage == 105)
                {
                    MovementScript(105);
                    //move to merchant, buy sword and pots
                    Console.WriteLine("Arrived at merchant!");
                    Console.WriteLine("Buying Sword and Vials");
                    data = acc.Get("/games/nq2/nq2.phtml?act=merch&targ=10401&greet=1", refer);
                    data = acc.Get("/games/nq2/nq2.phtml?act=merch&targ=10401&mact=buy&targ_item=10011&quant=1", refer + "?act=merch&targ=10401&greet=1");
                    bool first = true;
                    do
                    {
                        if (first)
                        {
                            first = false;
                            if (FindString("Buy 10 (150 gp)", data))
                            {
                                data = acc.Get("/games/nq2/nq2.phtml?act=merch&targ=10401&mact=buy&targ_item=30011&quant=10", refer + "?act=merch&targ=10401&mact=buy&targ_item=10011&quant=1");
                            }
                        }
                        else
                        {
                            if (FindString("Buy 10 (150 gp)", data))
                            {
                                data = acc.Get("/games/nq2/nq2.phtml?act=merch&targ=10401&mact=buy&targ_item=30011&quant=10", refer + "?act=merch&targ=10401&mact=buy&targ_item=30011&quant=10");
                            }
                        }

                    } while (FindString("You spent <B>150 gp</B>", data));

                    //equip sword
                    Console.WriteLine("Equipping Sword!");
                    data = acc.Get("/games/nq2/nq2.phtml?act=inv&iact=equip&targ_item=10011&targ_char=1", refer + "?act=inv");
                    data = acc.Get(neoQuest, refer + "?act=inv&iact=equip&targ_item=10011&targ_char=1");
                    SetStage(106);
                }
                if (stage == 106)
                {

                    //Train to level 9 inside cave
                    if (levels[0] < 9)
                    {
                        MovementScript(106);
                        TrainingMode(9);
                        SetStage(107);
                    }

                    //Train to level 10 inside cave
                    else if (levels[0] < 10)
                    {
                        MovementScript(106);
                        TrainingMode(10);
                        SetStage(107);
                    }

                    //finally level 11
                    else if (levels[0] < 11)
                    {
                        MovementScript(106);
                        TrainingMode(11);
                        SetStage(107);
                    }

                }
                if (stage == 107)
                {
                    //back to city, restock vials
                    MovementScript(107, false);
                    Console.WriteLine("Topping up vials");
                    data = acc.Get("/games/nq2/nq2.phtml?act=merch&targ=10401&greet=1", refer);
                    bool first = true;
                    do
                    {
                        if (first)
                        {
                            first = false;
                            if (FindString("Buy 10 (150 gp)", data))
                            {
                                data = acc.Get("/games/nq2/nq2.phtml?act=merch&targ=10401&mact=buy&targ_item=30011&quant=10", refer + "?act=merch&targ=10401&greet=1");
                            }
                        }
                        else
                        {
                            if (FindString("Buy 10 (150 gp)", data))
                            {
                                data = acc.Get("/games/nq2/nq2.phtml?act=merch&targ=10401&mact=buy&targ_item=30011&quant=10", refer + "?act=merch&targ=10401&mact=buy&targ_item=30011&quant=10");
                            }
                        }

                    } while (FindString("You spent <B>150 gp</B>", data));
                    data = acc.Get(neoQuest, refer + "?act=merch&targ=10401&mact=buy&targ_item=30011&quant=10");

                    if (levels[0] < 11)
                        SetStage(106);
                    else
                        SetStage(108);
                }
                if (stage == 108)
                {
                    Console.WriteLine("Off to Zombom!");
                    MovementScript(108, false);
                    Console.WriteLine("Arrived at Zombom!");
                    Move(2);
                    if (CheckBattle())
                    {
                        data = acc.Get(neoQuest + "?start=1", refer);
                        Console.WriteLine("\nEntering battle with Zombom!");
                        BattleLoop();
                    }
                    Console.WriteLine("Defeated Zombom!");
                    //check inv for zombom drop
                    string drop = "";
                    int dropID = 0;
                    data = acc.Get(inventory, refer + "?finish=1");
                    if (FindString("tempered iron shortsword", data))
                    {
                        drop = "tempered iron shortsword";
                        dropID = 10012;
                    }
                    data = acc.Get(neoQuest + "?act=inv&iact=equip&targ_item=" + dropID + "&targ_char=1", refer + "?act=inv");
                    Console.WriteLine("Equipping " + drop);
                    SetStage(109);
                }
                if (stage == 109)
                {
                    MovementScript(109);
                    data = acc.Get(neoQuest + "?act=talk&targ=10408", refer);
                    data = acc.Get(neoQuest + "?act=talk&targ=10408&say=do", refer + "?act=talk&targ=10408");
                    data = acc.Get(neoQuest + "?act=talk&targ=10408&say=join", refer + "?act=talk&targ=10408&say=do");
                    Console.WriteLine("Mipsy added to party!");
                    Int32.TryParse(data.Substring(data.IndexOf("Mipsy</TD><TD width=\"40\" align=\"center\">") + "Mipsy</TD><TD width=\"40\" align=\"center\">".Length, 2), out levels[1]);
                    Console.WriteLine("Mipsy is level " + levels[1]);
                    Console.WriteLine("Adding Mipsy's skills");
                    data = acc.Post(neoQuest, "act=skills&show_char=2&buy_char=2&skopt_9201=0&skopt_9202=" + (levels[1] - 1) + "&skopt_9203=0&skopt_9204=0&skopt_9205=0&skopt_9601=0&skopt_9602=0", "http://www.neopets.com/games/nq2/nq2.phtml?act=skills&buy_char=2");
                    data = acc.Get(neoQuest + "?act=skills&buy_char=2&buy_char=2&confirm=1&skopt_9202=10", refer);
                    data = acc.Get(neoQuest, refer + "?act=skills&buy_char=2&buy_char=2&confirm=1&skopt_9202=10");
                    SetStage(110);
                }
                if (stage == 110)
                {
                    TrainingMode(13);
                    MovementScript(110);
                    SetStage(111);
                }
                if (stage == 111)
                {
                    MovementScript(111);
                    //talk to kijandri
                    data = acc.Get(neoQuest + "?act=talk&targ=10708", refer);
                    data = acc.Get(neoQuest + "?act=talk&targ=10708&say=city", refer + "?act=talk&targ=10708");
                    data = acc.Get(neoQuest + "?act=talk&targ=10708&say=how", refer + "?act=talk&targ=10708&say=city");
                    data = acc.Post(neoQuest, "act=move&dir=5", refer + "?act=talk&targ=10708&say=how");
                    SetStage(112);
                }
                if (stage == 112)
                {
                    MovementScript(112);
                    //talk to potraddo
                    data = acc.Get(neoQuest + "?act=talk&targ=10718", refer);
                    data = acc.Get(neoQuest + "?act=talk&targ=10718&say=city", refer + "?act=talk&targ=10718");
                    data = acc.Get(neoQuest + "?act=talk&targ=10718&say=yes", refer + "?act=talk&targ=10718&say=city");
                    data = acc.Get(neoQuest + "?act=talk&targ=10718&say=about", refer + "?act=talk&targ=10718&say=yes");
                    data = acc.Get(neoQuest + "?act=talk&targ=10718&say=east", refer + "?act=talk&targ=10718&say=about");
                    data = acc.Get(neoQuest + "?act=talk&targ=10718&say=enter", refer + "?act=talk&targ=10718&say=east");
                    SetStage(113);
                }
                if (stage == 113)
                {
                    //Buy/equip weapons and armour
                    MovementScript(1121);
                    data = acc.Get(neoQuest + "?act=merch&targ=10709&greet=1", refer);
                    data = acc.Get(neoQuest + "?act=merch&targ=10709&mact=buy&targ_item=10014&quant=1", refer + "?act=merch&targ=10709&greet=1");
                    data = acc.Get(inventory, refer + "?act=merch&targ=10709&mact=buy&targ_item=10014&quant=1");
                    data = acc.Get(neoQuest + "?act=inv&iact=equip&targ_item=10014&targ_char=1", refer + "?act=inv");
                    data = acc.Get(neoQuest, refer + "?act=inv&iact=equip&targ_item=10014&targ_char=1");
                    Move(4);
                    Move(4);
                    Move(4);
                    data = acc.Get(neoQuest + "?act=merch&targ=10710&greet=1", refer);
                    data = acc.Get(neoQuest + "?act=merch&targ=10710&mact=buy&targ_item=20014&quant=1", refer + "?act=merch&targ=10710&greet=1");
                    data = acc.Get(neoQuest + "?act=merch&targ=10710&mact=buy&targ_item=20113&quant=1", refer + "?act=merch&targ=10710&mact=buy&targ_item=20114&quant=1");
                    data = acc.Get(inventory, refer + "?act=merch&targ=10710&mact=buy&targ_item=20113&quant=1");
                    data = acc.Get(neoQuest + "?act=inv&iact=equip&targ_item=20014&targ_char=1", refer + "?act=inv");
                    data = acc.Get(neoQuest + "?act=inv&iact=equip&targ_item=20013&targ_char=2", refer + "?act=inv&iact=equip&targ_item=20014&targ_char=1");
                    data = acc.Get(neoQuest, refer + "?act=inv&iact=equip&targ_item=20013&targ_char=2");
                    Console.WriteLine("If we have everything equipped in lakeside holy shit I'm a god");
                    Console.ReadLine();
                }
            }
        }

        private void BattleLoop()
        {
            bool battling = true;
            bool first = true;
            int numEnemies = -1;
            int enemiesKilled = 0;
            int stuck = 0;

            string referer = "http://www.neopets.com/games/nq2/nq2.phtml?start=1";
            while (battling)
            {
                bool myTurn = FindString("<TD align=\"center\" valign=\"top\"><B>Next turn:</B> <FONT color=\"red\">now</FONT>", data);
                if (fromSave)
                {
                    numEnemies = 0;
                    if (FindString("settarget(5)", data))
                        numEnemies++;
                    if (FindString("settarget(6)", data))
                        numEnemies++;
                    if (FindString("settarget(7)", data))
                        numEnemies++;
                    if (FindString("settarget(8)", data))
                        numEnemies++;
                    first = false;
                    if (numEnemies > 0)
                        fromSave = false;
                    else
                    {
                        int i = 0;
                        if (FindString("is defeated!", data))
                        {
                            battling = false;
                            data = acc.Post(neoQuest, "target=-1&fact=2&parm=&use_id=-1&nxactor=1", refer);
                            if (FindString("<B>Rohane has gained a level!</B>", data))
                            {
                                levels[0]++;
                                data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                                Console.WriteLine("Ending battle!");
                                Skill(levels[0], 0);
                                break;
                            }
                            if (FindString("<B>Mipsy has gained a level!</B>", data))
                            {
                                levels[1]++;
                                data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                                Console.WriteLine("Ending battle!");
                                Skill(levels[1], 1);
                                break;
                            }
                            data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                            Console.WriteLine("Ending battle!");
                        }
                        while (!myTurn)
                        {
                            data = acc.Post(neoQuest, "target=-1&fact=1&parm=&use_id=-1&nxactor=" + (5 + i), referer);
                            i++;
                            Console.WriteLine("Waiting!");
                            myTurn = FindString("<TD align=\"center\" valign=\"top\"><B>Next turn:</B> <FONT color=\"red\">now</FONT>", data);
                        }
                    }
                }
                if (first)
                {
                    first = false;
                }
                else
                {
                    referer = "http://www.neopets.com/games/nq2/nq2.phtml";
                }
                // check if dead
                if (FindString("Rohane is defeated!", data))
                {
                    battling = false;
                    Console.WriteLine("You died, shitty\n");
                    data = acc.Post(neoQuest, "target=-1&fact=2&parm=&use_id=-1&nxactor=5", referer);
                    data = acc.Post(neoQuest, "finish=1", referer);
                    if (stage == 100)
                    {
                        Console.WriteLine("Moving back outside from mom");
                        Move(3);
                        Move(3);
                        Move(3);
                        Move(3);
                        offset = 0;
                    }
                    else if (stage == 102)
                    {
                        Move(3);
                        Move(3);
                        Move(3);
                        Move(3);
                        offset = 0;
                        SetStage(101);
                    }
                    else if (stage > 105 && stage <= 108)
                    {
                        Console.WriteLine("Revived at White River, back to Zombom");
                        for (int i = 0; i < 10; i++)
                        {
                            Move(3);
                        }
                        offset = 0;
                    }
                }
                else if (FindString("is defeated!", data))
                {
                    numEnemies--;
                    enemiesKilled++;
                    Console.WriteLine("\nNumber of enemies left: " + numEnemies);
                    if (numEnemies == 0)
                    {
                        //Battle completed
                        battling = false;
                        data = acc.Post(neoQuest, "target=-1&fact=2&parm=&use_id=-1&nxactor=1", referer);
                        if (FindString("<B>Rohane has gained a level!</B>", data))
                        {
                            levels[0]++;
                            data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                            Console.WriteLine("Ending battle!");
                            Skill(levels[0], 0);
                            break;
                        }
                        if (FindString("<B>Mipsy has gained a level!</B>", data))
                        {
                            levels[1]++;
                            data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                            Console.WriteLine("Ending battle!");
                            Skill(levels[1], 1);
                            break;
                        }
                        data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                        Console.WriteLine("Ending battle!");
                    }
                }
                if (myTurn)
                {
                    //If started on a wait, number of enemies unknown
                    if (numEnemies == -1)
                    {
                        if (FindString("settarget(5)", data))
                            numEnemies = 1;
                        if (FindString("settarget(6)", data))
                            numEnemies++;
                        if (FindString("settarget(7)", data))
                            numEnemies++;
                        if (FindString("settarget(8)", data))
                            numEnemies++;
                        Console.WriteLine("\nNumber of enemies: " + numEnemies + "\n");
                    }

                    //Check if heal necessary
                    if (FindString("<B>Rohane</B></FONT><BR><TABLE><TR><TD><FONT color=\"#d0d000\"", data) || FindString("<B>Rohane</B></FONT><BR><TABLE><TR><TD><FONT color=\"red\">", data))
                    {
                        string healingItem = "";
                        int healingID = 30011;
                        if (stage < 108)
                        {
                            healingItem = "Healing Vial</A>";
                            healingID = 30011;
                        }
                        else if (stage >= 108)
                        {
                            if (!FindString("Healing Vial</A>", data))
                            {
                                healingItem = "Healing Flask</A>";
                                healingID = 30012;
                            }
                            if (!FindString("Healing Flask</A>", data))
                            {
                                healingItem = "Healing Potion</A>";
                                healingID = 30013;
                            }
                        }
                        if (FindString(healingItem, data))
                        {
                            Console.WriteLine("Healing Rohane!");
                            data = acc.Post(neoQuest, "target=-1&fact=5&parm=&use_id=" + healingID + "&nxactor=1", referer);
                        }
                        else
                        {
                            if (FindString("settarget(5);", data))
                            {
                                data = acc.Post(neoQuest, "target=5&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 1");
                            }
                            else if (FindString("settarget(6);", data))
                            {
                                data = acc.Post(neoQuest, "target=6&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 2");
                            }
                            else if (FindString("settarget(7);", data))
                            {
                                data = acc.Post(neoQuest, "target=7&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 3");
                            }
                            else if (FindString("settarget(8);", data))
                            {
                                data = acc.Post(neoQuest, "target=8&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 4");
                            }
                            else
                            {
                                data = acc.Post(neoQuest, "target=-1&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking");
                            }
                        }
                    }
                    else if (FindString("<B>Mipsy</B></FONT><BR><TABLE><TR><TD><FONT color=\"#d0d000\"", data) || FindString("<B>Mipsy</B></FONT><BR><TABLE><TR><TD><FONT color=\"red\">", data))
                    {
                        string healingItem = "";
                        int healingID = 30011;
                        if (stage < 108)
                        {
                            healingItem = "Healing Vial</A>";
                            healingID = 30011;
                        }
                        else if (stage >= 108)
                        {
                            if (!FindString("Healing Vial</A>", data))
                            {
                                healingItem = "Healing Flask</A>";
                                healingID = 30012;
                            }
                            if (!FindString("Healing Flask</A>", data))
                            {
                                healingItem = "Healing Potion</A>";
                                healingID = 30013;
                            }
                        }
                        if (FindString(healingItem, data))
                        {
                            Console.WriteLine("Healing Mipsy!");
                            data = acc.Post(neoQuest, "target=-1&fact=5&parm=&use_id=" + healingID + "&nxactor=2", referer);
                        }
                        else
                        {
                            Console.WriteLine("Group Direct Damage POW!");
                            data = acc.Post(neoQuest, "target=-1&fact=9202&parm=&use_id=-1&nxactor=2", referer);
                        }
                    }
                    else
                    {
                        // if Mipsy's turn
                        if (FindString("</TD><TD align=\"center\" valign=\"top\"><B>Next turn:</B> <FONT color=\"red\">now</FONT>", data))
                        {
                            //group direct damage
                            Console.WriteLine("Group direct damage POW");
                            data = acc.Post(neoQuest, "target=-1&fact=9202&parm=&use_id=-1&nxactor=2", referer);
                        }
                        else if (FindString("<TD align=\"center\" valign=\"top\"><B>Next turn:</B> <FONT color=\"red\">now</FONT>", data))
                        {
                            if (FindString("settarget(5);", data))
                            {
                                data = acc.Post(neoQuest, "target=5&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 1");
                            }
                            else if (FindString("settarget(6);", data))
                            {
                                data = acc.Post(neoQuest, "target=6&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 2");
                            }
                            else if (FindString("settarget(7);", data))
                            {
                                data = acc.Post(neoQuest, "target=7&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 3");
                            }
                            else if (FindString("settarget(8);", data))
                            {
                                data = acc.Post(neoQuest, "target=8&fact=3&parm=&use_id=-1&nxactor=1", referer);
                                Console.WriteLine("Attacking enemy 4");
                            }
                            else
                            {
                                if (FindString("<FONT color=\"red\">0", data))
                                {
                                    battling = false;
                                    data = acc.Post(neoQuest, "target=-1&fact=2&parm=&use_id=-1&nxactor=1", referer);
                                    if (FindString("<B>Rohane has gained a level!</B>", data))
                                    {
                                        levels[0]++;
                                        data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                                        Console.WriteLine("Ending battle!");
                                        Skill(levels[0], 0);
                                        break;
                                    }
                                    if (FindString("<B>Mipsy has gained a level!</B>", data))
                                    {
                                        levels[1]++;
                                        data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                                        Console.WriteLine("Ending battle!");
                                        Skill(levels[1], 1);
                                        break;
                                    }
                                    data = acc.Get("/games/nq2/nq2.phtml?finish=1", referer);
                                    Console.WriteLine("Ending battle!");
                                }
                            }
                        }
                    }
                    myTurn = false;
                    stuck = 0;
                }
                else
                {
                    while (numEnemies < 0)
                    {
                        for (int i = 0; i < 4; i++)
                            data = acc.Post(neoQuest, "target=-1&fact=1&parm=&use_id=-1&nxactor=" + (5 + i), referer);
                        Console.WriteLine("Waiting!");
                        break;
                    }

                    if (numEnemies > 0)
                    {
                        for (int i = 0; i < numEnemies; i++)
                        {
                            data = acc.Post(neoQuest, "target=-1&fact=1&parm=&use_id=-1&nxactor=" + (5 + i + enemiesKilled), referer);
                            Console.WriteLine("Waiting!");
                            stuck++;
                        }
                    }
                    if (stuck > 5)
                    {
                        data = acc.Get(neoQuest);
                        Console.WriteLine("Please");
                        stuck = 0;
                        System.Environment.Exit(1);
                    }
                }
            }
            Console.WriteLine("\nRohane's current level: " + levels[0]);
            Console.WriteLine("Mipsy's current level: " + levels[1]);

        }

        private void TrainingMode(int level)
        {
            Console.WriteLine("Grinding to level " + level + " before progressing");
            trainingMode = true;
            while (levels[0] < level)
            {
                if (offset == 0)
                {
                    offset++;
                    Move(3);
                }
                else
                {
                    offset++;
                    offset %= 2;
                    Move(4);
                }
                if (CheckBattle())
                {
                    data = acc.Get(neoQuest + "?start=1", refer);
                    Console.WriteLine("\nEntering battle!");
                    BattleLoop();
                }
            }
            Console.WriteLine("End of loop, fixing offset if needed");
            if (offset == 1)
            {
                offset = 0;
                Move(4);
            }
            trainingMode = false;
        }

        private void SetStage(int value)
        {
            stage = value;
            UpdateSave(666);
        }

        private void MovementScript(int currentStage, bool hunt = true)
        {
            string referer = "";
            if (hunt)
            {
                data = acc.Get(hunting, refer);
                referer = "http://www.neopets.com/games/nq2/nq2.phtml?act=travel&mode=2";
            }
            else
            {
                data = acc.Get(normal, refer);
                referer = "http://www.neopets.com/games/nq2/nq2.phtml?act=travel&mode=1";
            }

            int routeLength = TotalLines(".\\scripts\\" + currentStage + ".txt");
            using (StreamReader sr = new StreamReader(".\\scripts\\" + currentStage + ".txt"))
            {
                int progress = currentRoute.Length;
                string line;
                int dir = 0;
                int count = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    Int32.TryParse(line, out dir);
                    count++;
                    if (count > progress)
                    {
                        Move(dir, referer);
                        progress++;
                        Console.WriteLine("Current Progress: " + progress + "/" + routeLength);
                    }

                    if (CheckBattle())
                    {
                        data = acc.Get(neoQuest + "?start=1", refer);
                        Console.WriteLine("\nEntering battle!");
                        BattleLoop();
                    }
                    if (referer != refer)
                        referer = refer;
                }
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

        private void Move(int dir, string referer = refer)
        {
            data = acc.Post(neoQuest, "act=move&dir=" + dir, referer);
            UpdateSave(dir);
        }

        private void UpdateSave(int direction)
        {
            string[] lines = File.ReadAllLines(".\\saves\\" + user + ".save");
            //if new stage, reset movement progress string
            if (lines[0] != stage.ToString())
            {
                lines[1] = "";
            }

            //line 1 stage
            lines[0] = stage.ToString();

            if (direction != 666 && !trainingMode)
            {
                //line 2 movement
                lines[1] += direction;
            }

            for (int i = 0; i < 4; i++)
            {
                //lines 3-6 levels
                lines[2 + i] = levels[i].ToString();
            }

            lines[6] = offset.ToString();
            File.WriteAllLines(".\\saves\\" + user + ".save", lines);
        }

        int TotalLines(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }

        void ResetRouteProgress()
        {
            string[] lines = File.ReadAllLines(".\\saves\\" + user + ".save");
            lines[1] = "";
            File.WriteAllLines(".\\saves\\" + user + ".save", lines);
        }

        private static bool FindString(string search, string data)
        {
            return (data.IndexOf(search) != -1);
        }

        private bool CheckBattle()
        {
            return FindString("You are attacked by", data);
        }

        private int Skill(int level, int player)
        {
            StreamReader sr;
            string skillString = "";
            string line;
            int skill = 0;
            int character = player + 1;

            if (player == 0)
            {
                sr = new StreamReader(".\\Rohane.skills");
                int counter = 2;
                while ((line = sr.ReadLine()) != null && counter != level)
                {
                    counter++;
                }
                Int32.TryParse(line, out skill);
            }
            if (player == 1)
            {
                sr = new StreamReader(".\\Mipsy.skills");
                int counter = 2;
                while ((line = sr.ReadLine()) != null && counter != level)
                {
                    counter++;
                }
                Int32.TryParse(line, out skill);
            }

            data = acc.Get("/games/nq2/nq2.phtml?act=skills&show_char=" + character, refer);
            data = acc.Get("/games/nq2/nq2.phtml?act=skills&show_char=" + character + "&buy_char=" + character + "&skopt_" + skill + "=1", refer + "?act=skills&show_char=1");
            data = acc.Get("/games/nq2/nq2.phtml?act=skills&buy_char=" + character + "&buy_char=" + character + "&confirm=1&skopt_" + skill + "=1", refer + "?act=skills&show_char=1&buy_char=1&skopt_" + skill + "=1");
            data = acc.Get(neoQuest, refer + "?act=skills&buy_char=" + character + "&buy_char=" + character + "&confirm=1&skopt_" + skill + "=1");
            switch (skill)
            {
                case 9101:
                    skillString = "Added point to Rohane's skill: Critical Attacks";
                    break;
                case 9102:
                    skillString = "Added point to Rohane's skill: Damage Increase";
                    break;
                case 9103:
                    skillString = "Added point to Rohane's skill: Combat Focus";
                    break;
                case 9104:
                    skillString = "Added point to Rohane's skill: Stunning Strikes";
                    break;
                case 9105:
                    skillString = "Added point to Rohane's skill: Battle Taunt";
                    break;
                case 9501:
                    skillString = "Added point to Rohane's skill: Innage Magic Resistance";
                    break;
                case 9502:
                    skillString = "Added point to Rohane's skill: Innate Magic Haste";
                    break;
                case 9201:
                    skillString = "Added point to Mipsy's skill: Direct Damage";
                    break;
                case 9202:
                    skillString = "Added point to Mipsy's skill: Group Direct Damage";
                    break;
                case 9203:
                    skillString = "Added point to Mipsy's skill: Group Haste";
                    break;
                case 9204:
                    skillString = "Added point to Mipsy's skill: Slowing";
                    break;
                case 9205:
                    skillString = "Added point to Mipsy's skill: Damage Shields";
                    break;
                case 9601:
                    skillString = "Added point to Mipsy's skill: Innage Magic Resistance";
                    break;
                case 9602:
                    skillString = "Added point to Mipsy's skill: Innate Magic Haste";
                    break;
            }
            Console.WriteLine(skillString);
            return skill;
        }
    }
}