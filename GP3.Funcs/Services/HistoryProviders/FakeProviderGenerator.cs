using System;

namespace GP3.Funcs.Services.HistoryProviders
{
    public static class FakeProviderGenerator
    {
        public static double[] meanKwh = new double[]
        {
            0.4, 0.3, 0.2, 0.2, 0.2, 0.2, 0.2, 0.3, 0.3, 0.6, 0.6, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5,
            0.6, 0.8, 0.9, 1.0, 1.0, 0.9, 0.6
        };

        public static double GetCurrentDraw()
        {
            return meanKwh[DateTime.Now.Hour] + ((0.2 * Random.Shared.NextDouble()) - 0.1);
        }

        private static double GetPartKwh()
        {
            int offset = DateTime.Now.Minute / 60;
            return meanKwh[DateTime.Now.Hour] * offset;
        }
        
        private static double GetDayUsage(DateTime time)
        {
            double total = 0;
            for (int i = 0; i < time.Hour; i++)
            {
                total += meanKwh[time.Hour];
            }
            total += GetPartKwh();
            return total;
        }

        private static double GetMonthUsage(DateTime time)
        {
            double total = 0;
            for(int i = 0; i < time.Day; i++)
            {
                total += GetDayUsage(time.AddDays((-1 * time.Day) + i));
            }
            return total;
        }

        public static double GetDailyUsage()
            => GetDayUsage(DateTime.Now);

        public static double GetMonthlyUsage()
            => GetMonthUsage(DateTime.Now);
    }
}
