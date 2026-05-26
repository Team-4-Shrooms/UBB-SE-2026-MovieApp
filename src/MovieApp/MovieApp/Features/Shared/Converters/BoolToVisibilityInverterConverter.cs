using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MovieApp.Features.Shared.Converters
{
    public sealed class BoolToVisibilityInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is true ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
