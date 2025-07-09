using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QUANT.PATTERNS.Enumerations;

namespace QUANT.PATTERNS.Base
{
    public class PatternBase : IPatternBase
    {
        #region "Contructor"
        #endregion

        #region "Declare/Property"
        #endregion

        #region "Private funtion"
        private List<decimal?> SMA(List<decimal> values, int period)
        {
            var result = new List<decimal?>();
            decimal sum = 0;
            for (int i = 0; i < values.Count; i++)
            {
                sum += values[i];
                if (i >= period)
                    sum -= values[i - period];
                if (i >= period - 1)
                    result.Add(sum / period);
                else
                    result.Add(null);
            }
            return result;
        }

        private List<decimal?> EMA(List<decimal> values, int period)
        {
            var result = new List<decimal?>();
            decimal? prevEma = null;
            decimal k = 2M / (period + 1);

            for (int i = 0; i < values.Count; i++)
            {
                if (i < period - 1)
                {
                    result.Add(null);
                    continue;
                }
                if (i == period - 1)
                {
                    decimal sum = 0;
                    for (int j = 0; j < period; j++)
                        sum += values[j];
                    prevEma = sum / period;
                }
                else
                {
                    prevEma = values[i] * k + prevEma * (1 - k);
                }
                result.Add(prevEma);
            }
            return result;
        }

        private List<decimal?> RMA(List<decimal> values, int period)
        {
            var result = new List<decimal?>();
            decimal? prevRma = null;

            for (int i = 0; i < values.Count; i++)
            {
                if (i < period - 1)
                {
                    result.Add(null);
                    continue;
                }

                if (i == period - 1)
                {
                    decimal sum = 0;
                    for (int j = 0; j < period; j++)
                        sum += values[j];
                    prevRma = sum / period;
                }
                else
                {
                    prevRma = (prevRma * (period - 1) + values[i]) / period;
                }
                result.Add(prevRma);
            }
            return result;
        }

        private List<decimal?> WMA(List<decimal> values, int period)
        {
            var result = new List<decimal?>();
            int weightSum = period * (period + 1) / 2;

            for (int i = 0; i < values.Count; i++)
            {
                if (i < period - 1)
                {
                    result.Add(null);
                    continue;
                }

                decimal wma = 0;
                for (int j = 0; j < period; j++)
                {
                    wma += values[i - j] * (period - j);
                }
                result.Add(wma / weightSum);
            }
            return result;
        }



        #endregion

        #region "Public funtion"
        public PrimitiveDataFrameColumn<decimal> ATR(DataFrame df, int period = 14, ATR_TYPE atrType = ATR_TYPE.SMA)
        {
            var high = df["High"] as PrimitiveDataFrameColumn<decimal>;
            var low = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            var close = df["Close"] as PrimitiveDataFrameColumn<decimal>;
            if (high == null || low == null || close == null)
                return new PrimitiveDataFrameColumn<decimal>("ATR");
            int n = df.Rows.Count();
            var trueRange = new List<decimal>();

            for (int i = 0; i < n; i++)
            {
                decimal highLow = (high[i] ?? 0) - (low[i] ?? 0);
                decimal highClosePrev = (i == 0) ? 0 : Math.Abs(
                    (high[i] ?? 0) -
                    (close[i - 1] ?? 0)
                );
                decimal lowClosePrev = (i == 0) ? 0 : Math.Abs(
                    (low[i] ?? 0) -
                    (close[i - 1] ?? 0)
                );
                decimal tr = (i == 0)
                    ? highLow
                    : Math.Max(highLow, Math.Max(highClosePrev, lowClosePrev));

                trueRange.Add(tr);
            }

            // Tính ATR bằng RMA/SMMA
            var atr = RMA(trueRange, period);

            // Chuyển về DataFrameColumn để trả về giống pandas
            var atrCol = new PrimitiveDataFrameColumn<decimal>("ATR", atr);
            return atrCol;
        }

        public PrimitiveDataFrameColumn<int> PivotHigh(DataFrame df, int left = 5, int right = 5)
        {
            var highs = df["High"] as PrimitiveDataFrameColumn<decimal>;
            if (highs == null) return new PrimitiveDataFrameColumn<int>("PIVOT_HIGH");
            int n = highs.Count();
            var result = new List<int>();

            for (int i = left; i < n - left; i++)
            {
                decimal current = highs[i] ?? 0;
                bool isPivot = true;

                // So sánh với N giá trị trước và sau
                for (int j = i - left; j <= i + right; j++)
                {
                    if (j == i) continue;
                    if (highs[j] >= current)
                    {
                        isPivot = false;
                        break;
                    }
                }

                if (isPivot)
                    result.Add(i);
            }
            return new PrimitiveDataFrameColumn<int>("PIVOT_HIGH", result);
        }

        public PrimitiveDataFrameColumn<int> PivotLow(DataFrame df, int left = 5, int right = 5)
        {
            var lows = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            if (lows == null) return new PrimitiveDataFrameColumn<int>("PIVOT_LOW");
            int n = lows.Count();
            var result = new List<int>();

            for (int i = left; i < n - left; i++)
            {
                decimal current = lows[i] ?? 0;
                bool isPivot = true;

                // So sánh với N giá trị trước và sau
                for (int j = i - left; j <= i + right; j++)
                {
                    if (j == i) continue;
                    if (lows[j] <= current)
                    {
                        isPivot = false;
                        break;
                    }
                }

                if (isPivot)
                    result.Add(i);
            }
            return new PrimitiveDataFrameColumn<int>("PIVOT_LOW", result);
        }
        public DataFrame DetectMarketStructure(DataFrame df, int swingLookback = 5)
        {
            df = df.Clone(); // Đảm bảo không làm thay đổi DataFrame gốc

            // Tìm swing pivot
            var swingHighCol = PivotHigh(df, swingLookback, swingLookback);
            var swingLowCol = PivotLow(df, swingLookback, swingLookback);
            swingHighCol.SetName("SwingHigh");
            swingLowCol.SetName("SwingLow");

            df.Columns.Add(swingHighCol);
            df.Columns.Add(swingLowCol);

            var structure = new List<string>();
            decimal? lastHigh = null, lastLow = null;

            var high = df["High"] as PrimitiveDataFrameColumn<decimal>;
            var low = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            if (high == null || low == null)
                return df; 
            for (int i = 0; i < df.Rows.Count; i++)
            {
                string label = "";

                // Nếu là SwingHigh
                if (swingHighCol[i].HasValue)
                {
                    if (lastHigh == null || high[i] > lastHigh)
                        label = "HH";
                    else
                        label = "LH";
                    lastHigh = high[i];
                }
                // Nếu là SwingLow
                else if (swingHighCol[i].HasValue)
                {
                    if (lastLow == null || low[i] < lastLow)
                        label = "LL";
                    else
                        label = "HL";
                    lastLow = low[i];
                }
                structure.Add(label);
            }

            df.Columns.Add(new StringDataFrameColumn("Structure", structure));
            return df;
        }

      

        #endregion

    }
}
