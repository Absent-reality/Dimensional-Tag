
namespace DimensionalTag.Converters
{
    public class ByteArrayToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
    //        if (value is not byte[] || value is not string || value is not byte ) { return value; }

            if (value is byte[] bytes )
            {
                var array = BitConverter.ToString(bytes);
                return (string)array;
            }
            else if (value is byte aByte)
            {
                var array = $"{aByte}";
                return array;
            }
            else
            {
                return value;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotSupportedException();
        }
    }
    
}
