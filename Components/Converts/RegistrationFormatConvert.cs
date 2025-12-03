using System.Globalization;

namespace Ixjok.Components.Converts;

public sealed class RegistrationFormatConvert : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    => value is not DateTimeOffset registration
            ? null
            : $"{registration.DayOfWeek.ToString()[..3]} {registration:dd/mm/yyyy '-' h:m:s}";
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => null;
}