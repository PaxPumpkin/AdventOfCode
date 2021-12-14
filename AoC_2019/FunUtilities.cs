using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using AoC_2019;


namespace FunUtilities
{
    /// <summary>
    /// Class to essentially print over top of the same console line over
    /// and over again instead of outputing to the console in a scrolling-
    /// required way. The constructor take a template string to determine 
    /// the number of backspaces to use with each print so that previous
    /// output is deleted before the new output is sent. Output should be
    /// of a consistent length to the original template. 
    /// </summary>
    public class HackerScannerPrint
    {
        private static Random rnd = new Random();
        private string stringToPrint = "";
        private string backspaces = "";
        private char markerCharacter = '-';
        /// <summary>
        /// list of characters that will be used during replacement of marker characters during Print or PrintScanner calls.
        /// Set to a custom string of your own chars if the default are not desired.
        /// </summary>
        public string hackChars = @"!@#$%^&*()`~\|_?;:ABCDEF0123456789";
        /// <summary>
        /// With the given print template to setup the number of backspaces to use
        /// with each subsequent print, will use the default '-' char as the 
        /// in-string marker to replace with random chars on those subsequent print calls.
        /// Do not include the '-' char in subsequent print calls if random replacement behavior is
        /// not desired. Or, call the overloaded constructor to specify a different 
        /// replacement marker character if the default is desired to print without replacement.
        /// </summary>
        /// <param name="printTemplate"></param>
        public HackerScannerPrint(string printTemplate)
        {
            stringToPrint = printTemplate;
            backspaces = new string('\b', stringToPrint.Length);
        }
        /// <summary>
        /// With the given print template to setup the number of backspaces to use
        /// with each subsequent print, will use the specified char sent in the ctor as the 
        /// in-string marker to replace with random chars on those subsequent print calls.
        /// On subsequent calls to "Print", all locations where the specified marker char are
        /// found will be replaced with a random character, each time. 
        /// </summary>
        /// <param name="printTemplate"></param>
        /// <param name="replaceChar"></param>
        public HackerScannerPrint(string printTemplate, char replaceChar)
        {
            stringToPrint = printTemplate;
            markerCharacter = replaceChar;
            backspaces = new string('\b', stringToPrint.Length);

        }
        /// <summary>
        /// Typical call to output a string. Will call the multiparam
        /// overload of Print with a "false" bool to indicate that 
        /// backspaces should be output first to clear the line and re-print it.
        /// Any instances of the "marker" char used in class ctor will be replaced
        /// with a random character. Default ctor uses '-' as the marker. 
        /// The overloaded ctor allows specifying the replacement marker character
        /// if the default char is desired to be used legitimately in the output 
        /// and replacement is not desired. 
        /// </summary>
        /// <param name="toPrint"></param>
        public void Print(string toPrint) { Print(toPrint, false); }
        /// <summary>
        /// Typical call to output a string. If called with "true"  for the intial
        /// parameter, backspaces WILL NOT be printed out first. 
        /// If called with "false", backspaces will be printed first to erase previous
        /// output on the screen. The number of backspaces printed is determined during
        /// the call to the ctor with a template that has the length that will be used
        /// during subsequent calls. 
        /// Any instances of the "marker" char used in class ctor will be replaced
        /// with a random character. Default ctor uses '-' as the marker. 
        /// The overloaded ctor allows specifying the replacement marker character
        /// if the default char is desired to be used legitimately in the output 
        /// and replacement is not desired. 
        /// </summary>
        /// <param name="toPrint"></param>
        /// <param name="initial"></param>
        public void Print(string toPrint, bool initial)
        {
            if (!initial)
            {
                Console.Write(backspaces);
            }
            Console.Write(fillRandom(toPrint, markerCharacter));
        }
        /// <summary>
        /// Like the standard "Print" methods, this will overwrite the same
        /// space over and over again, replacing any "marker chars" with random
        /// characters. 
        /// This method won't "blow up" if you 
        /// send strings of varying lengths each time, but beware that 
        /// if you send a longer string, any characters output may remain
        /// on the screen if you later send a shorter one that doesn't entirely
        /// overwrite that length with blank spaces as well. 
        /// This method also puts each randomly replaced character out in a
        /// random color. Just for funsies. You can turn that off by specifying
        /// "false" for the colorized parameter.
        /// </summary>
        /// <param name="toPrint"></param>
        /// <param name="initial"></param>
        public void PrintScannerColorized(string toPrint, bool initial=false, bool colorized=true)
        {
            // dunno if this is desired behavior, but I wrote it just in case it's useful later...
            //if (toPrint.Length != backspaces.Length) throw new BigFatDummyException("The string you're printing is of a different length than your original template.");

            bool initialCursorState = Console.CursorVisible;
            Console.CursorVisible = false;
            if (!initial) Console.SetCursorPosition(0, Console.CursorTop); 
            toPrint.ToList().ForEach(c => {
                if (c == markerCharacter)
                {
                    if (colorized) Console.ForegroundColor = (ConsoleColor)(rnd.Next(15) + 1); // 1-15. 0 is black, not wanted for output, and max value is 15.
                    else Console.ResetColor(); // standard console defaults.
                    Console.Write(hackChars[rnd.Next(hackChars.Length)]);
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(c);
                }
            });
            Console.CursorVisible = initialCursorState;
        }
        /// <summary>
        /// Given a string and a char to use as replacement position marker,
        /// will return that string with all instances of the marker char
        /// replaced by a random char from the potential values:
        /// !@#$%^&*()`~\|_?;:ABCDEF0123456789
        /// </summary>
        /// <param name="input"></param>
        /// <param name="marker"></param>
        /// <returns></returns>
        public string fillRandom(string input, char marker)
        {
            while (input.IndexOf(marker) >= 0)
            {
                int next = input.IndexOf(marker);
                input = input.Remove(next, 1);
                input = input.Insert(next, hackChars[rnd.Next(hackChars.Length)].ToString());
            }
            return input;
        }
        /// <summary>
        /// This simulates processing in an AoC module that takes a long time
        ///   and where the solution is evolving during that processing. 
        /// It will print out the sample template with the Colorized version of the scanner print vs the plain white console color.
        /// Every 500 iterations, it will randomly replace a character marker with a "Solved" character
        ///   until all of the replacement chars are gone(indicating all processing is finished)
        ///   
        /// To check it out, just call this in a module: FunUtilities.HackerScannerPrint.SampleAndTest();
        /// </summary>
        public static void SampleAndTest()
        {
            Console.WriteLine("");// For CRLF periods in case our cursor is in a weird place. 
            Console.WriteLine("Testing Colorized Scanner Print");
            //The length of all subsequent calls should match your template's length. 
            //There is a check on that in the print method, but it is currently commented out.
            //   SO..... you can vary your actual later output length, but unless you're careful to also output enough " " chars to overwrite the extra stuff
            //       it will stay on the screen when you later go back to shorter string outputs. Just be aware. 
            string hsTemplate = "Calculating: _________ KEY"; 
            FunUtilities.HackerScannerPrint hs = new FunUtilities.HackerScannerPrint(hsTemplate, '_'); // setup with template length and char that will be replaced with random stuff on each print.
            hs.PrintScannerColorized(hsTemplate, true); // initial print output. The colorized print method just sets the cursor to the beginning of the line and doesn't actually care as much, but this is a demo.
            // OR....
            // the original version which doesn't do colorization and DOES use the preset backspace counts, so.... this could make it extra wonky if you vary your printed string length.
            //hs.Print(hsTemplate,true);
            bool testing = true; // flag for the WHILE loop.
            long timeCounter = 0; // iteration counter for processing simulation.
            Random rnd = new Random(); // fun to randomly pick which part of the puzzle is being "solved"

            // because I hate seeing it bounce around during processing, I'm turning off the cursor for now. 
            // but saving whatever state it was in to restore it later, just in case whatever called this test/demo method already had it off. 
            // I wouldn't want to turn it back on explicitly when I'm done here, when it was desired to be off initially. 
            bool initialCursorState = Console.CursorVisible;
            Console.CursorVisible = false; 

            //Processing simulation! 
            while (testing)
            {
                // the optional "initial" parameter defaults to false, and optional "colorized" parameter defaults to "true". 
                // So this call is equivalent to "hs.PrintScannerColorized(hsTemplate, false, true);" and even "hs.PrintScannerColorized(hsTemplate, false);"
                hs.PrintScannerColorized(hsTemplate); 
                // to do basic non-colorized version, you would do this:
                //hs.PrintScannerColorized(hsTemplate, false, false);
                //     OR the original version, also non-colorized:
                //hs.Print(hsTemplate);
                timeCounter++; // an iteration! Do first since %X of 0 is also 0 and it would "solve" the first one immediately. 
                // if it's time to solve a piece and there are still unsolved pieces....
                if (timeCounter % 500 == 0 && hsTemplate.IndexOf('_') != -1)
                {
                    int remaining = hsTemplate.Count(x => x == '_'); // how many unsolved bits?
                    int whichOne = rnd.Next(remaining) + 1; // randomly pick one of them to solve.
                    int startIndex = 0;
                    for (int x = 0; x < whichOne; x++) // loop through the random number of times to find the one we're solving.
                    {
                        startIndex = hsTemplate.IndexOf('_', startIndex + 1); // if the template started with a replacement char, this wouldn't work, but....
                    }
                    hsTemplate = hsTemplate.Remove(startIndex, 1); // ok, pull out our randomly selected part.
                    hsTemplate = hsTemplate.Insert(startIndex, "X"); // and mark it "solved" so that it won't get replaced with a random char each time we print. 
                }
                else
                {
                    // if on any iteration, there are no more simulated "unsolved" pieces, exit the loop.
                    if (hsTemplate.IndexOf('_') == -1) testing = false;
                }
            }

            //Disabled the exception code, but, just in case it pops up again later....
            //try
            //{
            //    // Since this would print out a longer string than our original template defined, the backspacing and/or overwriting will go wonky. Throws an exception!
            //    hs.PrintScannerColorized(hsTemplate + "ShouldThrowException");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("");
            //    Console.WriteLine("Got an error, boss:");
            //    Console.WriteLine(ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //}

            Console.CursorVisible = initialCursorState;// put it back to however I found it, even if it was off at the start. 
            Console.WriteLine(""); // essentially just for CRLF since we've been working on the same line, and that is where the cursor still is. We want to move on.
            Console.WriteLine("Testing Completed.");
        }
    }

    public class BigFatDummyException:Exception
    {
        public BigFatDummyException(string exceptionText):base("You BIG FAT DUMMY!\n" + exceptionText)
        { }
    }
}
namespace AoCLeaderBoardClasses
{
    public class LeaderBoard
    {
        string html = string.Empty;
        string url = @"https://adventofcode.com/2020/leaderboard/private/view/100445.json"; // default
        public LeaderBoard(string leaderboardEndpoint)
        {
            url = leaderboardEndpoint;
        }
        public void DoProcess()
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Reddit User PaxPumpkin:AoC - 197110";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(new Cookie("session", "53616c7465645f5f955f0705aea211aa85caba0ba0b28d695d70803818b83e203b55e1c62e09e86c8c628e544a547f76", "/", ".adventofcode.com"));
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            Leaderboard leaderboard = new Leaderboard();
            object result = new JavaScriptSerializer().DeserializeObject(html);
            Dictionary<string, object> mutableHelperObject = (Dictionary<string, object>)result;
            leaderboard.event_id = (string)mutableHelperObject["event"];
            leaderboard.owner_id = (string)mutableHelperObject["owner_id"];
            mutableHelperObject = (Dictionary<string, object>)mutableHelperObject["members"];
            leaderboard.members = new List<Member>();
            foreach (string key in mutableHelperObject.Keys)
            {
                Dictionary<string, object> memberObject = (Dictionary<string, object>)mutableHelperObject[key];
                Member member = new Member();
                member.local_score = (int)memberObject["local_score"];
                member.global_score = (int)memberObject["global_score"];
                member.name = (string)memberObject["name"];
                member.id = (string)memberObject["id"];
                member.stars = (int)memberObject["stars"];
                member.last_star_ts = long.Parse(memberObject["last_star_ts"].ToString()); // arrives as string when nonzero, as int when zero, is long...
                memberObject = (Dictionary<string, object>)memberObject["completion_day_level"];
                member.days = new List<Day>();
                foreach (string dkey in memberObject.Keys)
                {
                    Dictionary<string, object> dayo = (Dictionary<string, object>)memberObject[dkey];
                    Day thisday = new Day();
                    thisday.day_number = int.Parse(dkey);
                    thisday.stars = new List<Star>();
                    if (dayo.ContainsKey("1")) { thisday.stars.Add(new Star() { id = "1", earned_star_ts_epoch = long.Parse((string)((Dictionary<string, object>)dayo["1"])["get_star_ts"]) }); }
                    if (dayo.ContainsKey("2")) { thisday.stars.Add(new Star() { id = "2", earned_star_ts_epoch = long.Parse((string)((Dictionary<string, object>)dayo["2"])["get_star_ts"]) }); }
                    member.days.Add(thisday);

                }
                leaderboard.members.Add(member);
            }
            leaderboard.members = leaderboard.members.OrderByDescending(y => y.local_score).ToList();
            leaderboard.members.ForEach(y => y.days = y.days.OrderBy(z => z.day_number).ToList());

            outputMembers(leaderboard);
            string command = Console.ReadLine();
            while (command.Trim() != "")
            {
                int memberNumber = -1;
                if (command != "" && int.TryParse(command, out memberNumber))
                {
                    if (memberNumber > 0 && memberNumber <= leaderboard.members.Count)
                    {
                        Member member = leaderboard.members[memberNumber - 1];
                        outputMemberInfo(member);
                        command = Console.ReadLine();
                        while (command != "")
                        {
                            outputMemberInfo(member);
                            command = Console.ReadLine();
                        }
                    }
                }

                outputMembers(leaderboard);
                command = Console.ReadLine();
            }
        }
        private void outputMemberInfo(Member member)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(member.name + ":" + member.local_score.ToString() + " points -- " + member.stars.ToString() + " stars total");
            Console.WriteLine("Last star earned at " + member.LastStarDateTime.ToString());
            int counter = 1;
            member.days.ForEach(y =>
            {

                //List<Type> thingy = ass.GetTypes().Where(c => c.BaseType != null && c.BaseType.Name.Equals("AoCodeModule")).ToList();
                Type dayType = AoCodeShell.thingy.Where(q => q.Name.StartsWith("Day" + y.day_number.ToString().PadLeft(2, '0'))).ToList().FirstOrDefault();
                string goodDayName = "", dayName = dayType != null ? dayType.Name.Substring(6) : "Undetermined";
                int charPointer = 0;
                foreach (char x in dayName)
                {
                    if (charPointer != (dayName.Length - 1) && ((int)dayName[charPointer + 1]) < 97)
                    {
                        goodDayName += x.ToString() + " ";
                    }
                    else { goodDayName += x.ToString(); }
                    charPointer++;
                }
                Console.WriteLine("".PadRight(100, '*'));
                Console.WriteLine("** Day number: " + y.day_number.ToString() + " - " + goodDayName + "  [" + y.stars.Count.ToString() + " star(s)]");
                y.stars.ForEach(z =>
                {
                    Console.WriteLine("** ------" + z.id.ToString() + " earned at " + z.earned_star_ts.ToString());
                });
                if (y.stars.Count > 1)
                {
                    TimeSpan duration = y.stars[1].earned_star_ts - y.stars[0].earned_star_ts;
                    Console.WriteLine("** Time between stars: " + (duration.Days > 0 ? duration.Days.ToString() + " Days, " : "") + duration.Hours.ToString() + " Hours, " + duration.Minutes.ToString() + " minutes, " + duration.Seconds.ToString() + " seconds");
                }
                counter++;
            });
            Console.WriteLine();
            Console.Write("Press Enter to see list of Leaderboard members:");
        }
        private void outputMembers(Leaderboard x)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("************************");
            int counter = 1;
            x.members.ForEach(y => { Console.WriteLine(counter.ToString() + " - " + y.name.PadRight(20, ' ') + "  " + y.local_score.ToString().PadLeft(4, ' ') + " points, " + y.stars.ToString().PadLeft(2, ' ') + " stars. Last Activity: " + y.LastStarDateTime.ToString()); counter++; });
            Console.WriteLine();
            Console.Write("Which to expand(enter to exit): ");
        }
    }
    public class Leaderboard
    {
        public string owner_id { get; set; }
        public List<Member> members { get; set; }
        public string event_id { get; set; }
    }
    public class Member
    {
        public int local_score { get; set; }
        public int global_score { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public int stars { get; set; }
        public long last_star_ts { get { return last_star_ts_unix_epoch; } set { last_star_ts_unix_epoch = value; LastStarDateTime = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(value); } }
        private long last_star_ts_unix_epoch { get; set; }
        public DateTime LastStarDateTime { get { return LastStarDateTimeP.ToLocalTime(); } set { LastStarDateTimeP = value; } }
        private DateTime LastStarDateTimeP;
        public List<Day> days { get; set; }
    }
    public class Day
    {
        public List<Star> stars { get; set; }
        public int day_number { get; set; }
    }
    public class Star
    {
        public string id { get; set; }
        public long earned_star_ts_epoch { get { return earned_star_ts_unix; } set { earned_star_ts_unix = value; earned_star_ts = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(value); } }
        private long earned_star_ts_unix { get; set; }
        public DateTime earned_star_ts { get { return earned_star_tsP.ToLocalTime(); } set { earned_star_tsP = value; } }
        private DateTime earned_star_tsP;
    }
}