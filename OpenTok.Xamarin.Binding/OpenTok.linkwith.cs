using System;
using MonoTouch.ObjCRuntime;
 
// Linker Settings
[assembly: LinkWith("OpenTok", 
                    LinkTarget.ArmV7 | LinkTarget.ArmV7s, 
                    Frameworks = "GLKit CoreTelephony SystemConfiguration OpenGLES CoreMedia CoreVideo CoreGraphics AudioToolBox AVFoundation QuartzCore UIKit Foundation", 
                    LinkerFlags = "-lstdc++.6.0.9 -lpthread -lsqlite3 -lxml2 -ObjC", 
                    IsCxx = true, 
                    SmartLink = true, 
                    ForceLoad = true)]