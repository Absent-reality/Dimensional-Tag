
namespace DimensionalTag.Converters
{
    public class ByteArrayToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
    //        if (value is not byte[] || value is not string || value is not byte ) { return value; }
            switch (value)
            {
                case byte[] bytes:
                    var array = BitConverter.ToString(bytes);
                    return (string)array;

                case byte aByte:
                     var thisByte = $"{aByte}";
                     return thisByte;                
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    
}
