using System;
using MonoTouch.ObjCRuntime;

[assembly: LinkWith ("OpenTok", LinkTarget.ArmV7 | LinkTarget.ArmV7s, Frameworks = "CoreTelephony MobileCoreServices CFNetwork SystemConfiguration CoreMedia Security AudioToolbox CoreAudio CoreVideo OpenGLES QuartzCore AVFoundation CoreGraphics", LinkerFlags = "-lstdc++.6.0.9 -lsqlite3 -ObjC", IsCxx = true, SmartLink = true, ForceLoad = true)]