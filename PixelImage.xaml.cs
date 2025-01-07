using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LawnRobot
{
    public sealed partial class PixelImage : UserControl
    {
        public static readonly DependencyProperty ImageUriProperty =
         DependencyProperty.Register("ImageUri", typeof(Uri), typeof(TiledImageControl), new PropertyMetadata(null));

        public Uri ImageUri
        {
            get { return (Uri)GetValue(ImageUriProperty); }
            set
            {
                SetValue(ImageUriProperty, value);
                UpdateImageSource();
            }
        }

        public PixelImage()
        {
            this.InitializeComponent();
        }

        private async void UpdateImageSource()
        {
            ImageSource source = await SourceFromUriPixeled(ImageUri);
            RootImage.Source = source;
        }

        private async Task<ImageSource> SourceFromUriPixeled(Uri sourceUri)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);

            // Open a stream for the file
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                // Create the BitmapDecoder
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                // You can now use the decoder to access the image data
                var transform = new BitmapTransform();
                transform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;
                transform.ScaledWidth = (uint)decoder.PixelWidth;
                transform.ScaledHeight = (uint)decoder.PixelHeight;

                // load the bitmap & bitmap source using transform
                using var bmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);
                var source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(bmp);
                return source;
            }
        }
    }
}
