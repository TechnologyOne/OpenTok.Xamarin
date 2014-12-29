using System;

namespace OpenTok
{
    public enum OTSessionErrorCode  {
        AuthorizationFailure,
        InvalidSession,
        ConnectionFailed,
        NoMessagingServer,
        ConnectionRefused,
        StateFailed,
        P2PSessionMaxParticipants,
        ConnectionTimeout,
        InvalidSignalType,
        SignalDataTooLong
    }

    public enum OTPublisherErrorCode {
        PublisherInternalError,
        SessionDisconnected
    }

    public enum OTSubscriberErrorCode {
        SubscriberInternalError,
        ConnectionTimedOut
    }

    public enum OTSessionConnectionStatus {
        Connected,
        Connecting,
        NotConnected,
        Failed
    }
}
