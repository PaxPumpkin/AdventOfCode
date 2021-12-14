using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2020
{
    class Day16_TicketTranslation : AoCodeModule
    {
        public Day16_TicketTranslation()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day16_TicketTranslation part 1:Sum of all invalid field values: 27850(Process: 5.6445 ms)
            //** > Result for Day16_TicketTranslation part 2:My ticket departure fields product: 491924517533(Process: 3.9282 ms)
            ResetProcessTimer(true);
            int index = 0;
            List<ValidationCriterion> validators = new List<ValidationCriterion>();
            while (inputFile[index] != "")
            {
                validators.Add(new ValidationCriterion(inputFile[index]));
                index++;
            }
            index++; // skip blank line
            index++; // skip "your ticket" line
            string myTicketValues = inputFile[index];
            index++; // next line...
            index++; // skip blank line
            index++; // skip "nearby tickets" line
            List<List<int>> otherTickets = new List<List<int>>();
            List<int> helper;
            while (index < inputFile.Count)
            {
                helper = new List<int>();
                inputFile[index].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => helper.Add(int.Parse(x)));
                otherTickets.Add(helper);
                index++;
            }
            helper = new List<int>();
            //might as well get mine, too. Leave it in "helper" for now. Dunno what to do with it yet. 
            myTicketValues.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(x => helper.Add(int.Parse(x)));

            List<int> invalidFieldValues = new List<int>();
            List<int> badTicketIndices = new List<int>();
            index = 0;
            otherTickets.ForEach(ticket => {
                ticket.ForEach(ticketField =>
                {
                    bool isStillValidForSomething = false;
                    validators.ForEach(validator =>
                    {
                        if (validator.IsValidForThisCriterion(ticketField))
                            isStillValidForSomething = true;
                    });
                    if (!isStillValidForSomething)
                    {
                        invalidFieldValues.Add(ticketField);
                        badTicketIndices.Add(index);
                    }
                });
                index++;
            });
            AddResult("Sum of all invalid field values: " + invalidFieldValues.Sum().ToString());
            ResetProcessTimer(true);
            badTicketIndices.Reverse(); // always remove objects by index in highest-to-lowest index order...
            badTicketIndices.ForEach(badTicketIndex => otherTickets.RemoveAt(badTicketIndex));
            Dictionary<int, List<string>> validFieldNamesForEachFieldIndex = new Dictionary<int, List<string>>();
            List<string> allValidatorNames = new List<string>();
            validators.ForEach(validator => allValidatorNames.Add(validator.fieldName));
            index = 0;
            // use the first ticket as a template. All indices start with all field names possible. Assign the full list of field names as possible for each index. 
            otherTickets[0].ForEach(ticketField => { validFieldNamesForEachFieldIndex.Add(index, allValidatorNames.ToList()); index++; });
            // we will scan all fields on all tickets and remove potential field name matches for that index if any ticket has a field value in that index that is invalid for that field name.
            otherTickets.ForEach(ticket => {
                int fieldIndex = 0;
                ticket.ForEach(ticketField =>
                {
                    validators.ForEach(validator =>
                    {
                        if (!validator.IsValidForThisCriterion(ticketField))
                        {
                            validFieldNamesForEachFieldIndex[fieldIndex].Remove(validator.fieldName);
                        }
                    });
                    fieldIndex++;
                });
            });
            // this leaves us with many field indices that had values that were valid for multiple field names
            // But, as with all the puzzles, there is always a way. There is a field index that has only one field name that it matches.
            // so, if we remove that field name from all OTHER index possibilities, it will whittle down until we've removed all the non-matches.
            // so, we iterate until all field indices have only one possibility.
            while (validFieldNamesForEachFieldIndex.Values.Count(valueSet => valueSet.Count > 1)>0)
            {
                List<List<string>> orderedValueSets = validFieldNamesForEachFieldIndex.Values.OrderBy(valueSet => valueSet.Count).ToList();
                orderedValueSets.ForEach(set => {
                    if (set.Count == 1) // this set has a single possible name, so we should remove it from all other dictionary entries
                    {// remove this name from all other dictionary entries.
                        validFieldNamesForEachFieldIndex.Keys.ToList().ForEach(key => {
                            if (validFieldNamesForEachFieldIndex[key].Count>1)
                                validFieldNamesForEachFieldIndex[key].Remove(set[0]);
                        });
                    }
                });
            }
            // now we can set all of our validator indices to match the name.
            validFieldNamesForEachFieldIndex.Keys.ToList().ForEach(key => {
                validators.Where(validator => validator.fieldName == validFieldNamesForEachFieldIndex[key][0]).First().ticketFieldIndex = key;
            });
            long result = 1; // start with 1 since we are multiplying. 
            validators.Where(validator => validator.fieldName.StartsWith("departure")).ToList().ForEach(departure => {
                result *= helper[departure.ticketFieldIndex]; // we loaded "helper" with "my ticket" values up at initial data load, and never touched it again, so it's legit.
            });
            AddResult("My ticket departure fields product: " + result.ToString());
        }
    }
    class ValidationCriterion
    {
        public string fieldName { get; set; }
        public int[,] ranges { get; set; }
        public int ticketFieldIndex { get; set; }
        public ValidationCriterion(string definitionLine)
        {
            string[] pieces = definitionLine.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            fieldName = pieces[0];
            pieces = pieces[1].Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            ranges = new int[2, 2] { { int.Parse(pieces[0]), int.Parse(pieces[1]) }, { int.Parse(pieces[3]), int.Parse(pieces[4]) } };
            ticketFieldIndex = -1;
        }
        public bool IsValidForThisCriterion(int testValue)
        {
            return (ranges[0, 0] <= testValue && testValue <= ranges[0, 1]) || (ranges[1, 0] <= testValue && testValue <= ranges[1, 1]);
        }
    }
}
