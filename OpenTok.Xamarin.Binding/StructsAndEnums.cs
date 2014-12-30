using System;

namespace OpenTok
{
    public enum OTSessionErrorCode  {
        OTSessionSuccess = 0,
        OTAuthorizationFailure = 1004,
        OTErrorInvalidSession = 1005,
        OTConnectionFailed = 1006,
        OTNullOrInvalidParameter = 1011,
        OTNotConnected = 1010,
        OTSessionIllegalState = 1015,
        OTNoMessagingServer = 1503,
        OTConnectionRefused = 1023,
        OTSessionStateFailed = 1020,
        OTP2PSessionMaxParticipants = 1403,
        OTSessionConnectionTimeout = 1021,
        OTSessionInternalError = 2000,
        OTSessionInvalidSignalType = 1461,
        OTSessionSignalDataTooLong = 1413,
        OTConnectionDropped = 1022,
        OTSessionSubscriberNotFound = 1112,
        OTSessionPublisherNotFound = 1113
    }

    public enum OTPublisherErrorCode {
        OTPublisherSuccess = 0,
        OTSessionDisconnected = 1010,
        OTPublisherInternalError = 2000,
        OTPublisherWebRTCError = 1610
    }

    public enum OTSubscriberErrorCode {
        OTSubscriberSuccess = 0,
        OTConnectionTimedOut = 1542,
        OTSubscriberSessionDisconnected = 1541,
        OTSubscriberWebRTCError = 1600,
        OTSubscriberServerCannotFindStream = 1604,
        OTSubscriberInternalError = 2000
    }

    public enum OTSessionConnectionStatus {
        OTSessionConnectionStatusNotConnected,
        OTSessionConnectionStatusConnected,
        OTSessionConnectionStatusConnecting,
        OTSessionConnectionStatusDisconnecting,
        OTSessionConnectionStatusFailed
    }

    public enum OTVideoOrientation {
        OTVideoOrientationUp = 1,
        OTVideoOrientationDown = 2,
        OTVideoOrientationLeft = 3,
        OTVideoOrientationRight = 4
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
        OTSubscriberVideoEventPublisherPropertyChanged = 1,
        OTSubscriberVideoEventSubscriberPropertyChanged = 2,
        OTSubscriberVideoEventQualityChanged = 3
    }
}
