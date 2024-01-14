using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;

namespace Marvel
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {

#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS || MACCATALYST
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0f;
                handler.PlatformView.Layer.CornerRadius = 0f;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                handler.PlatformView.Layer.BorderColor = Colors.Transparent.ToCGColor();
#elif WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.BorderBrush = null;
#endif

            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
