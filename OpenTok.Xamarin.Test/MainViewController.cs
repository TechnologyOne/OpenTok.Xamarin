using System;
using System.Diagnostics;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace OpenTok.Xamarin.Test
{
    public partial class MainViewController : UIViewController
    {
        OTSession _session;
        OTPublisher _publisher;
        OTSubscriber _subscriber;

        static readonly float widgetHeight = 240;
        static readonly float widgetWidth = 320;

        // *** Fill the following variables using your own Project info from the Dashboard  ***
        // ***                   https://dashboard.tokbox.com/projects  
        const string kApiKey = "";

        // Replace with your generated session ID
        const string kSessionId = @""; 
        // Replace with your generated token (use the Dashboard or an OpenTok server-side library)
        const string kToken = @"";     


        bool subscribeToSelf = true; // Change to false to subscribe to streams other than your own.

        public MainViewController() : base("MainViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            _session = new OTSession(kApiKey, kSessionId, new SessionDelegate(this));

            DoConnect();
        }

//        private void UpdateSubscriber()
//        {
//            foreach(var streamId in _session.Streams)
//            {
//                var stream = (OTStream)streamId.Value;
//
//                if (!Equals(stream.Connection.ConnectionId, _session.Connection.ConnectionId))
//                {
//                    _subscriber = new OTSubscriber(stream, new SubDelegate(this));
//                    break;
//                }
//            }
//        }

        private void DoConnect()
        {
            OTError error = null;

            _session.ConnectWithToken (kToken, error);

            if (error != null)
            {
                this.ShowAlert(error.Description);
            }
        }

        /**
         * Sets up an instance of OTPublisher to use with this session. OTPubilsher
         * binds to the device camera and microphone, and will provide A/V streams
         * to the OpenTok session.
         */
        private void DoPublish()
        {
            _publisher = new OTPublisher(new PubDelegate(this), UIDevice.CurrentDevice.Name);

            OTError error = null;

            _session.Publish(_publisher, error);

            if (error != null)
            {
                this.ShowAlert(error.Description);
            }

            _publisher.View.Frame = new RectangleF(0, 0, widgetWidth, widgetHeight);

            View.AddSubview(_publisher.View);
        }

        /**
         * Cleans up the publisher and its view. At this point, the publisher should not
         * be attached to the session any more.
         */
        private void CleanupPublisher()
        {
            _publisher.View.RemoveFromSuperview();
            _publisher = null;

            // This is a good place to notify user that publishing has stopped
        }

        /**
         * Instantiates a subscriber for the given stream and asynchronously begins the
         * process to begin receiving A/V content for this stream. Unlike doPublish, 
         * this method does not add the subscriber to the view hierarchy. Instead, we 
         * add the subscriber only after it has connected and begins receiving data.
         */
        private void DoSubscribe(OTStream stream)
        {
            _subscriber = new OTSubscriber(stream, new SubDelegate(this));

            OTError error = null;

            _session.Subscribe(_subscriber, error);

            if (error != null)
            {
                this.ShowAlert(error.Description);
            }
        }

        /**
         * Cleans the subscriber from the view hierarchy, if any.
         * NB: You do *not* have to call unsubscribe in your controller in response to
         * a streamDestroyed event. Any subscribers (or the publisher) for a stream will
         * be automatically removed from the session during cleanup of the stream.
         */
        private void CleanupSubscriber()
        {
            _subscriber.View.RemoveFromSuperview();
            _subscriber = null;
        }

        private class SessionDelegate : OTSessionDelegate
        {
            private MainViewController _this;
            public SessionDelegate(MainViewController This)
            {
                _this = This;
            }

            public override void DidConnect(OTSession session)
            {
                Debug.WriteLine("Did Connect {0}", session.SessionId);

                _this.DoPublish();
            }

            public override void DidFail(OTSession session, OTError error)
            {
                var msg = string.Format("There was an error connecting to session {0}", session.SessionId);

                Debug.WriteLine("SessionDidFail ({0})", msg);

                _this.ShowAlert(msg);
            }

            public override void DidDisconnect(OTSession session)
            {
                var msg = string.Format("Session disconnected: ({0})", session.SessionId);

                Debug.WriteLine("DidDisconnect ({0})", msg);

                _this.ShowAlert(msg);
            }

            public override void ConnectionCreated(OTSession session, OTConnection connection)
            {
                Debug.WriteLine("ConnectionCreated ({0})", connection.ConnectionId);
            }

            public override void ConnectionDestroyed(OTSession session, OTConnection connection)
            {
                Debug.WriteLine("ConnectionDestroyed ({0})", connection.ConnectionId);

                if (_this._subscriber.Stream.Connection.ConnectionId == connection.ConnectionId)
                {
                    _this.CleanupSubscriber();
                }
            }

            public override void StreamCreated(OTSession session, OTStream stream)
            {
                Debug.WriteLine("StreamCreated ({0})", stream.StreamId);

                if(_this._subscriber == null && !_this.subscribeToSelf)
                {
                    _this.DoSubscribe(stream);
                }
            }

            public override void StreamDestroyed(OTSession session, OTStream stream)
            {
                Debug.WriteLine("StreamDestroyed ({0})", stream.StreamId);

                if (_this._subscriber.Stream.StreamId == stream.StreamId)
                {
                    _this.CleanupSubscriber();
                }
            }
        }

        private class SubDelegate : OTSubscriberDelegate
        {
            private MainViewController _this;
            public SubDelegate(MainViewController This)
            {
                _this = This;

            }


            public override void DidChangeVideoDimensions(OTStream stream, SizeF dimensions)
            {

            }

            public override void DidConnectToStream(OTSubscriber subscriber)
            {
                _this._subscriber.View.Frame = new RectangleF(0, widgetHeight, widgetWidth, widgetHeight);

                _this.View.AddSubview(_this._subscriber.View);
            }

            public override void DidFail(OTSubscriber subscriber, OTError error)
            {
                Debug.WriteLine("Subscriber {0} DidFailWithError {1}", subscriber.Stream.StreamId, error);

                var msg = string.Format("There was an error subscribing to stream {0}", subscriber.Stream.StreamId);

                _this.ShowAlert(msg);
            }
        }

        private class PubDelegate : OTPublisherDelegate
        {
            private MainViewController _this;
            public PubDelegate(MainViewController This)
            {
                _this = This;
            }

            public override void DidChangeCameraPosition(OTPublisher publisher, MonoTouch.AVFoundation.AVCaptureDevicePosition cameraPosition)
            {

            }

            public override void DidFail(OTPublisher publisher, OTError error)
            {
                Debug.WriteLine("Publisher DidFail {0}", error);

                _this.ShowAlert("There was an error publishing");

                _this.CleanupPublisher();
            }


            public override void StreamCreated(OTPublisher publisher, OTStream stream)
            {
                // If Subscribe To Self is true: Our own publisher is now visible to
                // all participants in the OpenTok session. We will attempt to subscribe to
                // our own stream. Expect to see a slight delay in the subscriber video and
                // an echo of the audio coming from the device microphone.
                if (_this._subscriber == null && _this.subscribeToSelf)
                {
                    _this.DoSubscribe(stream);
                }
            }
            public override void StreamDestroyed(OTPublisher publisher, OTStream stream)
            {
                if (_this._subscriber.Stream.StreamId == stream.StreamId)
                {
                    _this.CleanupSubscriber();
                }

                _this.CleanupPublisher();
            }
        }

        private class AlertViewDelegate : UIAlertViewDelegate
        {
            public override void Clicked(UIAlertView alertview, int buttonIndex)
            {

            }
            public override void Canceled(UIAlertView alertView)
            {

            }
        }

        private void ShowAlert(string message)
        {
            var alertView = new UIAlertView("Message from video session",
                message,
                new AlertViewDelegate(),
                "OK");

            alertView.Show();
        }
    }
}

