using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2016
{
    class Day04_SecurityThroughObscurity : AoCodeModule
    {
        public Day04_SecurityThroughObscurity()
        {
            // If you always save input file in the /InputFiles/ subfolder and name it the same as the class processing it, this will work.
            // if you put it elsewhere or name it differently, just change below. 
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
            //Print("Did Something");// outputs to console immediately
            //Print("DidSomethingElse", FileOutputAlso); // both console and output file
            //Print("Another Thing", FileOutputOnly); // output file only.
        }
        public override void DoProcess()
        {
            //If Comma Delimited on a single input line
            //List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            //string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            List<Room> rooms = new List<Room>();
            foreach (string processingLine in inputFile)
            {
                Room newRoom = new Room(processingLine);
                rooms.Add(newRoom);
            }
            AddResult(rooms.Where(x=>x.IsRealRoom==true).Sum(x=>x.sectorId).ToString()); // includes elapsed time from last ResetProcessTimer
            ResetProcessTimer(true);
            List<Room> possibleNP = rooms.Where(x => x.IsRealRoom == true && x.realRoomName.Contains("north") && x.realRoomName.Contains("pole")).ToList();
            foreach (Room x in possibleNP) {
                AddResult(x.realRoomName + " - " + x.sectorId.ToString());
            }
        }
    }
    public class Room {
        string checksum = "";
        public int sectorId = 0;
        string encrypt = "";
        string calcChecksum = "";
        public string realRoomName = "";
        public bool IsRealRoom{get{ return checksum==calcChecksum;} }
        public Room(string definition)
        {
            string[] pieces = definition.Split(new char[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
            checksum = pieces[pieces.GetUpperBound(0)]; // get last piece.
            checksum = checksum.Replace(']', ' ').Trim(); // remove that silly end bracket
            pieces = pieces[0].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries); // split into the pieces.
            sectorId = int.Parse(pieces[pieces.GetUpperBound(0)]); // last part is always the sector ID and always a number
            foreach (string piece in pieces)
            {
                encrypt += (piece != sectorId.ToString()) ? piece : ""; //smush them together except the sector id
                int advanceLetter = sectorId % 26; // only 26 letters in the alphabet, so every 26 just ends where it starts. We want to know how much FARTHER to shift. 
                int a = ((int)'a') - 1, z = (int)'z'; // a is amount to add to base to GET to 'a', z is max value to allow for shifted characters.
                if (piece != sectorId.ToString())// only shifting the encrypted parts, not the sector ID part.
                {
                    foreach (char letter in piece)// shift each letter
                    {
                        int newLetter = ((int)letter) + advanceLetter; // shift the letter by the leftover shift amount
                        realRoomName += ((char)((newLetter > z) ? newLetter - z + a : newLetter)).ToString(); // if it is past z, subtract the value of z and then add the beginning point for a to start over.
                    }
                    realRoomName += " "; // represents the "-" character which says it was a space in the description. 
                }
            }
            realRoomName = realRoomName.Trim(); //whatever
            // groups by character with count in an new anonymous object
            var possible = encrypt.GroupBy(line => line).Select(line => new { k = line.Key, cnt = line.Count() }).ToList();
            // order by the count (highest to lowest), then by alpha on the character involved
            possible = possible.OrderByDescending(x => x.cnt).ThenBy(x => x.k).ToList();
            for (int x = 0; x < 5; x++) // take first 5 characters to make the checksum
            {
                calcChecksum += possible[x].k.ToString();
            }


        }
    }
}
