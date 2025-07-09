using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QUANT.PATTERNS.Enumerations;

namespace QUANT.PATTERNS.Base
{
    public interface IPatternBase
    {
        /// <summary>
        /// Hàm tính trung bình động
        /// </summary>
        /// <param name="df">dataframe chứa dữ liệu high,low,close</param>
        /// <param name="period">mặc định 14</param>
        /// <param name="atrType">SMA, EMA ...</param>
        /// <returns></returns>
        PrimitiveDataFrameColumn<decimal> ATR(DataFrame df, int period = 14, ATR_TYPE atrType = ATR_TYPE.SMA);
        /// <summary>
        /// Hàm tìm đỉnh cục bộ
        /// </summary>
        /// <param name="df">Dataframe</param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        PrimitiveDataFrameColumn<int> PivotHigh(DataFrame df, int left = 5, int right = 5);
        /// <summary>
        /// hàm tìm đáy cục bộ
        /// </summary>
        /// <param name="df">Dataframe</param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        PrimitiveDataFrameColumn<int> PivotLow(DataFrame df, int left = 5, int right = 5);

        /// <summary>
        /// Hàm đánh dấu thị trường HH, LH, HL, LL
        /// </summary>
        /// <param name="df"></param>
        /// <param name="swingLookback"></param>
        /// <returns></returns>
        DataFrame DetectMarketStructure(DataFrame df, int swingLookback = 5);
        //void SupportZone();
        //void ResistanceZone();
        //void SupplyZone();
        //void DemandZone();
        //void Breakout();
        //void Fakeout();

    }
}
