using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using System;

namespace LawnRobot
{
    public sealed partial class TiledImageControl : UserControl
    {
        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(Uri), typeof(TiledImageControl), new PropertyMetadata(null));

        public Uri ImageUri
        {
            get { return (Uri)GetValue(ImageUriProperty); }
            set
            {
                SetValue(ImageUriProperty, value);
                _Dirty = true;
                CanvasRoot.Invalidate();
            }
        }

        private CanvasBitmap? _CanvasBitmap;
        private bool _Dirty = true;

        public TiledImageControl()
        {
            InitializeComponent();
        }

        private void OnCanvasCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            EnsureResources();
        }

        private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            EnsureResources();

            // Approach adapted from https://stackoverflow.com/questions/74527783/repeating-brush-or-tile-of-image-in-winui-3
            Rect sizeRect = RectHelper.FromCoordinatesAndDimensions(0, 0, 50, 50);
            var list = new CanvasCommandList(sender);
            var session = list.CreateDrawingSession();

            session.DrawImage(_CanvasBitmap, new System.Numerics.Vector2(), sizeRect, 1.0f, CanvasImageInterpolation.NearestNeighbor);

            using var tile = new TileEffect();
            Rect sizeRect2 = RectHelper.FromCoordinatesAndDimensions(0, 0, 50, 50);
            tile.Source = list;
            tile.SourceRectangle = sizeRect2;
            args.DrawingSession.DrawImage(tile);
        }

        private void EnsureResources()
        {
            if (ImageUri != null && (_CanvasBitmap == null || _Dirty))
            {
                // Approach adapted from https://stackoverflow.com/questions/74527783/repeating-brush-or-tile-of-image-in-winui-3
                var awaiter = CanvasBitmap.LoadAsync(CanvasRoot, ImageUri).GetAwaiter();
                _CanvasBitmap = awaiter.GetResult();
                _Dirty = true;
            }
        }
    }
}
