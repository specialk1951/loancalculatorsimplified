using Microsoft.Maui.Handlers;
using UIKit;

namespace LoanCalculatorSimplified;

public static class ButtonHandlerCustomization
{
    public static void Configure()
    {
        ButtonHandler.Mapper.AppendToMapping("iOSButtonStyle", (handler, view) =>
        {
            handler.PlatformView.BackgroundColor = UIColor.White;
            handler.PlatformView.SetTitleColor(UIColor.Black, UIControlState.Normal);
        });
    }
}
