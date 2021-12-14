using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace AoC_2020
{
    class Day21_AllergenAssessment : AoCodeModule
    {
        public Day21_AllergenAssessment()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();            
        }
        public override void DoProcess()
        {
			//** > Result for Day21_AllergenAssessment part 1:The ingredients appear 2461 times.(Process: 8.1818 ms)
			//** > Result for Day21_AllergenAssessment part 2:The Bad Ingredients List, ordered: ltbj,nrfmm,pvhcsn,jxbnb,chpdjkf,jtqt,zzkq,jqnhd(Process: 1.5056 ms)
			ResetProcessTimer(true);
			List<string> allIngredients = new List<string>();
			List<string> allAllergens = new List<string>();
			Dictionary<string, int> allergenCount = new Dictionary<string, int>(); // how often is this allergen showing up
			Dictionary<string, Dictionary<string, int>> CrossContaminations = new Dictionary<string, Dictionary<string, int>>();
			// have I mentioned recently HOW MUCH I HATE REGEX???? It's just ingredient expedient...
			Regex regex = new Regex(@"(.*) \(contains (.*)\)", RegexOptions.Compiled | RegexOptions.ECMAScript);
			foreach (string funkyFoodThing in inputFile)
			{
				Match match = regex.Match(funkyFoodThing);
				if (match.Success) // uhm, Just in case. But seriously... 
				{
					string[] ingredientList = match.Groups[1].Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					string[] allergensList = match.Groups[2].Value.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
					// ok, so double-iterate and cross-match 
					foreach (string allergen in allergensList)
					{
						foreach (string ingredient in ingredientList)
						{
							if (!CrossContaminations.ContainsKey(ingredient)) CrossContaminations[ingredient] = new Dictionary<string, int>();
							if (!CrossContaminations[ingredient].ContainsKey(allergen)) CrossContaminations[ingredient][allergen] = 0;
							CrossContaminations[ingredient][allergen]++;
						}
						if (!allergenCount.ContainsKey(allergen)) allergenCount[allergen] = 0;
						allergenCount[allergen]++;
					}
					allIngredients.AddRange(ingredientList);
					allAllergens.AddRange(allergensList);
				}
			}

			Dictionary<string, string> ingredientAllergenMixMatch = new Dictionary<string, string>();

			while (true) // until cleared....
			{
				var contamination = CrossContaminations.Where(item => item.Value.Count(ok => ok.Value == allergenCount[ok.Key]) == 1);
				if (contamination.Count() == 0) break; // we outta here!
				foreach (KeyValuePair<string,Dictionary<string,int>> mashup in contamination)
				{
					string theAllergen = mashup.Value.First(item => item.Value == allergenCount[item.Key]).Key;
					ingredientAllergenMixMatch[mashup.Key] = theAllergen;
					foreach (KeyValuePair<string, Dictionary<string, int>> foundAllergen in CrossContaminations) //oopsie poopsie! Clear it out!
					{
						foundAllergen.Value[theAllergen] = 0;
					}
				}
			}
			int count = 0;
			foreach (string item in allIngredients)
			{
				if (!ingredientAllergenMixMatch.ContainsKey(item)) count++;
			}
			AddResult("The ingredients appear " + count.ToString() +" times.");
			ResetProcessTimer(true); // this is kinda dumb. We already DID all the processing.... Whatever. 
			string stupidIngredients = "";
			foreach (KeyValuePair<string,string> ingredient in ingredientAllergenMixMatch.OrderBy(ingredientName => ingredientName.Value))
			{
				stupidIngredients += ingredient.Key + ","; // no spaces, comma delimited.
			}
			stupidIngredients = stupidIngredients.Substring(0, stupidIngredients.Length - 1); // wait! we hate final commas.
			AddResult("The Bad Ingredients List, ordered: " + stupidIngredients);

		}
    }



}
