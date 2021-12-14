using System;
using System.Collections.Generic;
using System.Linq;


namespace AoC_2020
{
    class Day07_HandyHaversacks : AoCodeModule
    {
        List<Bag> primaryDefinitions = new List<Bag>();
        int counter = 0;
        int recursionCounter = 0;
        List<string> bagsChecked = new List<string>();
        public Day07_HandyHaversacks()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            //** > Result for Day07_HandyHaversacks part 1:Data setup completed.(Process: 20 ms)
            //** > Result for Day07_HandyHaversacks part 2:224 bags can either hold my bag directly or hold something that will eventually also hold my bag(Process: 29 ms)
            //** > Result for Day07_HandyHaversacks part 3:My shiny gold bag must contain 1488 other bags(Process: 7 ms)
             
            string finalResult = "Not Set";
            ResetProcessTimer(true);
            
            //This data setup realllllllly sucks. There is probably a regex that would do it quickly, but I'd need to brush up on my regex. BLAH. I HATE REGEX.
            int index = 0;
            foreach (string processingLine in inputFile)
            {
                string workingValue = processingLine;
                //posh chartreuse bags contain 2 faded blue bags, 4 dark coral bags, 2 light maroon bags, 5 dark purple bags.
                index = workingValue.IndexOf("bags");
                string initialDescriptor = workingValue.Substring(0, index - 1); //"posh chartreuse"
                string[] newDefinitions = initialDescriptor.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                workingValue = workingValue.Substring(index + 13); // everything after "bags contain "
                List<string> insideBags = workingValue.Split(new char[] { ',' }).ToList();
                for (int x=0; x<insideBags.Count; x++)
                {
                    insideBags[x] = insideBags[x].Substring(0, insideBags[x].IndexOf("bag") - 1);// ex: instead of "2 faded blue bags", we get "2 faded blue"
                }

                Bag newBag = primaryDefinitions.Count(x => x.adjective == newDefinitions[0] && x.color == newDefinitions[1]) == 0?new Bag() { adjective = newDefinitions[0], color = newDefinitions[1] }: primaryDefinitions.Where(x => x.adjective == newDefinitions[0] && x.color == newDefinitions[1]).First();
                insideBags.ForEach(ib =>
                {
                    if (ib != "no other")
                    {
                        string[] ibdescriptors = ib.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        Bag ibx = primaryDefinitions.Count(x => x.adjective == ibdescriptors[1] && x.color == ibdescriptors[2]) == 0 ? new Bag() { adjective = ibdescriptors[1], color = ibdescriptors[2] } : primaryDefinitions.Where(x => x.adjective == ibdescriptors[1] && x.color == ibdescriptors[2]).First();
                        // if this were a new bag that wasn't in the primary definitions already, technically should go ahead and add it.... I didn't at first, and I don't think it will affect the answer since I don't rely on actual object references, just values 
                        if (primaryDefinitions.Count(x=>x.adjective==ibx.adjective && x.color == ibx.color) == 0) { primaryDefinitions.Add(ibx); }
                        int count = Convert.ToInt32(ibdescriptors[0]);
                        for (int i = 0; i < count; i++) { newBag.holds.Add(ibx); } // yeah, adding multiples of the same bag. I could probably break this out, but it turns out it works realllllly well as-is
                    }
                });
                primaryDefinitions.Add(newBag);

            }
            AddResult("Data setup completed.");

            ResetProcessTimer(true);
            Bag myBag = primaryDefinitions.Where(x => x.adjective == "shiny" && x.color == "gold").First();
            RecursiveBagHolding(myBag);
            // decrement counter by one, since the recursion actually counts my original bag, and since it can't actually hold itself...
            counter--;
            finalResult = counter.ToString() + " bags can either hold my bag directly or hold something that will eventually also hold my bag";
            AddResult(finalResult); 

            ResetProcessTimer(true);
            counter = 0;
            recursionCounter = 0;
            RecursiveBagCounting(myBag);
            finalResult = "My shiny gold bag must contain " + counter.ToString() + " other bags";
            AddResult(finalResult);
        }

        public void RecursiveBagHolding(Bag brandNewBag)
        {
            recursionCounter++;
            // this list of strings to prevent overiteration what's already been iterated is a serious process time hog :(
            string bagDescription = brandNewBag.adjective + " " + brandNewBag.color;
            if (bagsChecked.Count(x => x == bagDescription) == 0)
            {
                bagsChecked.Add(bagDescription);
                //Print((new String('\t', recursionCounter - 1)) + "Looking to See What Holds a " + bagDescription, FileOutputAlso);
                // I could fix this as an actual object-equality selector, instead of matching the string values, but.... whatever. 
                List<Bag> holdingTheBag = primaryDefinitions.Where(x => x.holds.Count(someBag => someBag.adjective == brandNewBag.adjective && someBag.color == brandNewBag.color) > 0).ToList();
                foreach (Bag ofHolding in holdingTheBag)
                {
                    RecursiveBagHolding(ofHolding);
                }
                counter++;
            }
            recursionCounter--;
        }
        public void RecursiveBagCounting(Bag brandNewBag)
        {
            recursionCounter++;
            //string bagDescription = brandNewBag.adjective + " " + brandNewBag.color;
            //Print((new String('\t', recursionCounter - 1)) + "A " +  bagDescription + " holds " + brandNewBag.holds.Count().ToString() + " other bags(" + counter.ToString() + " + " + brandNewBag.holds.Count().ToString() + ")", FileOutputAlso);
            counter += brandNewBag.holds.Count();
            foreach (Bag ofHolding in brandNewBag.holds)
            {
                RecursiveBagCounting(primaryDefinitions.Where(x => x.adjective == ofHolding.adjective && x.color == ofHolding.color).First());
            }
            recursionCounter--;
        }
    }
    class Bag{
        public string adjective;
        public string color;
        public List<Bag> holds= new List<Bag>();
    }
}
