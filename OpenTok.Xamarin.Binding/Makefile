BTOUCH=/Developer/MonoTouch/usr/bin/btouch
SMCS=/Developer/MonoTouch/usr/bin/smcs
MONOXBUILD=/Library/Frameworks/Mono.framework/Commands/xbuild

all: OpenTok.dll

# Opentok: clean
#	cp ../OpenTok-iOS-2.3.1/Opentok.framework/OpenTok .


OpenTok.dll: Makefile ApiDefinition.cs StructsAndEnums.cs Opentok
	$(MONOXBUILD) /p:Configuration=Release OpenTok.Xamarin.Binding.csproj
	cp bin/Release/OpenTok.dll OpenTok.dll

clean:
	-rm -rf list ios *.dll *.zip *.mdb *.sln bin/ obj/ Opentok 

