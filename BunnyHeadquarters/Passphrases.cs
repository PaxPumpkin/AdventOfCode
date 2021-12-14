using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BunnyHeadquarters
{
    class Passphrases
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("C:\\Sandbox\\2016AoC\\BunnyHeadquarters\\BunnyHeadquarters\\passphrases.txt");
            List<string> phrases = new List<string>();
            while (!sr.EndOfStream)
            {
                phrases.Add(sr.ReadLine());
            }
            sr.Close();
            int good = 0;
            int goodResorted = 0;
            foreach (string passphrase in phrases)
            {
                bool failed = false;
                bool failedResorted = false;
                // this is just to compare the individual words in each phrase
                List<string> constituents = new List<string>(passphrase.Split(new char[] { ' ' }));

                // this will init an empty list, take each word from the phrase, and reorder the individual characters.
                // any words that are anagrams will make up a matching list of letters as a final result. (example from website, oiii and iioi and ioii and end up as individual strings sorted as iiio, which will violate the original count==1 constraint
                List<string> resortedconstituents = new List<string>();
                // this could be made more efficient by changing the flag system below and only iterating the original list once, but this isn't performance heavy in this application...
                foreach (string element in constituents)
                {
                    char[] sorted = element.ToCharArray().OrderBy(item => item.ToString()).ToArray();
                    resortedconstituents.Add(new string(sorted));
                }


                foreach (string constituent in constituents)
                {
                    failed = constituents.Count(item=>item.Equals(constituent))>1;
                    if (failed)
                        break;
                }
                if (!failed)
                    good++;


                foreach (string constituent in resortedconstituents)
                {
                    failedResorted = resortedconstituents.Count(item => item.Equals(constituent)) > 1;
                    if (failedResorted)
                        break;
                }
                if (!failedResorted)
                    goodResorted++;
            }
            Console.WriteLine("good original ones:" + good.ToString());
            Console.WriteLine("good resorted ones:" + goodResorted.ToString());
            Console.ReadLine();
        }
    }
}
