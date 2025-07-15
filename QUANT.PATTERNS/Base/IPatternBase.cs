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

        /// <summary>
        /// Hàm xác định bos, choch
        /// </summary>
        /// <param name="df"></param>
        /// <param name="useVolume"></param>
        /// <param name="volumeLookback"></param>
        /// <param name="volumeThreshold">mặc định 1,2 (Nghĩa là volume hiện tại phải lớn hơn 120% trung bình)</param>
        /// <returns></returns>
        DataFrame DetectBosAndChoch(DataFrame df, bool useVolume = false, int volumeLookback = 20, decimal volumeThreshold = 1.2M);

        /// <summary>
        /// Hàm xác định order block + mitigation
        /// </summary>
        /// <param name="df"></param>
        /// <param name="atrPeriod"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        DataFrame DetectOrderBlocks(DataFrame df, int atrPeriod = 14, decimal multiplier = 2M);

        /// <summary>
        /// Hàm xác định FVG
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        DataFrame DetectFVG(DataFrame df);

        /// <summary>
        /// Hàm xác định Equal High, Low
        /// </summary>
        /// <param name="df"></param>
        /// <param name="thresholdPct"></param>
        /// <param name="atrPeriod"></param>
        /// <returns></returns>
        DataFrame DetectEQHL(DataFrame df, decimal thresholdPct = 0.1M, int atrPeriod = 14);

        /// <summary>
        /// Hàm phân vùng Premium, Discount
        /// </summary>
        /// <param name="df"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        DataFrame ComputePremiumDiscount(DataFrame df, int window = 50);

        /// <summary>
        /// Hàm gắn nhãn Strong, Weak
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        DataFrame LabelStrongWeak(DataFrame df);

        /// <summary>
        /// Hàm áp dụng bộ lọc Confluence
        /// </summary>
        /// <param name="df"></param>
        /// <returns></returns>
        DataFrame ApplyConfluenceFilter(DataFrame df);

        /// <summary>
        /// Hàm xác định FVG, OB cho timeframe nhỏ
        /// </summary>
        /// <param name="df"></param>
        /// <param name="tfDf"></param>
        /// <returns></returns>
        DataFrame DetectMtfFvgOb(DataFrame df, DataFrame tfDf);
    }
}
