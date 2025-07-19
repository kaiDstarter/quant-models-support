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
        public PatternBase()
        {
        }
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

        private List<decimal> RMA(List<decimal> values, int period)
        {
            var result = new List<decimal>();
            decimal prevRma = 0;

            for (int i = 0; i < values.Count; i++)
            {
                if (i < period - 1)
                {
                    result.Add(0);
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

        public PrimitiveDataFrameColumn<bool> PivotHigh(DataFrame df, int left = 5, int right = 5)
        {
            var highs = df["High"] as PrimitiveDataFrameColumn<decimal>;
            if (highs == null) return new PrimitiveDataFrameColumn<bool>("PIVOT_HIGH");
            int n = highs.Count();
            bool[] result = new bool[n];

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
                result[i] = isPivot;
            }
            return new PrimitiveDataFrameColumn<bool>("PIVOT_HIGH", result);
        }

        public PrimitiveDataFrameColumn<bool> PivotLow(DataFrame df, int left = 5, int right = 5)
        {
            var lows = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            if (lows == null) return new PrimitiveDataFrameColumn<bool>("PIVOT_LOW");
            int n = lows.Count();
            bool[] result = new bool[n];

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

                result[i] = isPivot;
            }
            return new PrimitiveDataFrameColumn<bool>("PIVOT_LOW", result);
        }

        public DataFrame DetectMarketStructure(DataFrame df, int swingLookback = 5)
        {
            //df = df.Clone(); // Đảm bảo không làm thay đổi DataFrame gốc

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
                bool? hasHigh = swingHighCol[i];
                bool? hasLow = swingLowCol[i];
                // Nếu là SwingHigh
                if (hasHigh.HasValue && hasHigh.Value)
                {
                    if (lastHigh == null || high[i] > lastHigh)
                        label = "HH";
                    else
                        label = "LH";
                    lastHigh = high[i];
                }
                // Nếu là SwingLow
                else if (hasLow.HasValue && hasLow.Value)
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

        public DataFrame DetectPointDrawBosChoCh(DataFrame df, int swingLookback = 5)
        {
            var structure = df.Columns.Any(x => x.Name.Equals("Structure")) ? df["Structure"] as StringDataFrameColumn : null;
            var closes = df.Columns.Any(x => x.Name.Equals("Close")) ? df["Close"] as DecimalDataFrameColumn : null;
            var highs = df.Columns.Any(x => x.Name.Equals("High")) ? df["High"] as DecimalDataFrameColumn : null;
            var lows = df.Columns.Any(x => x.Name.Equals("Low")) ? df["Low"] as DecimalDataFrameColumn : null;
            var times = df.Columns.Any(x => x.Name.Equals("Time")) ? df["Time"] as PrimitiveDataFrameColumn<long> : null;
            if (structure == null || closes == null || highs == null || lows == null || times == null)
                return df;
            int n = structure.Count();
            var points = new PrimitiveDataFrameColumn<long>("BosChoChPoint", n);
            for (int i = 0; i < n; i++)
            {
                var label = structure[i];
                if (string.IsNullOrWhiteSpace(label)) continue;

                int index = -1;
                var high = highs[i];
                var low = lows[i];

                var filters = closes.ToList().Skip(i + 5).Take(35).ToList();

                //nếu đỉnh là tăng thì tìm point là thân nến > đỉnh này
                if (label == "HH" || label == "LH")
                {
                    
                    index = filters.FindIndex(0, x => x >= high);
                }
                else if (label == "HL" || label == "LL")
                {
                    index = filters.FindIndex(0, x => x <= low);
                }
                if (index < 0) continue;
                points[i] = times[index+5+i];
            }
            df.Columns.Add(points);
            return df;
        }

        public DataFrame DetectBosAndChoch(DataFrame df, bool useVolume = false, int volumeLookback = 20, decimal volumeThreshold = 1.2M)
        {
            var structure = df["Structure"] as StringDataFrameColumn;
            if (structure == null)
                return df;

            var bosCol = new StringDataFrameColumn("BOS", df.Rows.Count);
            var chochCol = new StringDataFrameColumn("CHoCH", df.Rows.Count);

            PrimitiveDataFrameColumn<decimal>? volume = null;
            if (useVolume)
                volume = df["Volume"] as PrimitiveDataFrameColumn<decimal>;

            //if not exist volume column, ignore useVolume
            if (useVolume && volume == null)
                useVolume = false;

            string? trend = null;
            int? lastHighIdx = null, lastLowIdx = null;
            for (int i = 0; i < df.Rows.Count; i++)
            {
                string label = structure[i];
                string bos = "", choch = "";

                bool volumeOk = true;
                if (useVolume && volume != null && i > 0)
                {
                    int start = Math.Max(0, i - volumeLookback);
                    decimal avgVol = 0;
                    int count = 0;
                    for (int k = start; k < i; k++)
                    {
                        avgVol += volume[k] ?? 0;
                        count++;
                    }
                    avgVol = count > 0 ? avgVol / count : 0;

                    // Kiểm tra điều kiện volume
                    volumeOk = avgVol > 0 && volume[i] > avgVol * volumeThreshold;
                }

                // BOS
                if (label == "HH" && trend == "up" && lastHighIdx != null)
                    if (!useVolume || volumeOk)
                        bos = "BOS";
                if (label == "LL" && trend == "down" && lastLowIdx != null)
                    if (!useVolume || volumeOk)
                        bos = "BOS";

                // CHoCH
                if (label == "LL" && trend == "up")
                    if (!useVolume || volumeOk)
                        choch = "CHoCH";
                if (label == "HH" && trend == "down")
                    if (!useVolume || volumeOk)
                        choch = "CHoCH";

                // Cập nhật trend và pivot gần nhất
                if (label == "HH" || label == "LH")
                {
                    trend = "up";
                    if (label == "HH") lastHighIdx = i;
                }
                else if (label == "LL" || label == "HL")
                {
                    trend = "down";
                    if (label == "LL") lastLowIdx = i;
                }

                bosCol[i] = bos;
                chochCol[i] = choch;
            }

            df.Columns.Add(bosCol);
            df.Columns.Add(chochCol);
            return df;
        }

        public DataFrame DetectBosAndChoch(DataFrame df)
        {
            if (!df.Columns.Any(x => x.Name.Contains("Structure"))) return df;

            var structureCol = df["Structure"] as StringDataFrameColumn;
            if (structureCol == null) return df;
            int n = df.Rows.Count();
            var signalCol = new StringDataFrameColumn("Signal", n);

            string? prevTrend = null;

            for (int i = 1; i < n; i++)
            {
                string s = structureCol[i]?.ToString() ?? "";
                // Xác định trend hiện tại
                string currentTrend =
                    (s == "HH" || s == "LH") ? "up" :
                    (s == "LL" || s == "HL") ? "down" :
                    prevTrend;

                // Nếu đổi hướng => CHoCH
                if (!string.IsNullOrEmpty(prevTrend) && currentTrend != null && currentTrend != prevTrend)
                    signalCol[i] = "CHoCH";
                // Nếu tiếp tục hướng cũ => BOS
                else if (!string.IsNullOrEmpty(prevTrend) && currentTrend != null && currentTrend == prevTrend)
                    signalCol[i] = "BOS";
                else
                    signalCol[i] = "";

                prevTrend = currentTrend;
            }

            df.Columns.Add(signalCol);
            return df;
        }
        public DataFrame DetectOrderBlocks(DataFrame df, int atrPeriod = 14, decimal multiplier = 2M)
        {
            // Chuẩn bị các cột cần thiết
            var highCol = df["High"] as PrimitiveDataFrameColumn<decimal>;
            var lowCol = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            var closeCol = df["Close"] as PrimitiveDataFrameColumn<decimal>;
            var openCol = df["Open"] as PrimitiveDataFrameColumn<decimal>;
            if (highCol == null || lowCol == null || closeCol == null || openCol == null)
                return df;

            int n = df.Rows.Count();

            // Tính ATR
            var atrList = ATR(df, atrPeriod);

            // Tạo cột OrderBlock & OBNitigated
            var obCol = new StringDataFrameColumn("OrderBlock", n);
            var obMitigatedCol = new BooleanDataFrameColumn("OBMitigated", n);

            // PHÁT HIỆN ORDER BLOCK
            for (int i = 1; i < n; i++)
            {
                var prevOpen = openCol[i - 1];
                var prevClose = closeCol[i - 1];
                var prevHigh = highCol[i - 1];
                var prevLow = lowCol[i - 1];
                var currOpen = openCol[i];
                var currClose = closeCol[i];
                var currHigh = highCol[i];
                var currLow = lowCol[i];
                var atr = atrList[i] ?? 0;

                // Bullish OB: nến giảm → nến tăng vượt đỉnh, biên độ vừa phải
                if (prevClose < prevOpen && currClose > currOpen)
                {
                    if (currHigh > prevHigh && (currHigh - currLow) < multiplier * atr)
                        obCol[i - 1] = "Bullish OB";
                }
                // Bearish OB: nến tăng → nến giảm phá đáy, biên độ vừa phải
                else if (prevClose > prevOpen && currClose < currOpen)
                {
                    if (currLow < prevLow && (currHigh - currLow) < multiplier * atr)
                        obCol[i - 1] = "Bearish OB";
                }
            }

            // KIỂM TRA MITIGATED
            for (int i = 0; i < n - 1; i++)
            {
                if (obCol[i] == "Bullish OB")
                {
                    decimal obLow = lowCol[i] ?? 0;
                    bool mitigated = false;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (lowCol[j] < obLow)
                        {
                            mitigated = true;
                            break;
                        }
                    }
                    obMitigatedCol[i] = mitigated;
                }
                else if (obCol[i] == "Bearish OB")
                {
                    decimal obHigh = highCol[i] ?? 0;
                    bool mitigated = false;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (highCol[j] > obHigh)
                        {
                            mitigated = true;
                            break;
                        }
                    }
                    obMitigatedCol[i] = mitigated;
                }
                else
                {
                    obMitigatedCol[i] = false;
                }
            }

            // Thêm kết quả vào DataFrame
            df.Columns.Add(obCol);
            df.Columns.Add(obMitigatedCol);
            return df;
        }

        public DataFrame DetectFVG(DataFrame df)
        {
            var lowCol = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            var highCol = df["High"] as PrimitiveDataFrameColumn<decimal>;
            if (lowCol == null || highCol == null)
                return df;
            int n = df.Rows.Count();
            var fvgCol = new StringDataFrameColumn("FVG", n);

            for (int i = 2; i < n; i++)
            {
                // Bullish FVG: đáy hiện tại > đỉnh 2 nến trước
                if (lowCol[i] > highCol[i - 2])
                {
                    fvgCol[i] = "Bullish FVG";
                }
                // Bearish FVG: đỉnh hiện tại < đáy 2 nến trước
                else if (highCol[i] < lowCol[i - 2])
                {
                    fvgCol[i] = "Bearish FVG";
                }
                else
                {
                    fvgCol[i] = "";
                }
            }

            // 2 nến đầu mặc định không thể có FVG
            fvgCol[0] = "";
            fvgCol[1] = "";

            df.Columns.Add(fvgCol);
            return df;
        }
        public DataFrame DetectEQHL(DataFrame df, decimal thresholdPct = 0.1M, int atrPeriod = 14)
        {
            var highCol = df["High"] as PrimitiveDataFrameColumn<decimal>;
            var lowCol = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            var closeCol = df["Close"] as PrimitiveDataFrameColumn<decimal>;

            if (highCol == null || lowCol == null || closeCol == null)
                return df;

            int n = df.Rows.Count();

            // Tính ATR (dùng hàm ATR trước đây)
            var atrVals = ATR(df, atrPeriod);

            // Tạo cột EQH/EQL
            var eqhCol = new BooleanDataFrameColumn("EQH", new bool[n]);
            var eqlCol = new BooleanDataFrameColumn("EQL", new bool[n]);

            for (int i = 1; i < n; i++)
            {
                var atr = atrVals[i] ?? 0;
                // EQH: chênh lệch high nhỏ hơn ngưỡng ATR * thresholdPct
                if (Math.Abs((highCol[i] ?? 0) - (highCol[i - 1] ?? 0)) <= atr * thresholdPct)
                    eqhCol[i] = true;
                else
                    eqhCol[i] = false;

                // EQL: chênh lệch low nhỏ hơn ngưỡng ATR * thresholdPct
                if (Math.Abs((lowCol[i] ?? 0) - (lowCol[i - 1] ?? 0)) <= atr * thresholdPct)
                    eqlCol[i] = true;
                else
                    eqlCol[i] = false;
            }

            df.Columns.Add(eqhCol);
            df.Columns.Add(eqlCol);
            return df;
        }

        public DataFrame ComputePremiumDiscount(DataFrame df, int window = 50)
        {
            var highCol = df["High"] as PrimitiveDataFrameColumn<decimal>;
            var lowCol = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            var closeCol = df["Close"] as PrimitiveDataFrameColumn<decimal>;
            if (highCol == null || lowCol == null || closeCol == null)
                return df;
            int n = df.Rows.Count();

            // Tạo danh sách rolling max/min
            var highest = new List<decimal>();
            var lowest = new List<decimal>();
            for (int i = 0; i < n; i++)
            {
                int start = Math.Max(0, i - window + 1);
                decimal maxHigh = highCol.Skip(start).Take(i - start + 1).Max() ?? 0;
                decimal minLow = lowCol.Skip(start).Take(i - start + 1).Min() ?? 0;
                highest.Add(maxHigh);
                lowest.Add(minLow);
            }

            var zoneCol = new StringDataFrameColumn("Zone", n);

            for (int i = 0; i < n; i++)
            {
                var high = highest[i];
                var low = lowest[i];
                // Gần đỉnh → Premium
                if (closeCol[i] >= high * 0.95M)
                    zoneCol[i] = "Premium";
                // Gần đáy → Discount
                else if (closeCol[i] <= low * 1.05M)
                    zoneCol[i] = "Discount";
                else
                    zoneCol[i] = "Equilibrium";
            }

            df.Columns.Add(zoneCol);
            return df;
        }

        public DataFrame LabelStrongWeak(DataFrame df)
        {
            var highCol = df["High"] as PrimitiveDataFrameColumn<decimal>;
            var lowCol = df["Low"] as PrimitiveDataFrameColumn<decimal>;
            var structureCol = df["Structure"] as StringDataFrameColumn;

            if (highCol == null || lowCol == null || structureCol == null)
                return df;

            int n = df.Rows.Count();

            var swingStrengthCol = new StringDataFrameColumn("SwingStrength", n);

            for (int i = 1; i < n - 5; i++)
            {
                var structure = structureCol[i];
                if (structure == "HH" || structure == "LH")
                {
                    // Kiểm tra 4 nến tiếp theo
                    decimal futureMax = highCol.Skip(i + 1).Take(4).Max() ?? 0;
                    swingStrengthCol[i] = (futureMax < highCol[i]) ? "Strong High" : "Weak High";
                }
                else if (structure == "LL" || structure == "HL")
                {
                    decimal futureMin = lowCol.Skip(i + 1).Take(4).Min() ?? 0;
                    swingStrengthCol[i] = (futureMin > lowCol[i]) ? "Strong Low" : "Weak Low";
                }
                else
                {
                    swingStrengthCol[i] = "";
                }
            }
            // Các nến đầu/cuối không xét được đủ future, gán rỗng
            for (int i = n - 5; i < n; i++) swingStrengthCol[i] = "";

            df.Columns.Add(swingStrengthCol);
            return df;
        }

        public DataFrame ApplyConfluenceFilter(DataFrame df)
        {
            var openCol = df["Open"] as PrimitiveDataFrameColumn<decimal>;
            var closeCol = df["Close"] as PrimitiveDataFrameColumn<decimal>;
            var highCol = df["High"] as PrimitiveDataFrameColumn<decimal>;
            var lowCol = df["Low"] as PrimitiveDataFrameColumn<decimal>;

            if (openCol == null || closeCol == null || highCol == null || lowCol == null)
                return df;

            int n = df.Rows.Count();

            var bodySizeCol = new DecimalDataFrameColumn("BodySize", n);
            var wickSizeCol = new DecimalDataFrameColumn("WickSize", n);
            var confluenceCol = new BooleanDataFrameColumn("Confluence", n);

            for (int i = 0; i < n; i++)
            {
                decimal bodySize = Math.Abs((closeCol[i] ?? 0) - (openCol[i] ?? 0));
                decimal wickSize = ((highCol[i] ?? 0) - (lowCol[i] ?? 0)) - bodySize;
                bodySizeCol[i] = bodySize;
                wickSizeCol[i] = wickSize;
                confluenceCol[i] = bodySize > wickSize;
            }

            df.Columns.Add(bodySizeCol);
            df.Columns.Add(wickSizeCol);
            df.Columns.Add(confluenceCol);
            return df;
        }

        public DataFrame DetectMtfFvgOb(DataFrame df, DataFrame tfDf)
        {
            // Phát hiện FVG và Order Block ở timeframe lớn hơn
            tfDf = DetectFVG(tfDf);
            tfDf = DetectOrderBlocks(tfDf);

            // Lấy FVG, OB mới nhất (dòng cuối cùng) từ tfDf

            string lastFvg = tfDf.Columns.Any(x => x.Name.Contains("FVG")) && tfDf.Rows.Count > 0
                ? tfDf["FVG"][tfDf.Rows.Count - 1]?.ToString() ?? ""
                : "";

            string lastOb = tfDf.Columns.Any(x => x.Name.Contains("OrderBlock")) && tfDf.Rows.Count > 0
                ? tfDf["OrderBlock"][tfDf.Rows.Count - 1]?.ToString() ?? ""
                : "";

            // Gán cho toàn bộ DataFrame của timeframe nhỏ
            var mtfFvgCol = new StringDataFrameColumn("MTF_FVG", df.Rows.Count);
            var mtfObCol = new StringDataFrameColumn("MTF_OB", df.Rows.Count);

            for (int i = 0; i < df.Rows.Count; i++)
            {
                mtfFvgCol[i] = lastFvg;
                mtfObCol[i] = lastOb;
            }

            df.Columns.Add(mtfFvgCol);
            df.Columns.Add(mtfObCol);
            return df;
        }
        #endregion

    }
}
