using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUANT.PATTERNS.Models
{
    internal class OHCLV
    {
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double volume { get; set; }
        public long time { get; set; }
    }
}
