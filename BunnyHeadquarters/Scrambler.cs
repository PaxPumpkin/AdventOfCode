using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BunnyHeadquarters
{
    class Scrambler
    {

        static void Main(string[] args)
        {
            Password password = new Password("fbgdceah");
            //Password password = new Password("abcdefgh");
            StreamReader sr = new StreamReader("C:\\Sandbox\\2016AoC\\BunnyHeadquarters\\BunnyHeadquarters\\scrambler.txt");
            List<string> ScrambleCommands = new List<string>();
            while (!sr.EndOfStream)
            {
                ScrambleCommands.Add(sr.ReadLine());
            }
            sr.Close();
            //for (int x = 0; x < ScrambleCommands.Count; x++ )
            for (int x = ScrambleCommands.Count-1; x >=0; x--)
            {
                string scrambleCommand = ScrambleCommands[x];
                string[] cmd = scrambleCommand.Split(new char[] { ' ' });
                Console.WriteLine(scrambleCommand);
                switch (cmd[0])
                {
                    case "rotate":
                        if (cmd[1].Equals("right")) { password.RotateLeft(Int32.Parse(cmd[2])); }
                        else if (cmd[1].Equals("left")) { password.RotateRight(Int32.Parse(cmd[2])); }
                        else { password.RotateForCharX((cmd[cmd.GetUpperBound(0)].ToCharArray())[0]); }
                        break;

                    case "swap":
                        if (cmd[1].Equals("position")) { password.Swap(Int32.Parse(cmd[5]), Int32.Parse(cmd[2])); }
                        else { password.SwapC((cmd[5].ToCharArray())[0], (cmd[2].ToCharArray())[0]); }
                        break;

                    case "reverse":
                        password.ReverseSection(Int32.Parse(cmd[4]), Int32.Parse(cmd[2]));
                        break;

                    case "move":
                        password.MoveChars(Int32.Parse(cmd[5]), Int32.Parse(cmd[2]));
                        break;

                    default:
                        Console.WriteLine("OOOOOPSIE!!! Cannot parse: " + scrambleCommand);
                        break;

                }

            }
            //test.Swap(4, 0);
            //test.SwapC('d', 'b');
            //test.ReverseSection(0, 4);
            //test.RotateLeft(1);
            //test.MoveChars(1, 4);
            //test.MoveChars(3, 0);
            //test.RotateForChar('b');
            //test.RotateForChar('d');
            Console.WriteLine("final: " + password.currentValue);
        }
    }
    class Password
    {
        string password = "";
        public string currentValue { get { return password; } }
        public Password(string pw)
        {
            password = pw;
        }
        public void Swap(int x, int y)
        {
            char[] temp = password.ToCharArray();
            temp[x] = password[y];
            temp[y] = password[x];
            password= new string(temp);
        }
        public void SwapC(char x, char y)
        {
            int x2 = password.IndexOf(x);
            int y2 = password.IndexOf(y);
            Swap(x2, y2);
        }
        public void RotateLeft(int steps) { Rotate(-1, steps); }
        public void RotateRight(int steps) { Rotate(1, steps); }
        public void Rotate(int direction, int steps)
        {
            for (int x=0; x<steps; x++){
                string movingChar = password[(direction>0?password.Length-1:0)].ToString();
                password = (direction>0)? movingChar + password.Substring(0, password.Length - 1):password.Substring(1)+movingChar;
            }
        }
        public void RotateForChar(char x)
        {
            int charindex = password.IndexOf(x);
            Rotate(1, (1 + charindex + (charindex>=4?1:0)));
        }
        public void RotateForCharX(char x)
        {
            Rotate(-1, 1);

            int charindex = password.IndexOf(x);
            Rotate(-1, (1 + charindex + (charindex >= 4 ? 1 : 0)));
        }
        public void ReverseSection(int x, int y)
        {
            char[] temp = password.ToCharArray();
            int floor = x;
            for (int i = y; i >= floor; i--)
            {
                temp[x++] = password[i];
            }
            password = new string(temp);
        }
        public void MoveChars(int x, int y)
        {
            char[] temp = password.ToCharArray();
            char yankedChar = password[x];
            string temp2 = "";
            for (int i = 0; i < password.Length; i++)
            {
                if (i != x) temp2 += temp[i].ToString();
            }
            int charPointer=0;
            for (int i = 0; i < password.Length; i++)
            {
                temp[i]= (i==y)?yankedChar:temp2[charPointer];
                charPointer += (i == y) ? 0 : 1;
            }
            password = new string(temp);
        }
    }
}
