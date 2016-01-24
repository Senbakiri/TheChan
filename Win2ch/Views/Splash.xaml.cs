using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Win2ch.Views {
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplashScreen
    public sealed partial class Splash {
        public Splash(SplashScreen splashScreen) {
            InitializeComponent();

            Action resize = () => {
                if ((int)splashScreen.ImageLocation.Top == 0) {
                    MyImage.Visibility = Visibility.Collapsed;
                    return;
                } else {
                    MyCanvas.Background = null;
                    MyImage.Visibility = Visibility.Visible;
                }
                MyImage.Height = splashScreen.ImageLocation.Height;
                MyImage.Width = splashScreen.ImageLocation.Width;
                MyImage.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top);
                MyImage.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left);
                ProgressTransform.TranslateY = MyImage.Height / 2;
            };
            Window.Current.SizeChanged += (s, e) => resize();
            resize();
        }
    }
}

