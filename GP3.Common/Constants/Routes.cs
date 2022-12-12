namespace GP3.Common.Constants
{
    public static class Routes
    {
        /* Admin API routes */
        public const string Admin = "funcAdmin";

        /* Price API routes */
        public const string Price = "price";
        public const string PriceOffset = "priceOffset";

        /* Integration API routes */
        public const string Integration = "integration";

        /* History API routes */
        public const string HistoryCurrentDraw = "history/currentDraw";
        public const string HistoryMonthlyUsage = "history/monthlyUsage";
        public const string HistoryDailyUsage = "history/dailyUsage";
        public const string HistoryRegisterProvider = "history/registerProvider";
        public const string HistoryIsRegistered = "history/isRegistered";
    }
}
