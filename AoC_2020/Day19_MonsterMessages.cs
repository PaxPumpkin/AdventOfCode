using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AoC_2020
{
    class Day19_MonsterMessages : AoCodeModule
    {
        public Day19_MonsterMessages()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //1213 is too high.
            //** > Result for Day19_MonsterMessages part 1:Original Rule Message Match Count: 213(Process: 184.8509 ms)
            //** > Result for Day19_MonsterMessages part 2:Replacement of rules 8 and 11 get match count: 325(Process: 899.9668 ms)
            string finalResult = "Not Set";
            ResetProcessTimer(true);

            var satelliteData = SatelliteMessageHelper.ParseInputLines(inputFile, null);

            var result = SatelliteMessageHelper.GetNumberOfValidMatches(satelliteData, 0);
            AddResult("Original Rule Message Match Count: " + result.ToString());
            ResetProcessTimer(true);
            //return result;

            var lineReplacements = new Dictionary<string, string>()
            {
                { "8: 42", "8: 42 | 42 8" },
                { "11: 42 31", "11: 42 31 | 42 11 31" }
            };
            satelliteData = SatelliteMessageHelper.ParseInputLines(inputFile, lineReplacements);
            result = SatelliteMessageHelper.GetNumberOfValidMatches(satelliteData, 0);
            AddResult("Replacement of rules 8 and 11 get match count: " + result.ToString());
            //return result;
            /*
            Dictionary<int, List<string>> rules = new Dictionary<int, List<string>>();
            List<string> messages = new List<string>();
            int a=-1, b=-1;
            int ruleNumber;
            int index = 0;
            string[] rulePieces;
            char[] ruleSplitter = new char[] { ':', ' ', '|' };
            List<string> ruleReferences = new List<string>();
            
            int noWhammies = (int)StringSplitOptions.RemoveEmptyEntries;
            while (inputFile[index] != "")
            {
                string rule = inputFile[index].Trim();


                rulePieces = rule.Split(ruleSplitter, noWhammies);
                ruleNumber = int.Parse(rulePieces[0]);
                ruleReferences = new List<string>();
                ruleReferences.Add(rulePieces[1]);
                if (rulePieces.Length > 2)
                { 
                ruleReferences.Add(rulePieces[2]);
                    if (rulePieces.Length > 3)
                    {
                        ruleReferences.Add(rulePieces[3]);
                        ruleReferences.Add(rulePieces[4]);
                    }
                }
                rules.Add(ruleNumber, ruleReferences);
                
                if (rule.Contains("a") || rule.Contains("b"))
                {
                    if (rule.Contains("a"))
                    {
                        a = ruleNumber;
                    }
                    else
                    {
                        b = ruleNumber;
                    }
                }
                index++;
            }
            index++;
            while (index < inputFile.Count)
            {
                messages.Add(inputFile[index].Trim());
                index++;
            }
            string ruleZero = BuildExpression(rules,a, b, 0);
            AddResult(finalResult); 
            */
        }
        public string BuildExpression(Dictionary<int, List<string>> rules, int ruleA, int ruleB, int ruleToResolve)
        {
            string shit = "";
            //rules.Keys.Where(thisKey => rules[thisKey].Contains(ruleA.ToString())).ToList().ForEach(targetKey => rules[targetKey].ForEach(value => value = (value == ruleA.ToString()) ? rules[ruleA]));
            return shit;
        }
        public class SatelliteData
        {
            public IDictionary<int, string> AtomicRules { get; private set; } = new Dictionary<int, string>();
            public IDictionary<int, IList<IList<int>>> SubRules { get; private set; } = new Dictionary<int, IList<IList<int>>>();
            public IList<string> Messages { get; private set; } = new List<string>();
            public SatelliteData(
                IDictionary<int, string> atomicRules,
                IDictionary<int, IList<IList<int>>> subRules,
                IList<string> messages)
            {
                AtomicRules = atomicRules;
                SubRules = subRules;
                Messages = messages;
            }
        }
        public static class SatelliteMessageHelper
        {
            public const string PatternAtomicRule = @"^(\d+): ""(\w+)""$";
            public const string PatternSubRules = @"^(\d+):((\|?\s?\d+\s?)+)$";
            public const string PatternMessage = @"^(\w+)$";

            public static int GetNumberOfValidMatches(SatelliteData satelliteData, int ruleNumber)
            {
                var result = 0;
                foreach (var message in satelliteData.Messages)
                {
                    var isValid = GetIsMatch(message, ruleNumber, satelliteData);
                    if (isValid)
                    {
                        result++;
                    }
                }
                return result;
            }

            public static bool GetIsMatch(
                string message,
                int ruleNumber,
                SatelliteData satelliteData)
            {
                // Each rule in the stack is represented by a tuple:
                // Item1: The list of rules remaining to be checked/expanded
                // Item2: The current index in the message string to be checked
                var rulesToCheck = new Stack<Tuple<IList<int>, int>>();
                var rulesAlreadyVisited = new HashSet<string>();

                // Seed the stack
                rulesToCheck.Push(new Tuple<IList<int>, int>(new List<int>() { ruleNumber }, 0));

                // Loop until a match is found, or the stack is empty
                while (rulesToCheck.Count > 0)
                {
                    var currentRuleToCheck = rulesToCheck.Pop();
                    var currentRuleToCheckString = GetRuleString(currentRuleToCheck.Item1, currentRuleToCheck.Item2);
                    if (rulesAlreadyVisited.Contains(currentRuleToCheckString))
                    {
                        continue;
                    }
                    rulesAlreadyVisited.Add(currentRuleToCheckString);

                    var currentSubRules = currentRuleToCheck.Item1;
                    var currentStartIndex = currentRuleToCheck.Item2;

                    if (currentSubRules.Count == 0)
                        continue;
                    if (currentStartIndex >= message.Length)
                        continue;

                    // Process the first sub rule
                    // If it expands into more sub rules, add those to the front of the rule list and push it back into the stack
                    // If it is atomic, check the message for a match. If there is a match, then increment the start index
                    var rulesToPushToStack = new List<IList<int>>();

                    var subRuleNumberToCheck = currentSubRules[0];
                    var nextSubRules = currentSubRules.ToList();
                    nextSubRules.RemoveAt(0);
                    if (satelliteData.AtomicRules.ContainsKey(subRuleNumberToCheck))
                    {
                        var atomicPatternToCheck = satelliteData.AtomicRules[subRuleNumberToCheck];
                        if (currentStartIndex + atomicPatternToCheck.Length > message.Length)
                            continue;
                        var messageSubstring = message.Substring(currentStartIndex, atomicPatternToCheck.Length);
                        var isSubstringMatch = atomicPatternToCheck.Equals(messageSubstring);
                        currentStartIndex += atomicPatternToCheck.Length;
                        if (isSubstringMatch)
                        {
                            // The message matches the rule if there are no more subrules and the start index is at the end
                            if (currentStartIndex == message.Length
                                && nextSubRules.Count == 0)
                            {
                                return true;
                            }

                            rulesToPushToStack.Add(nextSubRules);
                        }
                    }
                    else if (satelliteData.SubRules.ContainsKey(subRuleNumberToCheck))
                    {
                        // Expand the sub rule with lower-level sub rules
                        var expandedSubRules = satelliteData.SubRules[subRuleNumberToCheck];
                        foreach (var expandedSubRule in expandedSubRules)
                        {
                            var ruleToPush = new List<int>();
                            foreach (var subRuleNumber in expandedSubRule)
                            {
                                ruleToPush.Add(subRuleNumber);
                            }
                            foreach (var subRuleNumber in nextSubRules)
                            {
                                ruleToPush.Add(subRuleNumber);
                            }
                            if (currentStartIndex + ruleToPush.Count <= message.Length)
                            {
                                rulesToPushToStack.Add(ruleToPush);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"Rule not found: {subRuleNumberToCheck}");
                    }

                    // push the reduced list of rules back into the stack
                    foreach (var ruleToPushToStack in rulesToPushToStack)
                    {
                        var ruleTupleToPush = new Tuple<IList<int>, int>(ruleToPushToStack, currentStartIndex);
                        var ruleString = GetRuleString(ruleTupleToPush.Item1, ruleTupleToPush.Item2);
                        if (!rulesAlreadyVisited.Contains(ruleString))
                        {
                            rulesToCheck.Push(ruleTupleToPush);
                        }
                    }
                }

                return false;
            }

            public static string GetRuleString(IList<int> rules, int currentIndex)
            {
                var result = $"{string.Join(",", rules)}:{currentIndex}";
                return result;
            }

            public static Tuple<int, string> ParseAtomicRule(Match match)
            {
                var ruleNumber = int.Parse(match.Groups[1].Value);
                var ruleValue = match.Groups[2].Value;
                var result = new Tuple<int, string>(ruleNumber, ruleValue);
                return result;
            }

            public static Tuple<int, IList<IList<int>>> ParseSubRule(Match match)
            {
                var ruleNumber = int.Parse(match.Groups[1].Value);
                var subRules = new List<IList<int>>();
                var subRulesString = match.Groups[2].Value;
                foreach (var subRuleString in subRulesString.Split('|'))
                {
                    var subRule = new List<int>();
                    var subRuleMatches = Regex.Matches(subRuleString, @"\d+");
                    foreach (Match subRuleMatch in subRuleMatches)
                    {
                        var subRuleNumber = int.Parse(subRuleMatch.Value);
                        subRule.Add(subRuleNumber);
                    }
                    subRules.Add(subRule);
                }
                var result = new Tuple<int, IList<IList<int>>>(ruleNumber, subRules);
                return result;
            }

            public static SatelliteData ParseInputLines(
                IList<string> inputLines,
                IDictionary<string, string> lineReplacements = null)
            {
                var atomicRules = new Dictionary<int, string>();
                var subRules = new Dictionary<int, IList<IList<int>>>();
                var messages = new List<string>();

                // Handle rule replacements
                if (lineReplacements != null)
                {
                    for (int i = 0; i < inputLines.Count; i++)
                    {
                        var inputLine = inputLines[i];
                        if (lineReplacements.ContainsKey(inputLine))
                        {
                            inputLines[i] = lineReplacements[inputLine];
                        }
                    }
                }

                foreach (var inputLine in inputLines)
                {
                    if (string.IsNullOrWhiteSpace(inputLine))
                        continue;

                    var matchAtomicRule = Regex.Match(inputLine, PatternAtomicRule);
                    var matchSubRule = Regex.Match(inputLine, PatternSubRules);
                    var matchMessage = Regex.Match(inputLine, PatternMessage);

                    if (matchAtomicRule.Success)
                    {
                        var atomicRule = ParseAtomicRule(matchAtomicRule);
                        atomicRules.Add(atomicRule.Item1, atomicRule.Item2);
                    }
                    else if (matchSubRule.Success)
                    {
                        var subRule = ParseSubRule(matchSubRule);
                        subRules.Add(subRule.Item1, subRule.Item2);
                    }
                    else if (matchMessage.Success)
                    {
                        messages.Add(inputLine);
                    }
                    else
                    {
                        throw new Exception($"No patterns matched: {inputLine}");
                    }
                }

                var result = new SatelliteData(atomicRules, subRules, messages);
                return result;

            }
        }
    }
}
