using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using uLua;

namespace antSoftware {
    /// <summary>Wrapper class which indirectly exposes the Unity Application to the Game API.</summary>
    /** All public members of this class are exposed to Lua. Inherits from ```uLua.ExposedMonoBehaviour```. */
    public class Application : ExposedMonoBehaviour<Application> {
        // Fields
        /** <summary>Keeps track of the application window mode.</summary> */
        public FullScreenMode WindowMode = FullScreenMode.Windowed;

        /** <summary>Keeps track of the requested windowed resolution.</summary> */
        [SerializeField] private Vector2 _WindowedResolution;

        /** <summary>Keeps track of the requested fullscreen resolution.</summary> */
        [SerializeField] private Vector2 _FullscreenResolution;

        /** <summary>Counts the time passed since the last framerate update.</summary> */
        private float TimePassed = 0f;

        /** <summary>Counts the number of rendered frames since the last framerate update.</summary> */
        private int FrameCounter = 0;

        /** <summary>Keeps track of whether the resolution was changed in the present frame to prevent the ResolutionChanged event from being called unnecessarily in Update().</summary> */
        private bool ResolutionChangedThisFrame = false;

        // Access Methods

        /// <summary>Static constructor. Registers the uKey enum and window mode globals.</summary>
        /** This static constructor is only executed once. */
        static Application() {
            // uKey enum (Unity KeyCode)
            var Values = System.Enum.GetValues(typeof(KeyCode));
            foreach (KeyCode KeyCode in Values) {
                Lua.Set("uKey" + KeyCode.ToString(), KeyCode);
            }

            // Custom Key Modifiers
            Lua.Set("uKeyCustomShift", (int)KeyCode.RightShift + (int)KeyCode.LeftShift);
            Lua.Set("uKeyCustomAlt", (int)KeyCode.RightAlt + (int)KeyCode.LeftAlt);
            Lua.Set("uKeyCustomControl", (int)KeyCode.RightControl + (int)KeyCode.LeftControl);

            // Window Mode enum
            Values = System.Enum.GetValues(typeof(FullScreenMode));
            foreach (FullScreenMode FullScreenMode in Values) {
                Lua.Set("uWM_" + FullScreenMode.ToString(), FullScreenMode);
            }
        }

        // Public

        /// <summary>Returns the environment resolution (e.g. desktop resolution in Windows).</summary>
        /** The resolution is returned as a string with the format: ```{Width}x{Height}```. */
        public string EnvironmentResolution {
            get { return $"{Screen.currentResolution.width}x{Screen.currentResolution.height}"; }
        }

        /// <summary>Returns the last measured framerate of the application.</summary>
        /** The framerate is measured by counting the number of frames rendered in one second of real time. */
        public int Framerate { get; private set; }

        /// <summary>Returns the requested fullscreen resolution.</summary>
        /** The resolution is returned as a string with the format: ```{Width}x{Height}```. */
        public string FullscreenResolution {
            get { return $"{_FullscreenResolution.x}x{_FullscreenResolution.y}"; }
            set {
                List<string> FullscreenResolution = new List<string>(value.Split('x'));
                if (FullscreenResolution.Count == 2) _FullscreenResolution = new Vector2(int.Parse(FullscreenResolution[0]), int.Parse(FullscreenResolution[1]));
            }
        }

        /// <summary>Returns the current height of the Application window.</summary>
        /** This property uses ```Screen.height``` which only updates one frame after the resolution is changed. */
        public float Height {
            get { return Screen.height; }
        }

        /// <summary>Returns the number of available resolutions for the application.</summary>
        /** Determined by Unity by ```Screen.resolutions.Length```. May be used to browse through the list of available resolutions using the method antSoftware.Application.AvailableResolutions(). */
        public int NumResolutionsAvailable {
            get { return Screen.resolutions.Length; }
        }

        /// <summary>Returns a string which indicates the runtime platform.</summary>
        /** May be used to implement platform-specific behaviour in the Game API. */
        public string Platform {
            get { return UnityEngine.Application.platform.ToString(); }
        }

        /// <summary>Returns the requested resolution based on the current WindowMode setting.</summary>
        /** ```FullScreenWindow``` returns ```EnvironmentResolution```. 
         *  ```ExclusiveFullScreen``` returns ```FullScreenResolution```. 
         *  ```Windowed``` returns ```WindowedResolution```. 
         *  The resolution is returned as a string with the format: ```{Width}x{Height}```. */
        public string RequestedResolution {
            get {
                string RequestedResolution = "0x0";

                switch (WindowMode) {
                    case FullScreenMode.FullScreenWindow:
                        RequestedResolution = EnvironmentResolution;
                        break;
                    case FullScreenMode.ExclusiveFullScreen:
                        RequestedResolution = FullscreenResolution;
                        break;
                    case FullScreenMode.Windowed:
                        RequestedResolution = WindowedResolution;
                        break;
                    default:
                        break;
                }

                return RequestedResolution;
            }
        }

        /// <summary>Returns the resolution of the Application window. Can also be used to set the resolution for the current window mode setting.</summary>
        /** The resolution is returned as a string with the format: ```{Width}x{Height}```. */
        public string Resolution {
            get { return $"{Width}x{Height}"; }
            set {
                List<string> Resolution = new List<string>(value.Split('x'));

                if (Resolution.Count == 2) {
                    switch (WindowMode) {
                        case FullScreenMode.ExclusiveFullScreen:
                            _FullscreenResolution = new Vector2(int.Parse(Resolution[0]), int.Parse(Resolution[1]));
                            break;
                        case FullScreenMode.Windowed:
                            _WindowedResolution = new Vector2(int.Parse(Resolution[0]), int.Parse(Resolution[1]));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>Returns the runtime in seconds since the game started.</summary>
        /** Uses Unity's ```Time.realtimeSinceStartup```. */
        public int Runtime {
            get { return (int)Time.realtimeSinceStartup; }
        }

        /// <summary>Used to access/set the VSync setting of the Application.</summary>
        /** This property uses ```QualitySettings.vSyncCount``` directly. Uses a binary true/false setting (0/1).*/
        public bool VSync {
            get { return (QualitySettings.vSyncCount == 0)?false:true; }

            set { if (value) QualitySettings.vSyncCount = 1; else QualitySettings.vSyncCount = 0; }
        }

        /// <summary>Returns the current width of the Application window.</summary>
        /** This property uses ```Screen.width``` which only updates one frame after the resolution is changed. */
        public float Width {
            get { return Screen.width; }
        }

        /// <summary>Returns the requested windowed resolution.</summary>
        /** The resolution is returned as a string with the format: ```{Width}x{Height}```. */
        public string WindowedResolution {
            get { return $"{_WindowedResolution.x}x{_WindowedResolution.y}"; }
            set {
                List<string> WindowedResolution = new List<string>(value.Split('x'));
                if (WindowedResolution.Count == 2) _WindowedResolution = new Vector2(int.Parse(WindowedResolution[0]), int.Parse(WindowedResolution[1]));
            }
        }

        // Process Methods
        // Public

        /// <summary>Applies the requested video mode (WindowMode/Resolution).</summary>
        /** This method invokes the ```ResolutionChanged``` event. */
        public void ApplyVideoMode() {
            switch (WindowMode) {
                case FullScreenMode.ExclusiveFullScreen:
                case FullScreenMode.FullScreenWindow:
                    Screen.SetResolution((int)_FullscreenResolution.x, (int)_FullscreenResolution.y, WindowMode);
                    API.Invoke("ResolutionChanged");
                    break;
                case FullScreenMode.Windowed:
                    Screen.SetResolution((int)_WindowedResolution.x, (int)_WindowedResolution.y, WindowMode);
                    API.Invoke("ResolutionChanged");
                    break;
                default:
                    break;
            }

            ResolutionChangedThisFrame = true;

            Debug.Log($"Application: Video Mode set to {RequestedResolution} ({WindowMode} mode).");
        }

        /// <summary>Returns the resolution at the specified index from the list of available resolutions.</summary>
        /** @param i The index of the requested resolution. */
        public string AvailableResolutions(int i) {
            string AvailableResolutions = "0x0";
            if (NumResolutionsAvailable > i) AvailableResolutions = $"{Screen.resolutions[i].width}x{Screen.resolutions[i].height}";
            return AvailableResolutions;
        }

        /// <summary>Checks if the specified key is pressed.</summary>
        /** @param Key The keycode of the inspected key. */
        public bool IsKeyPressed(KeyCode Key) {
            bool IsKeyPressed = false;

            if ((int)Key == 615) { // Custom Alt
                IsKeyPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            } else if ((int)Key == 611) { // Custom Control
                IsKeyPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            } else if ((int)Key == 607) { // Custom Shift
                IsKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            } else IsKeyPressed = Input.GetKey(Key);

            return IsKeyPressed;
        }

        /// <summary>Returns a string corresponding to the specified key combination.</summary>
        /** @param Key KeyCode of the key.
          * @param Modifier KeyCode of the modifier key.
          * @param Short (Optional) If set to true, this method will shorten the returned string where possible. */
        public string KeyCombinationToText(KeyCode Key, KeyCode Modifier, bool Short = false) {
            string Text = "";

            if (Modifier != KeyCode.None) {
                Text = Modifier.ToString() switch {
                    // Custom Control
                    "611" or "LeftControl" or "RightControl" => "Ctrl",
                    // Custom Shift
                    "607" or "LeftShift" or "RightShift" => "Shift",
                    // Custom Alt
                    "615" or "LeftAlt" or "RightAlt" => "Alt",
                    _ => Modifier.ToString(),
                };

                if (Key != KeyCode.None) {
                    if (Short) Text += "-"; else Text += " + ";
                }
            }

            if (Key != KeyCode.None) {
                string KeyText = Key.ToString() switch {
                    "Alpha1" => "1",
                    "Alpha2" => "2",
                    "Alpha3" => "3",
                    "Alpha4" => "4",
                    "Alpha5" => "5",
                    "Alpha6" => "6",
                    "Alpha7" => "7",
                    "Alpha8" => "8",
                    "Alpha9" => "9",
                    "Alpha0" => "0",
                    _ => Key.ToString(),
                };
                Text += KeyText;
            }

            return Text;
        }

        /// <summary>Saves a screenshot to the external directory.</summary>
        /** @param Prefix (Optional) Prefix for the screenshot file name. */
        public void SaveScreenshot(string Prefix = "") {
            DateTime Now = DateTime.Now;

            int Year = Now.Year;
            int Month = Now.Month;
            int Day = Now.Day;
            int Hour = Now.Hour;
            int Minute = Now.Minute;
            int Second = Now.Second;

            string Date = Year.ToString();
            if (Month < 10) Date += $"0{Month.ToString()}"; else Date += Month.ToString();
            if (Day < 10) Date += $"0{Day.ToString()}"; else Date += Day.ToString();
            if (Hour < 10) Date += $"0{Hour.ToString()}"; else Date += Hour.ToString();
            if (Minute < 10) Date += $"0{Minute.ToString()}"; else Date += Minute.ToString();
            if (Second < 10) Date += $"0{Second.ToString()}"; else Date += Second.ToString();

            ScreenCapture.CaptureScreenshot($"{ExternalDirectory}Screenshots/{Prefix}{Date}.png");

            Debug.Log($"Application: Screenshot captured: '{Prefix}{Date}.png'.");
        }

        /// <summary>Stops the Application.</summary>
        /** This method calls ```UnityEngine.Application.Quit()``` directly. */
        public void Stop() {
            UnityEngine.Application.Quit();
        }

        // Protected

        /// <summary>Initialises video mode variables (WindowMode/Resolution).</summary>
        /** This method also sets up an external screenshot directory. */
        protected override void Awake() {
            base.Awake();

            // Set up screenshot directory
            if (!Directory.Exists($"{ExternalDirectory}Screenshots/")) {
                Directory.CreateDirectory($"{ExternalDirectory}Screenshots/");
                Debug.Log($"Application: Created directory: '{ExternalDirectory}Screenshots/'.");
            }

            // Initialise video mode variables
            WindowMode = Screen.fullScreenMode;
            FullscreenResolution = EnvironmentResolution;
            WindowedResolution = Resolution;

            Debug.Log($"Application: Started in {Resolution} ({WindowMode} mode).");
        }

        // Private

        /// <summary>Used to aggregate all modifier keys for quick checks when a key is pressed.</summary>
        /** The Shift/Control/Alt keys are included as modifier keys. */
        private bool IsKeyModifier(KeyCode Key) {
            return (Key == KeyCode.LeftShift || Key == KeyCode.RightShift || Key == KeyCode.LeftAlt || Key == KeyCode.RightAlt || Key == KeyCode.LeftControl || Key == KeyCode.RightControl || Key == KeyCode.LeftCommand || Key == KeyCode.RightCommand);
        }

        /// <summary>Invokes the KeyDown and KeyUp events when a key is pressed or released.</summary>
        /** The KeyCode of the pressed/released key is passed as a parameter to the invoked event. */
        private void OnGUI() {
            Event Event = Event.current;

            if (Event.type == EventType.KeyDown) {
                if (Event.isKey && Event.keyCode != KeyCode.None) {
                    if (!IsKeyModifier(Event.keyCode) && Event.keyCode != KeyCode.Return) {
                        API.Invoke("KeyDown", Event.keyCode);
                    }
                }
            } else if (Event.type == EventType.KeyUp) {
                if (Event.isKey && Event.keyCode != KeyCode.None) {
                    if (!IsKeyModifier(Event.keyCode) && Event.keyCode != KeyCode.Return) {
                        API.Invoke("KeyUp", Event.keyCode);
                    }
                }
            }
        }

        /// <summary>Measures the framerate and corrects the video mode in special cases.</summary>
        /** This method makes sure certain platform-specific behaviours are always kept consistent.
         *  ```MaximizedWindow``` is only used in the OSXPlayer, ```ExclusiveFullScreen``` is used in the WindowsPlayer.
         *  In Android platforms the resolution is set by the ```EnvironmentResolution``` and the window mode is always set to ```FullScreenWindow```. */
        private void Update() {
            // Correct Video Mode
#if UNITY_STANDALONE
            if (WindowMode == FullScreenMode.MaximizedWindow && UnityEngine.Application.platform != RuntimePlatform.OSXPlayer) WindowMode = FullScreenMode.FullScreenWindow;
            if (WindowMode == FullScreenMode.ExclusiveFullScreen && UnityEngine.Application.platform != RuntimePlatform.WindowsPlayer) WindowMode = FullScreenMode.FullScreenWindow;

            // Reset ResolutionChanged flag
            if (ResolutionChangedThisFrame) {
                ResolutionChangedThisFrame = false;
            } else {
                // Window Resized
                if (Resolution != RequestedResolution) {
                    Resolution = $"{Width}x{Height}";
                    API.Invoke("ResolutionChanged");
                }
            }
#else
            if (WindowMode == FullScreenMode.Windowed) WindowMode = FullScreenMode.FullScreenWindow;
            FullscreenResolution = EnvironmentResolution;
            WindowedResolution = EnvironmentResolution;
#endif

            // Framerate calculation
            TimePassed += Time.deltaTime;
            ++FrameCounter;

            // Measure framerate
            if (TimePassed >= 1.0f) {
                Framerate = Mathf.RoundToInt(FrameCounter/TimePassed);

                TimePassed = 0f;
                FrameCounter = 0;
            }
        }

        /// <summary>Returns the external directory as defined in the Game API.</summary>
        /** The external directory is used in this class to capture screenshots. */
        private string ExternalDirectory {
            get { return API.ExternalDirectory; }
        }
    }
}