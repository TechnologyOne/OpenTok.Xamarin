// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace OpenTok.Xamarin.iOS
{
	partial class VideoChatView
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem HangupButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PublisherView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel StatusLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView SubscriberView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem SwitchButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar ToolBar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HangupButton != null) {
				HangupButton.Dispose ();
				HangupButton = null;
			}

			if (PublisherView != null) {
				PublisherView.Dispose ();
				PublisherView = null;
			}

			if (SubscriberView != null) {
				SubscriberView.Dispose ();
				SubscriberView = null;
			}

			if (SwitchButton != null) {
				SwitchButton.Dispose ();
				SwitchButton = null;
			}

			if (ToolBar != null) {
				ToolBar.Dispose ();
				ToolBar = null;
			}

			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}
		}
	}
}
