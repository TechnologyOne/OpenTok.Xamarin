using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OpenTok.Xamarin.Test
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Disable the Idle Timer
            UIApplication.SharedApplication.IdleTimerDisabled = true;

            // create a new window instance based on the screen size
            this.window = new UIWindow (UIScreen.MainScreen.Bounds);

            // Set up the main controller
            UINavigationController rootViewController = new UINavigationController ();

            // Instantiate the Main View
            MainViewController mainView = new MainViewController ();

            rootViewController.PushViewController (mainView, false); 

            // make the window visible
            this.window.RootViewController = rootViewController;
            this.window.MakeKeyAndVisible ();

            return true;
        }
    }
}

