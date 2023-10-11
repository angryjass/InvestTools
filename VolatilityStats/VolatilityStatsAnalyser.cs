using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VolatilityStats
{
    public class VolatilityStatsAnalyser
    {
        private readonly HttpClient client;

        public VolatilityStatsAnalyser()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://iss.moex.com/iss/history/engines/stock/markets/shares/securities/");
        }

        public async Task<double> GetVolatilityAsync(PeriodEnum period, string ticker, CancellationToken token)
        {
            var dates = GetDatesByPeriod(period);
            var closeValues = new List<double>();
            var start = 0;
            while (true)
            {
                var req = new HttpRequestMessage(HttpMethod.Get, ticker + ".json?from=" + dates[0] + "&till=" + dates[1] + "&start=" + start);
                var response = await client.SendAsync(req, token);
                var jobj = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync(token));

                var valuesArr = (jobj.GetValue("history") as JObject).GetValue("data") as JArray;

                if (valuesArr == null || valuesArr.Count == 0)
                    break;

                foreach (var row in (jobj.GetValue("history") as JObject).GetValue("data") as JArray)
                {
                    if (row[0].ToString() != BoardsEnum.TQBR.ToString())
                        continue;

                    closeValues.Add((double)row[11]);
                }

                start += 100;
            }

            return Math.Round(GetVolatilityValue(closeValues.ToArray()), 2);
        }

        private double GetVolatilityValue(double[] values)
        {
            var sum = 0d;
            for(var i = 1; i != values.Length; i++)
            {
                var valueInPercents = ((values[i] / values[i - 1]) - 1) * 100;
                valueInPercents *= valueInPercents;
                sum += valueInPercents;
            }
            return Math.Sqrt(sum / values.Length);
        }

        private string[] GetDatesByPeriod(PeriodEnum period)
        {
            if (period == PeriodEnum.Year)
            {
                return new[] { DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") };
            }
            else if (period == PeriodEnum.Quartal)
            {
                return new[] { DateTime.Now.AddMonths(-4).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") };
            }
            else if (period == PeriodEnum.Month)
            {
                return new[] {DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") };
            }
            else if (period == PeriodEnum.Week)
            {
                return new[] {DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") };
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}