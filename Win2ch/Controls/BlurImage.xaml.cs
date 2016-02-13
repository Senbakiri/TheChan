using System;
using System.Numerics;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Win2ch.Controls {
    public sealed partial class BlurImage {

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            "BlurRadius", typeof (int), typeof (BlurImage), new PropertyMetadata(10));

        public int BlurRadius {
            get { return (int) GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register(
            "ImageUrl", typeof (string), typeof (BlurImage), new PropertyMetadata(default(string)));

        public string ImageUrl {
            get { return (string) GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        public BlurImage() {
            InitializeComponent();
        }

        private bool _isLoaded;
        private GaussianBlurEffect _blurEffect;
        private ScaleEffect _scaleEffect;
        private CanvasBitmap _imageBitmap;

        private void CanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args) {
            if (!_isLoaded)
                return;

            using (var session = args.DrawingSession) {
                session.Units = CanvasUnits.Pixels;

                var displayScaling = DisplayInformation.GetForCurrentView().LogicalDpi / 96.0;

                var pixelWidth = sender.ActualWidth * displayScaling;

                var scalefactor = pixelWidth / _imageBitmap.Size.Width;

                if (Math.Abs(scalefactor) < 0.001)
                    return;

                _scaleEffect.Source = _imageBitmap;
                _scaleEffect.Scale = new Vector2 {
                    X = (float)scalefactor,
                    Y = (float)scalefactor
                };

                _blurEffect.Source = _scaleEffect;
                _blurEffect.BlurAmount = BlurRadius;

                session.DrawImage(_blurEffect, 0.0f, 0.0f);
            }
        }

        private async void CanvasControl_OnCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args) {
            if (string.IsNullOrWhiteSpace(ImageUrl))
                return;

            _blurEffect = new GaussianBlurEffect();
            _scaleEffect = new ScaleEffect();
            _imageBitmap = await CanvasBitmap.LoadAsync(sender.Device, new Uri(ImageUrl));
            _isLoaded = true;
            sender.Invalidate();
        }
    }
}
