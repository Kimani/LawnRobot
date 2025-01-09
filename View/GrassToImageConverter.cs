using LawnRobot.Model;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace LawnRobot.View
{
    internal class GrassToImageConverter : IValueConverter
    {
        public ImageSource CutGrassSource  { get; set; }
        public ImageSource TallGrassSource { get; set; }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            LawnDisplayType? displayType = value as LawnDisplayType?;
            return ((displayType != null) && (displayType.Value == LawnDisplayType.ShortGrass)) ? CutGrassSource : TallGrassSource;
        }
    }
} 
