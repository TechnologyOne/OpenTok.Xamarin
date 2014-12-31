using System;
using System.Diagnostics;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using OpenTok;

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
        // *** https://dashboard.tokbox.com/projects  
        const string _apiKey = "45117962";
        const string _sessionId = @"2_MX40NTExNzk2Mn5-MTQxOTk1NzExOTM3OX5LZHdFTHJaOS8vYVdKbGRNMXZnRnB5Tm5-fg"; 
        const string _token = @"T1==cGFydG5lcl9pZD00NTExNzk2MiZzaWc9MTNjNzI1MzllM2Q4NWFiODAyZmNjZjg4NWE3MTcwMzU0ODdmNjAzYzpyb2xlPXB1Ymxpc2hlciZzZXNzaW9uX2lkPTJfTVg0ME5URXhOemsyTW41LU1UUXhPVGsxTnpFeE9UTTNPWDVMWkhkRlRISmFPUzh2WVZkS2JHUk5NWFpuUm5CNVRtNS1mZyZjcmVhdGVfdGltZT0xNDE5OTU3MTU4Jm5vbmNlPTAuMjkwMDY3Mjg4NzY3NjUyNjQmZXhwaXJlX3RpbWU9MTQyMjU0OTA4MA==";     

        bool _subscribeToSelf = true; // Change to false to subscribe to streams other than your own.

        public MainViewController() : base("MainViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Hide top Nav bar
            this.NavigationController.SetNavigationBarHidden (true, false);
           
            DoConnect();
        }

        private void DoConnect()
        {
            OTError error;

            _session = new OTSession(_apiKey, _sessionId, new SessionDelegate(this));

            _session.ConnectWithToken (_token, out error);

            if (error != null)
            {
                this.ShowAlert(error.Description);
            }
        }

        private void DoDisconnect()
        {
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
            private MainViewController _this;

            public SessionDelegate(MainViewController This)
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

                _this.ShowAlert(msg);
            }

            public override void DidDisconnect(OTSession session)
            {
                var msg = "SessionDelegate:DidDisconnect: " + session.SessionId;

                Debug.WriteLine(msg);

                _this.ShowAlert(msg);
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
            private MainViewController _this;

            public SubscriberDelegate(MainViewController This)
            {
                _this = This;
            }

            public override void DidConnectToStream(OTSubscriber subscriber)
            {
                Debug.WriteLine("SubscriberDelegate:DidConnectToStream: " + subscriber.Stream.StreamId);

                _this._subscriber.View.Frame = new RectangleF(0, widgetHeight, widgetWidth, widgetHeight);

                _this.View.AddSubview(_this._subscriber.View);
            }

            public override void DidFailWithError(OTSubscriber subscriber, OTError error)
            {
                var msg = String.Format("SubscriberDelegate:DidFailWithError: Stream {0}, Error: {1}", subscriber.Stream.StreamId, error.Description);

                Debug.WriteLine(msg);

                _this.ShowAlert(msg);
            }
        }

        private class PublisherDelegate : OTPublisherKitDelegate
        {
            private MainViewController _this;

            public PublisherDelegate(MainViewController This)
            {
                _this = This;
            }

            public override void DidFailWithError(OTPublisher publisher, OTError error)
            {
                var msg = String.Format("PublisherDelegate:DidFailWithError: Error: {0}", error.Description);

                Debug.WriteLine(msg);

                _this.ShowAlert(msg);

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

        private void ShowAlert(string message)
        {
            var alert = new UIAlertView ("Alert", message, null, "Ok", null);

            alert.Show ();
        }
    }
}

