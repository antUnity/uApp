using System.Collections.Generic;
using UnityEngine;
using System;
using uLua;

namespace AtUnity {
    public class Application : LuaMonoBehaviour {
        // Private Members
        private Vector2 WindowedResolution, FullscreenResolution;
        private float TimePassed = 0f;
        private int Frames = 0;
        private string UserPath = "";

        // Public Members
        public FullScreenMode WindowMode = FullScreenMode.Windowed;

        // Access Methods

        static Application() {
            API.RegisterUserType<Application>();
        }
        
        // Public
        public string AvailableResolutions(int i) {
            string Resolution = "0x0";

            if (NumResolutionsAvailable > i) Resolution = Screen.resolutions[i].width + "x" + Screen.resolutions[i].height;

            return Resolution;
        }

        public int Framerate { get; set; }

        public float GetHeight(FullScreenMode WindowMode) {
            float Height = this.Height;

            switch (WindowMode) {
                case FullScreenMode.FullScreenWindow:
                    Height = ScreenHeight;
                    break;
                case FullScreenMode.ExclusiveFullScreen:
                    Height = FullscreenResolution.y;
                    break;
                case FullScreenMode.Windowed:
                    Height = WindowedResolution.y;
                    break;
                default:
                    break;
            }

            return Height;
        }

        public float GetWidth(FullScreenMode WindowMode) {
            float Width = this.Width;

            switch (WindowMode) {
                case FullScreenMode.FullScreenWindow:
                    Width = ScreenWidth;
                    break;
                case FullScreenMode.ExclusiveFullScreen:
                    Width = FullscreenResolution.x;
                    break;
                case FullScreenMode.Windowed:
                    Width = WindowedResolution.x;
                    break;
                default:
                    break;
            }

            return Width;
        }

        public float Height {
            get { return Screen.height; }
        }

        public int NumResolutionsAvailable {
            get { return Screen.resolutions.Length;  }
        }

        public int Runtime {
            get { return (int)Time.realtimeSinceStartup; }
        }

        public float ScreenHeight {
            get { return Screen.currentResolution.height; }
        }

        public float ScreenWidth {
            get { return Screen.currentResolution.width; }
        }

        public bool VSync {
            get { return (QualitySettings.vSyncCount == 0)?false:true; }

            set { if (value) QualitySettings.vSyncCount = 1; else QualitySettings.vSyncCount = 0; }
        }

        public float Width {
            get { return Screen.width; }
        }

        // Process Methods
        // Public

        public void ApplyVideoMode() {
            switch (WindowMode) {
                //case FullScreenMode.MaximizedWindow:
                case FullScreenMode.ExclusiveFullScreen:
                case FullScreenMode.FullScreenWindow:
                    API.OnEvent("ResolutionChanged");
                    Screen.SetResolution((int)FullscreenResolution.x, (int)FullscreenResolution.y, WindowMode);
                    break;
                case FullScreenMode.Windowed:
                    API.OnEvent("ResolutionChanged");
                    Screen.SetResolution((int)WindowedResolution.x, (int)WindowedResolution.y, WindowMode);
                    break;
                default:
                    break;
            }
        }

        public bool IsKeyPressed(KeyCode Key) {
            bool IsKeyPressed = Input.GetKey(Key);

            if ((int)Key == 615) { // Custom Alt
                IsKeyPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            } else if ((int)Key == 611) { // Custom Control
                IsKeyPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            } else if ((int)Key == 607) { // Custom Shift
                IsKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }

            return IsKeyPressed;
        }

        public string KeyCombinationToText(KeyCode Key, KeyCode Modifier, bool Short = false) {
            string Text = "";

            if (Modifier != KeyCode.None) {
                Text = Modifier.ToString();
                switch (Text) {
                    case "611": // Custom Control
                    case "LeftControl":
                    case "RightControl":
                        Text = "Ctrl";
                        break;
                    case "607": // Custom Shift
                    case "LeftShift":
                    case "RightShift":
                        Text = "Shift";
                        break;
                    case "615": // Custom Alt
                    case "LeftAlt":
                    case "RightAlt":
                        Text = "Alt";
                        break;
                    default:
                        break;
                }

                if (Key != KeyCode.None) {
                    if (Short) Text += "-"; else Text += " + ";
                }
            }

            if (Key != KeyCode.None) {
                string KeyText = Key.ToString();
                switch (KeyText) {
                    case "Alpha1":
                        KeyText = "1";
                        break;
                    case "Alpha2":
                        KeyText = "2";
                        break;
                    case "Alpha3":
                        KeyText = "3";
                        break;
                    case "Alpha4":
                        KeyText = "4";
                        break;
                    case "Alpha5":
                        KeyText = "5";
                        break;
                    case "Alpha6":
                        KeyText = "6";
                        break;
                    case "Alpha7":
                        KeyText = "7";
                        break;
                    case "Alpha8":
                        KeyText = "8";
                        break;
                    case "Alpha9":
                        KeyText = "9";
                        break;
                    case "Alpha0":
                        KeyText = "0";
                        break;
                    default:
                        break;
                }

                Text += KeyText;
            }

            return Text;
        }

        public void SaveScreenshot(int Upscale = 1) {
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int Day = DateTime.Now.Day;
            int Hour = DateTime.Now.Hour;
            int Minute = DateTime.Now.Minute;
            int Second = DateTime.Now.Second;

            string Date = Year.ToString();
            if (Month < 10) Date += "0" + Month.ToString(); else Date += Month.ToString();
            if (Day < 10) Date += "0" + Day.ToString(); else Date += Day.ToString();
            if (Hour < 10) Date += "0" + Hour.ToString(); else Date += Hour.ToString();
            if (Minute < 10) Date += "0" + Minute.ToString(); else Date += Minute.ToString();
            if (Second < 10) Date += "0" + Second.ToString(); else Date += Second.ToString();

            ScreenCapture.CaptureScreenshot(UserPath + "Screenshots\\" + Date + ".png");
        }

        public void SetResolution(string Resolution, FullScreenMode WindowMode) {
            List<string> WidthHeight = new List<string>(Resolution.Split('x'));

            if (WidthHeight.Count == 2) {
                switch (WindowMode) {
                    case FullScreenMode.ExclusiveFullScreen:
                        FullscreenResolution = new Vector2(int.Parse(WidthHeight[0]), int.Parse(WidthHeight[1]));
                        break;
                    case FullScreenMode.Windowed:
                        WindowedResolution = new Vector2(int.Parse(WidthHeight[0]), int.Parse(WidthHeight[1]));
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetVideoMode(FullScreenMode WindowMode, string Resolution, bool VSync) {
            this.WindowMode = WindowMode;
            SetResolution(Resolution, WindowMode);
            this.VSync = VSync;

            ApplyVideoMode();
        }

        public void Stop() {
            UnityEngine.Application.Quit();
        }

        // Private

        private void Awake() {
            UserPath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\Titania\\User\\";

            // Register Globals
            API.Set("WM_WINDOWED", FullScreenMode.Windowed);
            API.Set("WM_FULLSCREEN", FullScreenMode.ExclusiveFullScreen);
            API.Set("WM_BORDERLESS", FullScreenMode.FullScreenWindow);
            //API.Set("WM_MAXIMISED", FullScreenMode.MaximizedWindow);

            // Register Events
            API.RegisterEvent("KeyDown");
            API.RegisterEvent("KeyUp");
            API.RegisterEvent("ResolutionChanged");

            // Register
            API.Register(this);
        }

        private bool IsKeyModifier(KeyCode Key) {
            return (Key == KeyCode.LeftShift || Key == KeyCode.RightShift || Key == KeyCode.LeftAlt || Key == KeyCode.RightAlt || Key == KeyCode.LeftControl || Key == KeyCode.RightControl || Key == KeyCode.LeftCommand || Key == KeyCode.RightCommand);
        }

        private void OnGUI() {
            Event Event = Event.current;

            if (Event.type == EventType.KeyDown) {
                if (Event.isKey && Event.keyCode != KeyCode.None) {
                    if (!IsKeyModifier(Event.keyCode) && Event.keyCode != KeyCode.Return) {
                        API.OnEvent("KeyDown", Event.keyCode);
                    }
                }
            } else if (Event.type == EventType.KeyUp) {
                if (Event.isKey && Event.keyCode != KeyCode.None) {
                    if (!IsKeyModifier(Event.keyCode) && Event.keyCode != KeyCode.Return) {
                        API.OnEvent("KeyUp", Event.keyCode);
                    }
                }
            }
        }

        private void Update() {
#if UNITY_STANDALONE
            if (WindowMode == FullScreenMode.MaximizedWindow && UnityEngine.Application.platform != RuntimePlatform.OSXPlayer) WindowMode = FullScreenMode.FullScreenWindow;
            if (WindowMode == FullScreenMode.ExclusiveFullScreen && UnityEngine.Application.platform != RuntimePlatform.WindowsPlayer) WindowMode = FullScreenMode.FullScreenWindow;
#else
            if (WindowMode == FullScreenMode.Windowed) WindowMode = FullScreenMode.FullScreenWindow;
#endif

            // Window Resized
            if (Width != GetWidth(WindowMode) || Height != GetHeight(WindowMode)) {
                SetResolution(Width + "x" + Height, WindowMode);
                API.OnEvent("ResolutionChanged");
            }

            // FPS Math
            TimePassed += Time.deltaTime;
            ++Frames;

            if (TimePassed > 1.0) {
                Framerate = Frames;

                TimePassed = 0f;
                Frames = 0;
            }
        }
    }
}