namespace FinancesBlazor.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime PreviousMonthFirstDay(this DateTime dateTime)
        {
            var month = new DateTime(dateTime.Year, dateTime.Month, 1);
            return month.AddMonths(-1);
        }

        public static DateTime PreviousMonthLastDay(this DateTime dateTime)
        {
            var month = new DateTime(dateTime.Year, dateTime.Month, 1);
            return month.AddDays(-1);
        }

        public static DateTime CurrentMonthFirstDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime CurrentMonthLastDay(this DateTime dateTime)
        {
            var month = new DateTime(dateTime.Year, dateTime.Month, 1);
            return month.AddMonths(1).AddDays(-1);
        }

        public static DateTime PreviousYearFirstDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1).AddYears(-1);
        }

        public static DateTime PreviousYearLastDay(this DateTime dateTime)
        {
            var month = new DateTime(dateTime.Year, 1, 1);
            return month.AddDays(-1);
        }

        public static DateTime CurrentYearFirstDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        public static DateTime CurrentYearLastDay(this DateTime dateTime)
        {
            var month = new DateTime(dateTime.Year, 1, 1);
            return month.AddYears(1).AddDays(-1);
        }
    }
}
