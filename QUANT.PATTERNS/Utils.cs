using Microsoft.Data.Analysis;
using QUANT.PATTERNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUANT.PATTERNS
{
    public class Utils
    {
        public static  bool IsUpMarket(string structure)
        {
            return (structure == Constants.HIGH_HIGH || structure == Constants.LOW_HIGH);
        }

        public static bool IsDownMarket(string structure)
        {
            return (structure == Constants.LOW_LOW || structure == Constants.HIGH_LOW);
        }
    }
}
