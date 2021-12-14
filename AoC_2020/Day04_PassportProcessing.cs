using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC_2020
{
    class Day04_PassportProcessing : AoCodeModule
    {
        List<Dictionary<string,string>> passports= new List<Dictionary<string,string>>();
        public Day04_PassportProcessing()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //** > Result for Day04_PassportProcessing part 1:There are 216 valid passports(Process: 0 ms)
            //** > Result for Day04_PassportProcessing part 2:There are 150 validated passports(Process: 1 ms)
            
                        Dictionary<string, string> passport = new Dictionary<string, string>();
            inputFile.ForEach(inputLine => 
            {
                if (inputLine.Trim().Equals(""))
                {
                    passports.Add(passport);
                    passport = new Dictionary<string, string>();
                }
                else
                {
                    string[] bitsNpieces = inputLine.Trim().Split(new char[] { ' ', ':' });
                    for (int x=0; x<bitsNpieces.Length; x+=2)
                    {
                        passport.Add(bitsNpieces[x], bitsNpieces[x + 1]);
                    }
                }
            });
            if (passport.Count > 0) { passports.Add(passport); }
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            finalResult = "There are " + passports.Count(testPP=> (testPP.Count()==8 || (testPP.Count()==7 && !testPP.ContainsKey("cid")) )).ToString() + " valid passports";
            AddResult(finalResult);
            ResetProcessTimer(true);
            List<string> EyeColors = new List<string>() { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            finalResult = "There are " + passports.Count(testPP2 =>
            {
                bool result = true; int testNum;
                result = result && testPP2.ContainsKey("byr") && (testPP2["byr"].Length == 4 && Int32.TryParse(testPP2["byr"], out testNum) && testNum >= 1920 && testNum <= 2002);
                result = result && testPP2.ContainsKey("iyr") && (testPP2["iyr"].Length == 4 && Int32.TryParse(testPP2["iyr"], out testNum) && testNum >= 2010 && testNum <= 2020);
                result = result && testPP2.ContainsKey("eyr") && (testPP2["eyr"].Length == 4 && Int32.TryParse(testPP2["eyr"], out testNum) && testNum >= 2020 && testNum <= 2030);
                result = result && testPP2.ContainsKey("hgt") && ((testPP2["hgt"].EndsWith("cm") || testPP2["hgt"].EndsWith("in")) &&
                        (testPP2["hgt"].EndsWith("in")) ? (Int32.TryParse(testPP2["hgt"].Substring(0, testPP2["hgt"].Length-2), out testNum) && testNum >= 59 && testNum <= 76) 
                        : (Int32.TryParse(testPP2["hgt"].Substring(0, testPP2["hgt"].Length - 2), out testNum) && testNum >= 150 && testNum <= 193));
                // could probably use TryParse with hex indicators, but.... this works.
                result = result && testPP2.ContainsKey("hcl") && testPP2["hcl"].StartsWith("#") && testPP2["hcl"].Length == 7 && IsHex(testPP2["hcl"].Substring(1));
                result = result && testPP2.ContainsKey("ecl") && EyeColors.Contains(testPP2["ecl"]);
                result = result && testPP2.ContainsKey("pid") && testPP2["pid"].Length == 9 && Int32.TryParse(testPP2["pid"], out testNum);
                return result;
            }).ToString() + " validated passports";
            AddResult(finalResult);
        }
        private bool IsHex(string test)
        {
            bool isHex;
            foreach (char c in test)
            {
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }
            return true;
        }
    }
}
