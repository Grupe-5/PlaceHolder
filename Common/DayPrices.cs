namespace Common
{
    public struct DayPrices : IComparable<DayPrices>
    {
        public DateTime Date { get; }
        public IList<Double> HourlyPrices { get; }

        public DayPrices(DateTime date, Double[] prices)
        {
            if (prices == null || prices.Length != 24)
            {
                throw new ArgumentException("Prices should have be non-null and have a length of 24!");
            }

            Date = date.Date;
            HourlyPrices = new List<Double>(prices);
        }

        /* Compares DayPrice dates */
        public int CompareTo(DayPrices other)
        {
            if (this.Date < other.Date)
            {
                return 1;
            }
            else if (this.Date > other.Date)
            {
                return -1;
            }

            return 0;
        }

        /* Returns true if the prices between two objects are the same */
        public bool HasSamePrices(DayPrices other)
        {
            return this.HourlyPrices.SequenceEqual(other.HourlyPrices);
        }
    }
}