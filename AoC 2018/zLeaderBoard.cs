using System;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class zLeaderBoard : AoCodeModule
    {
        public zLeaderBoard()
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
            string html = string.Empty;
            //string url = @"https://adventofcode.com/2018/leaderboard/private/view/100445.json";
            string url = @"https://adventofcode.com/2020/leaderboard/private/view/100445.json";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Reddit User PaxPumpkin:AoC - 197110";
            request.CookieContainer = new CookieContainer();
            //request.CookieContainer.Add(new Cookie("session", "53616c7465645f5fed9514fa0fda47fdb7f81a67b20e5390982fd9c8dfd7314eb92630ed162782f332d317e51b274141","/", ".adventofcode.com"));
            request.CookieContainer.Add(new Cookie("session", "53616c7465645f5f955f0705aea211aa85caba0ba0b28d695d70803818b83e203b55e1c62e09e86c8c628e544a547f76", "/", ".adventofcode.com"));
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            Leaderboard x = new Leaderboard();
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(x.GetType());
            //x = (Leaderboard)serializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(html)));
            object result = new JavaScriptSerializer().DeserializeObject(html);
            Dictionary<string,object> helper = (Dictionary<string,object>)result;
            x.event_id = (string)helper["event"];
            x.owner_id = (string)helper["owner_id"];
            helper = (Dictionary<string, object>)helper["members"];
            x.members = new List<Member>();
            foreach (string key in helper.Keys)
            {
                Dictionary<string, object> membero = (Dictionary<string, object>)helper[key];
                Member member = new Member();
                member.local_score = (int)membero["local_score"];
                member.global_score = (int)membero["global_score"];
                member.name = (string)membero["name"];
                member.id = (string)membero["id"];
                member.stars = (int)membero["stars"];
                member.last_star_ts = long.Parse(membero["last_star_ts"].ToString());
                membero = (Dictionary<string, object>)membero["completion_day_level"];
                member.days = new List<Day>();
                foreach (string dkey in membero.Keys)
                {
                    Dictionary<string, object> dayo = (Dictionary<string, object>)membero[dkey];
                    Day thisday = new Day();
                    thisday.day_number = int.Parse(dkey);
                    thisday.stars = new List<Star>();
                    if (dayo.ContainsKey("1")) { thisday.stars.Add(new Star() { id = "1", earned_star_ts_epoch = long.Parse((string)((Dictionary<string, object>)dayo["1"])["get_star_ts"]) });}
                    if (dayo.ContainsKey("2")) { thisday.stars.Add(new Star() { id = "2", earned_star_ts_epoch = long.Parse((string)((Dictionary<string, object>)dayo["2"])["get_star_ts"]) });}
                    member.days.Add(thisday);

    	        }
                x.members.Add(member);
            }
            x.members = x.members.OrderByDescending(y => y.local_score).ToList();
            x.members.ForEach(y => y.days = y.days.OrderBy(z => z.day_number).ToList());
            
            outputMembers(x);
            string command = Console.ReadLine();
            while (command.Trim() != "")
            {
                int memberNumber = -1;
                if (command != "" && int.TryParse(command, out memberNumber))
                {
                    if (memberNumber > 0 && memberNumber <= x.members.Count)
                    {
                        Member member = x.members[memberNumber-1];
                        outputMemberInfo(member);
                        command = Console.ReadLine();
                        while (command != "")
                        {
                            outputMemberInfo(member);
                            command = Console.ReadLine();
                        }
                    }
                }

                outputMembers(x);
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
            member.days.ForEach(y => {

                //List<Type> thingy = ass.GetTypes().Where(c => c.BaseType != null && c.BaseType.Name.Equals("AoCodeModule")).ToList();
                Type dayType = AoCodeShell.thingy.Where(q => q.Name.StartsWith("Day" + y.day_number.ToString().PadLeft(2, '0'))).ToList().FirstOrDefault();
                string goodDayName="", dayName = dayType != null ? dayType.Name.Substring(6):"Undetermined";
                int charPointer = 0;
                foreach (char x in dayName)
                {
                    if (charPointer != (dayName.Length - 1) && ((int)dayName[charPointer + 1]) < 97)
                    {
                        goodDayName +=  x.ToString() + " ";
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
                    Console.WriteLine("** Time between stars: " + (duration.Days>0?duration.Days.ToString() + " Days, ":"") + duration.Hours.ToString() + " Hours, " + duration.Minutes.ToString() + " minutes, " + duration.Seconds.ToString() + " seconds"  );
                }
                counter++; });
            Console.WriteLine();
            Console.Write("Press Enter to see list of Leaderboard members:");
        }
        private void outputMembers(Leaderboard x)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("************************");
            int counter = 1;
            x.members.ForEach(y => { Console.WriteLine(counter.ToString() + " - " + y.name.PadRight(20,' ') + "  " + y.local_score.ToString().PadLeft(4,' ') + " points, " + y.stars.ToString().PadLeft(2,' ') + " stars. Last Activity: " + y.LastStarDateTime.ToString()); counter++; });
            Console.WriteLine();
            Console.Write("Which to expand(enter to exit): ");
        }
    }
    public class Leaderboard {
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
        public long last_star_ts { get { return last_star_ts_unix_epoch; } set { last_star_ts_unix_epoch = value;  LastStarDateTime = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(value); } }
        private long last_star_ts_unix_epoch { get; set; }
        public DateTime LastStarDateTime { get { return LastStarDateTimeP.ToLocalTime(); } set { LastStarDateTimeP = value; } }
        private DateTime LastStarDateTimeP;
        public List<Day> days { get; set; }
    }
    public class Day {
        public List<Star> stars { get; set; }
        public int day_number { get; set; }
    }
    public class Star {
        public string id { get; set; }
        public long earned_star_ts_epoch { get { return earned_star_ts_unix; } set { earned_star_ts_unix = value; earned_star_ts= (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(value); } }
        private long earned_star_ts_unix { get; set; }
        public DateTime earned_star_ts { get { return earned_star_tsP.ToLocalTime(); } set { earned_star_tsP = value; } }
        private DateTime earned_star_tsP;
    }
}
