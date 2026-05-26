using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace MovieApp.Features.Shared.Converters;

public sealed class IntToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int id)
        {
            return id > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        if (value is long longId)
        {
            return longId > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
