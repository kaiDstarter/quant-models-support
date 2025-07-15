using Microsoft.Data.Analysis;
using QUANT.PATTERNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QUANT.PATTERNS
{
    public static class Extensions
    {
        public static DataFrame LoadFromOHCLVList(this DataFrame df, List<OHCLV> ohclvList)
        {
            Dictionary<string, Type> types = new Dictionary<string, Type>();
            types.Add("Time", typeof(long));
            types.Add("Open", typeof(decimal));
            types.Add("High", typeof(decimal));
            types.Add("Low", typeof(decimal));
            types.Add("Close", typeof(decimal));
            types.Add("Volume", typeof(decimal));
            types.Add("TimeFrame", typeof(string));

            var typeList = types.Select(x => (x.Key, x.Value)).ToArray();
            List<IList<object>> lists = new List<IList<object>>();
            foreach (var item in ohclvList)
            {
                object[] objects = new object[] { item.time, item.open, item.high, item.low, item.close, item.volume, item.timeFrame ?? "" };
                lists.Add(objects);
            }
            df = DataFrame.LoadFrom(lists, typeList);
            return df;
        }
    }
}
