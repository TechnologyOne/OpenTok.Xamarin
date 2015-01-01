using System;
using System.Diagnostics;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using OpenTok;

namespace OpenTok.Xamarin.iOS
{
    public partial class MainViewController : UIViewController
    {
        VideoChatView _videoChatView;

        // *** Fill the following variables using your own Project info from the Dashboard  ***
        // *** https://dashboard.tokbox.com/projects  
        const string _apiKey = "45117962";
        const string _sessionId = @"2_MX40NTExNzk2Mn5-MTQxOTk1NzExOTM3OX5LZHdFTHJaOS8vYVdKbGRNMXZnRnB5Tm5-fg"; 
        const string _token = @"T1==cGFydG5lcl9pZD00NTExNzk2MiZzaWc9MTNjNzI1MzllM2Q4NWFiODAyZmNjZjg4NWE3MTcwMzU0ODdmNjAzYzpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTJfTVg0ME5URXhOemsyTW41LU1UUXhPVGsxTnpFeE9UTTNPWDVMWkhkRlRISmFPUzh2WVZkS2JHUk5NWFpuUm5CNVRtNS1mZyZjcmVhdGVfdGltZT0xNDE5OTU3MTU4Jm5vbmNlPTAuMjkwMDY3Mjg4NzY3NjUyNjQmZXhwaXJlX3RpbWU9MTQyMjU0OTA4MA==";     

        public MainViewController() : base("MainViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Hide top Nav bar
            this.NavigationController.SetNavigationBarHidden (true, false);

            // Configure the Video Chat View
            _videoChatView = new VideoChatView ()
                {
                    Frame = View.Frame,
                    ApiKey = _apiKey,
                    SessionId = _sessionId,
                    Token = _token,
                    SubscribeToSelf = true
                };

            // Add The View
            View.AddSubview (_videoChatView);

            // Subscribe to Events
            _videoChatView.OnHangup += (sender, e) =>
                {
                    Debug.WriteLine("OnHangup: User tapped the hangup button.");
                };

            _videoChatView.OnError += (sender, e) =>
                {
                    Debug.WriteLine(e.Message);

                    this.ShowAlert(e.Message);
                };

            // Connect to Session
            _videoChatView.Connect();
        }

        private void ShowAlert(string message)
        {
            var alert = new UIAlertView ("Alert", message, null, "Ok", null);

            alert.Show ();
        }
    }
}

