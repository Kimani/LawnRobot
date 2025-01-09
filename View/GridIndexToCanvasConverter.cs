using Microsoft.UI.Xaml.Data;
using System;

namespace LawnRobot.View
{
    internal class GridIndexToCanvasConverter : IValueConverter
    {
        public int GridSpan { get; set; }
        public int GridStart { get; set; }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int? intValue = value as int?;
            return intValue.HasValue ? ((intValue.Value - GridStart) * GridSpan) : 0;
        }
    }
}
