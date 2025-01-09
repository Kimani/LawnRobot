using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace LawnRobot.View
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? boolValue = value as bool?;
            return (boolValue.HasValue && boolValue.Value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
