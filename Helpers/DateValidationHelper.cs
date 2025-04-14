using System;
using System.Globalization;

namespace FssRedact.Helpers
{
    public static class DateValidationHelper
    {
        public static string? ValidateSingleDate(string? date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return "Дата обязательна.";

            if (!TryParseDate(date, out var parsedDate))
                return "Неверный формат даты.";

            if (parsedDate.Year < 1990)
                return "Год не может быть меньше 1990.";

            if (parsedDate > DateTime.Today)
                return "Дата не может быть в будущем.";

            return null;
        }

        public static string? ValidateDateRange(string? beginDate, string? endDate)
        {
            if (TryParseDate(beginDate, out var begin) && TryParseDate(endDate, out var end))
            {
                if (end < begin)
                    return "Дата конца не может быть раньше даты начала.";
            }

            return null;
        }

        public static bool TryParseDate(string? date, out DateTime result)
        {
            return DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}
