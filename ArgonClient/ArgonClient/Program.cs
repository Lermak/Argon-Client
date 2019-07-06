using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;

class EmployeeTCPClient
{
    class player
    {
        public int xLoc;
        public int yLoc;

        public string username;

        public player(int xLoc, int yLoc, string username)
        {
            this.xLoc = xLoc;
            this.yLoc = yLoc;
            this.username = username;
        }

        public player(string input)
        {
            char[] delim = {' '};
            string[] parts = input.Split(delim);

            xLoc = int.Parse(parts[0]);
            yLoc = int.Parse(parts[1]);
            username = parts[2];
        }

        public override string ToString()
        {
            return xLoc.ToString() + " " + yLoc.ToString() + " " + username + "\n";
        }
    }
    public static void Main(string[] args)
    {
        TcpClient client = new TcpClient("localhost", 2055);
        try
        {
            Stream s = client.GetStream();
            StreamReader sr = new StreamReader(s);
            StreamWriter sw = new StreamWriter(s);
            sw.AutoFlush = true;
            Console.WriteLine(sr.ReadLine().Decrypt());
            while (true)
            {
                mainMenu(sw, sr);

            }
            s.Close();
        }
        finally
        {
            // code in finally block is guranteed 
            // to execute irrespective of 
            // whether any exception occurs or does 
            // not occur in the try block
            client.Close();
        }
    }

    private static void AttemptLogin(StreamReader sr, StreamWriter sw)
    {
        string input;
        Console.WriteLine("Enter Username: ");
        input = Console.ReadLine();
        sw.WriteLine(input.Crypt());
        Console.WriteLine("Enter password: ");
        input = Console.ReadLine();
        sw.WriteLine(input.Crypt());
        
        string msg = sr.ReadLine().Decrypt();
        

        if (msg == "Login Success")
        {
            Console.WriteLine("Success");
        }
        else
            Console.WriteLine("Failiure");

        System.Threading.Thread.Sleep(500);
    }

    private static void mainMenu(StreamWriter sw, StreamReader sr)
    {
        System.Threading.Thread.Sleep(1000);
        Console.Clear();
        Console.WriteLine("1. Login \n2. Create Account\n3. Close");
        Console.WriteLine("4. Play\n5. Logout");

        string input = Console.ReadLine();
        sw.WriteLine(input.Crypt());

        if (input == "1")
        {
            Console.Clear();
            AttemptLogin(sr, sw);
        }
        else if (input == "2")
        {
            Console.Clear();
            AttemptCreate(sr, sw);
        }
        else if (input == "3")
        {
            Environment.Exit(1);
        }
        else if (input == "4")
        {
            string msg = sr.ReadLine().Decrypt();
            if (msg == "Playing")
                playGame(sw, sr);
            else
                Console.WriteLine(msg);
        }
        else if (input == "5")
        {
            Console.WriteLine(sr.ReadLine().Decrypt());
        }
    }

    private static void drawMap(StreamWriter sw, StreamReader sr)
    {
        int mapRadius = 10;
        char[] row = new char[mapRadius*4+1];
        sw.WriteLine("GetActive".Crypt());
        //first line always returns the current player
        string ply = sr.ReadLine().Decrypt();
        player myPlayer;
        try
        { 
            myPlayer = new player(ply);
        }
        catch
        {
            Console.WriteLine("Error Retrieveing Character Information");
            return;
        }
        List<player> activePlayers = new List<player>();

        //if there are additional players continue looking through until all are found
        while (sr.Peek() >= 0)
        {
            string msg = sr.ReadLine().Decrypt();
            activePlayers.Add(new player(msg));
        }



        for (int y = myPlayer.yLoc - mapRadius; y < myPlayer.yLoc + mapRadius; ++y)
        {
            int rowCount = 0;
            row = new char[mapRadius*4+1];
            for (int x = myPlayer.xLoc - (mapRadius * 2) - 1; x < myPlayer.xLoc + mapRadius * 2; ++x)
            {
                foreach(player p in activePlayers)
                {
                    if (p.xLoc == x && p.yLoc == y)
                    {
                        row[rowCount] = 'P';
                    }
                }
                if (row[rowCount] != 'P')
                {
                    if (x == myPlayer.xLoc - mapRadius * 2 - 1)
                        row[rowCount] = ' ';
                    else if (x % 3 == 0)
                        row[rowCount] = (char)166;
                    else if (y % 2 == 0)
                        row[rowCount] = (char)152;
                    else
                        row[rowCount] = (char)95;
                }
                if (myPlayer.xLoc == x && myPlayer.yLoc == y)
                    row[rowCount] = 'A';
                rowCount++;
            }
            Console.WriteLine(row);
        }
    }

    private static void controlPlayer(StreamWriter sw, StreamReader sr, char input)
    {
            if(input == 'w' ||
                input == 'a' ||
                input == 's' ||
                input == 'd' ||
                input == 'q')
                sw.WriteLine(input.ToString().Crypt());                
    }

    private static void playGame(StreamWriter sw, StreamReader sr)
    {
        int playLoops = 0;
        ConsoleKeyInfo input;
        while (true)
        {
            
            if (Console.KeyAvailable)
            {
                input = Console.ReadKey(true);
            }
            else
                input = new ConsoleKeyInfo();

            Console.SetCursorPosition(0, 0);
            controlPlayer(sw, sr, input.KeyChar);         
            if (input.KeyChar == 'q')
                break;
            playLoops++;
            drawMap(sw, sr);
            

        }

        Console.WriteLine(playLoops.ToString());
    }

    private static void AttemptCreate(StreamReader sr, StreamWriter sw)
    {
        string input;
        Console.WriteLine("Enter Username: ");
        input = Console.ReadLine();
        sw.WriteLine(input.Crypt());
        Console.WriteLine("Enter password: ");
        input = Console.ReadLine();
        sw.WriteLine(input.Crypt());

        string msg = sr.ReadLine().Decrypt();


        if (msg == "Create Success")
        {
            Console.WriteLine("Success");
        }
        else
            Console.WriteLine("Failiure");

        System.Threading.Thread.Sleep(500);
    }
}