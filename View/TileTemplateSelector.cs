using LawnRobot.Model;
using LawnRobot.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LawnRobot.View
{
    internal partial class TileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? GrassTemplate { get; set; }
        public DataTemplate? FenceTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is LawnNodeViewModel taskItem)
            {
                LawnDisplayType displayType = taskItem.DisplayType;
                if ((displayType == Model.LawnDisplayType.ShortGrass) ||
                    (displayType == Model.LawnDisplayType.TallGrass))
                {
                    return GrassTemplate;
                }

                if (displayType != Model.LawnDisplayType.Empty)
                {
                    return FenceTemplate;
                }
            }
            return null;
        }
    }
}
