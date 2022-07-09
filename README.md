# uApp

uApp is an application framework for Unity Engine which utilises uLua, a Game API framework for Unity.

Combined with uLua, this script sets up a Lua API for the Unity Engine application. Utility features, such as a framerate counter, are also implemented in the ```Application``` class.

## Dependencies

- [MoonSharp for Unity](https://assetstore.unity.com/packages/tools/moonsharp-33776)
- [uLua for Unity](https://bit.ly/uLuaAsset)

## Documentation

This document is accompanied by source code documentation, which is found on the [GitHub pages](https://antUnity.github.io/uApp/) of the project's repository.

For any further questions do not hesitate to contact support@antsoftware.co.uk.

## Usage Tutorial

***Note: It is recommended to read the [uLua Usage Tutorial](https://bit.ly/uLuaDocs) in the full documentation of uLua before reading this document.***

The uApp contains a single script, antSoftware.Application, which generates the Lua API for the Application object.
This script sets up a framework for Unity Engine applications to streamline certain tasks, such as handling key presses, and resolution and window mode changes.

To start using uApp, add the antSoftware.Application script to a game object in your scene.

### Application API

Adding the antSoftware.Application script to any object in your scene will setup the global Application API in Lua.

The Application API contains all public members of the antSoftware.Application class. Some of the available methods are listed below:
- **ApplyVideoMode()**: Applies the requested video mode (WindowMode/Resolution).
- **AvailableResolution()**: Returns the resolution at the specified index from the list of available resolutions.
- **IsKeyPressed()**: Checks if the specified key is pressed.
- **SaveScreenshot()**: Saves a screenshot to the external directory.
- **Stop()**: Stops the Application.

To use the Application API, simply call one of its properties or methods in a Lua script within the uLua framework:

```
local Resolution = Application.Resolution;
local Framerate = Application.Framerate;

print ("Application resolution is " .. Resolution .. ".");
print ("The last known framerate is " .. Framerate .. ".");

Application:Stop();
```

### Enums

The Unity ```FullScreenMode``` and ```KeyCode``` enums are defined in their entirety in Lua. Some examples of the naming convention are listed below:
- **uWM_Windowed**: Corresponds to the ```FullScreenMode.Windowed``` value.
- **uKeyF12**: Corresponds to the ```KeyCode.F12``` value.

### Events

The following events are invoked by antSoftware.Application and may be handled in Lua:
- **ResolutionChanged**: Invoked when a resolution change is detected.
- **KeyDown**: Invoked when a key is pressed. The key code is passed as an argument to the event callback.
- **KeyUp**: Invoked when a key is released. The key code is passed as an argument to the event callback.
