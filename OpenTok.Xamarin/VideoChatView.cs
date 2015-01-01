using System;
using System.Diagnostics;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using OpenTok;
using MonoTouch.AVFoundation;

namespace OpenTok.Xamarin.iOS
{
    [Register("VideoChatView")]
    public partial class VideoChatView : UIView
    {
        OTSession _session;
        OTPublisher _publisher;
        OTSubscriber _subscriber;

        string _apiKey;
        string _sessionId;
        string _token;
        bool _subscribeToSelf = false;

        public EventHandler OnHangup;
        public EventHandler<OnErrorEventArgs> OnError;

        public string ApiKey
        {
            set { _apiKey = value; }
        }

        public string SessionId
        {
            set { _sessionId = value; }
        }

        public string Token
        {
            set { _token = value; }
        }

        public bool SubscribeToSelf
        {
            set { _subscribeToSelf = value; }
        }

        public VideoChatView(IntPtr h): base(h)
        {
        }

        public VideoChatView ()
        {
            // Add in the XIB file
            var array = NSBundle.MainBundle.LoadNib("VideoChatView", this, null);

            var view = Runtime.GetNSObject(array.ValueAt(0)) as UIView;

            view.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);

            this.AddSubview(view);

            // Add the Button Handlers
            this.HangupButton.Clicked += HangupButtonHandler;
            this.SwitchButton.Clicked += SwitchButtonHandler;

            // Initialize Status
            this.StatusLabel.Text = String.Empty;
        }

        private void HangupButtonHandler (object sender, EventArgs e)
        {
            if (_session == null)
            {
                this.DoConnect();

                this.HangupButton.TintColor = UIColor.Red;
            }
            else
            {
                this.DoDisconnect();

                this.HangupButton.TintColor = UIColor.Green;

                this.OnHangup(this, null);
            }
        }

        private void SwitchButtonHandler (object sender, EventArgs e)
        {
            try
            {
                if (_publisher.CameraPosition == AVCaptureDevicePosition.Front)
                {
                    _publisher.CameraPosition = AVCaptureDevicePosition.Back;
                }
                else
                {
                    _publisher.CameraPosition = AVCaptureDevicePosition.Front;
                }
            }
            catch{}
        }

        public void Connect()
        {
            this.DoConnect();
        }

        private void DoConnect()
        {
            OTError error;

            this.StatusLabel.Text = "Connecting...";

            _session = new OTSession(_apiKey, _sessionId, new SessionDelegate(this));

            _session.ConnectWithToken (_token, out error);

            if (error != null)
            {
                this.RaiseOnError(error.Description);
            }
        }

        private void DoDisconnect()
        {
            this.StatusLabel.Text = String.Empty;

            this.CleanupSubscriber();

            this.CleanupPublisher();

            if (_session != null) {

                if(_session.SessionConnectionStatus == OTSessionConnectionStatus.Connected){

                    _session.Disconnect ();
                }

                _session.Delegate = null;
                _session.Dispose();
                _session = null;
            }
        }

        /**
         * Sets up an instance of OTPublisher to use with this session. OTPubilsher
         * binds to the device camera and microphone, and will provide A/V streams
         * to the OpenTok session.
         */
        private void DoPublish()
        {
            _publisher = new OTPublisher(new PublisherDelegate(this), UIDevice.CurrentDevice.Name);

            OTError error;

            _session.Publish(_publisher, out error);

            if (error != null)
            {
                this.RaiseOnError(error.Description);
            }

            // Show the Video in the View In Round Mode
            _publisher.View.Frame = new RectangleF(0, 0, 100, 100);
            _publisher.View.Layer.CornerRadius = 50;
            _publisher.View.Layer.MasksToBounds = true;

            this.PublisherView.AddSubview (_publisher.View);
        }

        /**
         * Cleans up the publisher and its view. At this point, the publisher should not
         * be attached to the session any more.
         */
        private void CleanupPublisher()
        {
            if (_publisher != null)
            {
                _publisher.View.RemoveFromSuperview();
                _publisher.Delegate = null;
                _publisher.Dispose();
                _publisher = null;
            }
        }

        /**
         * Instantiates a subscriber for the given stream and asynchronously begins the
         * process to begin receiving A/V content for this stream. Unlike doPublish, 
         * this method does not add the subscriber to the view hierarchy. Instead, we 
         * add the subscriber only after it has connected and begins receiving data.
         */
        private void DoSubscribe(OTStream stream)
        {
            _subscriber = new OTSubscriber(stream, new SubscriberDelegate(this));

            OTError error;

            _session.Subscribe(_subscriber, out error);

            if (error != null)
            {
                this.RaiseOnError(error.Description);
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
            if (_subscriber != null)
            {
                _subscriber.View.RemoveFromSuperview();
                _subscriber.Delegate = null;
                _subscriber.Dispose();
                _subscriber = null;
            }
        }

        private class SessionDelegate : OTSessionDelegate
        {
            private VideoChatView _this;

            public SessionDelegate(VideoChatView This)
            {
                _this = This;
            }

            public override void DidConnect(OTSession session)
            {
                Debug.WriteLine("SessionDelegate:DidConnect: " + session.SessionId);

                _this.DoPublish();
            }

            public override void DidFailWithError(OTSession session, OTError error)
            {
                var msg = "SessionDelegate:DidFailWithError: " + session.SessionId;

                Debug.WriteLine(msg);

                _this.RaiseOnError(msg);
            }

            public override void DidDisconnect(OTSession session)
            {
                var msg = "SessionDelegate:DidDisconnect: " + session.SessionId;

                Debug.WriteLine(msg);
            }

            public override void ConnectionCreated(OTSession session, OTConnection connection)
            {
                Debug.WriteLine("SessionDelegate:ConnectionCreated: " + connection.ConnectionId);
            }

            public override void ConnectionDestroyed(OTSession session, OTConnection connection)
            {
                Debug.WriteLine("SessionDelegate:ConnectionDestroyed: " + connection.ConnectionId);

                _this.CleanupSubscriber();
            }

            public override void StreamCreated(OTSession session, OTStream stream)
            {
                Debug.WriteLine("SessionDelegate:StreamCreated: " + stream.StreamId);

                if(_this._subscriber == null && !_this._subscribeToSelf)
                {
                    _this.DoSubscribe(stream);
                }
            }

            public override void StreamDestroyed(OTSession session, OTStream stream)
            {
                Debug.WriteLine("SessionDelegate:StreamDestroyed: " + stream.StreamId);

                _this.CleanupSubscriber();
            }
        }

        private class SubscriberDelegate : OTSubscriberKitDelegate
        {
            private VideoChatView _this;

            public SubscriberDelegate(VideoChatView This)
            {
                _this = This;
            }

            public override void DidConnectToStream(OTSubscriber subscriber)
            {
                Debug.WriteLine("SubscriberDelegate:DidConnectToStream: " + subscriber.Stream.StreamId);

                _this._subscriber.View.Frame = new RectangleF(0, 0, _this.Frame.Width, _this.Frame.Height);

                _this.SubscriberView.AddSubview(_this._subscriber.View);

                _this.StatusLabel.Text = String.Empty;
            }

            public override void DidFailWithError(OTSubscriber subscriber, OTError error)
            {
                var msg = String.Format("SubscriberDelegate:DidFailWithError: Stream {0}, Error: {1}", subscriber.Stream.StreamId, error.Description);

                Debug.WriteLine(msg);

                _this.RaiseOnError(msg);
            }
        }

        private class PublisherDelegate : OTPublisherKitDelegate
        {
            private VideoChatView _this;

            public PublisherDelegate(VideoChatView This)
            {
                _this = This;
            }

            public override void DidFailWithError(OTPublisher publisher, OTError error)
            {
                var msg = String.Format("PublisherDelegate:DidFailWithError: Error: {0}", error.Description);

                Debug.WriteLine(msg);

                _this.RaiseOnError(msg);

                _this.CleanupPublisher();
            }


            public override void StreamCreated(OTPublisher publisher, OTStream stream)
            {
                Debug.WriteLine("PublisherDelegate:StreamCreated: " + stream.StreamId);

                // If Subscribe To Self is true: Our own publisher is now visible to
                // all participants in the OpenTok session. We will attempt to subscribe to
                // our own stream. Expect to see a slight delay in the subscriber video and
                // an echo of the audio coming from the device microphone.
                if (_this._subscriber == null && _this._subscribeToSelf)
                {
                    _this.DoSubscribe(stream);
                }
            }
            public override void StreamDestroyed(OTPublisher publisher, OTStream stream)
            {
                Debug.WriteLine("PublisherDelegate:StreamDestroyed: " + stream.StreamId);

                _this.CleanupSubscriber();

                _this.CleanupPublisher();
            }
        }

        private void RaiseOnError(string message)
        {
            OnErrorEventArgs e = new OnErrorEventArgs(message);

            this.OnError(this, e);
        }
    }

    public class OnErrorEventArgs : EventArgs
    {
        public OnErrorEventArgs(string s)
        {
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}

