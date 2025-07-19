using Microsoft.Data.Analysis;
using QUANT.PATTERNS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUANT.PATTERNS.TradingView
{
    public interface ITradingViewDraw
    {
        /// <summary>
        /// Vẽ market struct HH, HL, LH,LL
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        List<Shape> DrawMarketStruct(DataFrame df);
        /// <summary>
        /// Vẽ bos, choch
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        List<Shape> DrawBosAndChoCh(DataFrame df);
    }
}
