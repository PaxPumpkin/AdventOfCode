using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class MessingAroundWithYield : AoCodeModule
    {
        public MessingAroundWithYield()
        {

        }
        
        public override void DoProcess()
        {

            int num = 0;
            foreach (var itm in IterateOverItems())
            {
                num++;
                if (num == 5)
                    break;
            }
            num = 0;
            foreach (var itm in IterateOverItems())
            {
                num++;
                if (num == 5)
                    break;
            }
            object[] sample1 = new object[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            object[] sample2 = new object[] { 11, 12, 13, 14, 15 };
            List<object> lista = new List<object>(sample1);
            List<object> listb = new List<object>(sample2);
            foreach (var q in EfficientMerge(lista, listb))
            {
                FinalOutput.Add(q.ToString());
            }
        }


        public IEnumerable<object> IterateOverItems() {
            for (int i=0; i < 100000; ++i)
                yield return i;
        }
        IEnumerable<object> EfficientMerge(List<object> list1, List<object> list2)
        {
            foreach (var o in list1)
                yield return o;
            foreach (var o in list2)
                yield return o;
        }
    }
}
