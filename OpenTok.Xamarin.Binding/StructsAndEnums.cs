using System;

namespace OpenTok
{
    public enum OTSessionErrorCode  {
        Success = 0,
        AuthorizationFailure = 1004,
        ErrorInvalidSession = 1005,
        ConnectionFailed = 1006,
        NullOrInvalidParameter = 1011,
        NotConnected = 1010,
        IllegalState = 1015,
        NoMessagingServer = 1503,
        ConnectionRefused = 1023,
        StateFailed = 1020,
        P2PSessionMaxParticipants = 1403,
        ConnectionTimeout = 1021,
        InternalError = 2000,
        InvalidSignalType = 1461,
        SignalDataTooLong = 1413,
        ConnectionDropped = 1022,
        SubscriberNotFound = 1112,
        PublisherNotFound = 1113
    }

    public enum OTPublisherErrorCode {
        Success = 0,
        SessionDisconnected = 1010,
        InternalError = 2000,
        WebRTCError = 1610
    }

    public enum OTSubscriberErrorCode {
        Success = 0,
        ConnectionTimedOut = 1542,
        SessionDisconnected = 1541,
        WebRTCError = 1600,
        ServerCannotFindStream = 1604,
        InternalError = 2000
    }

    public enum OTSessionConnectionStatus {
        NotConnected,
        Connected,
        Connecting,
        Disconnecting,
        Failed
    }

    public enum OTVideoOrientation {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    public enum OTPixelFormat {
        I420 = 1228157488 /* 'I420' */,
        ARGB = 1095911234 /* 'ARGB' */,
        NV12 = 1314271538 /* 'NV12' */
    }

    public enum OTVideoViewType {
        Subscriber,
        Publisher
    }

    public enum OTSubscriberVideoEventReason {
        PublisherPropertyChanged = 1,
        SubscriberPropertyChanged = 2,
        QualityChanged = 3
    }
}
