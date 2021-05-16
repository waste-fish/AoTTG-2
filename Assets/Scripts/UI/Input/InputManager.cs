using Assets.Scripts.UI.InGame.Controls;
using Assets.Scripts.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI.Input
{
    public class InputManager : MonoBehaviour
    {
        private static KeyCode[] _cannonKeys;
        private static KeyCode[] _horseKeys;
        private static KeyCode[] _humanKeys;
        private static KeyCode[] _titanKeys;
        private static KeyCode[] _uiKeys;

        private const string CannonPlayerPrefs = "InputCannon";
        private const string HorsePlayerPrefs = "InputHorse";
        private const string HumanPlayerPrefs = "InputHuman";
        private const string TitanPlayerPrefs = "InputTitan";
        private const string UiPlayerPrefs = "InputUi";
        private const string OtherPlayersPrefs = "OtherControls";

        public const KeyCode ScrollUp = KeyCode.Joystick8Button18;
        public const KeyCode ScrollDown = KeyCode.Joystick8Button19;
        public static KeyCode Menu;

        public static bool HumanJump { get; private set; }
        public static bool HumanGas { get; private set; }
        public static bool HumanGasBurst { get; private set; }
        public static bool HumanAttack { get; private set; }
        public static bool HumanSpecialAttack { get; private set; }
        public static bool HumanHookLeft { get; private set; }
        public static bool HumanHookRight { get; private set; }
        public static bool HumanHookBoth { get; private set; }
        public static bool HumanReelIn { get; private set; }
        public static bool HumanReelOut { get; private set; }

        public InputManager()
        {
#if UNITY_EDITOR
            Menu = KeyCode.P;
#else
            Menu = KeyCode.Escape;
#endif
        }


        public static ControlSettings Settings;

        private void Awake()
        {
            InputMap inputMap = new InputMap();
            inputMap.Enable();

            inputMap.Human.Jump.started += (context) => { HumanJump = true; };
            inputMap.Human.Jump.canceled += (context) => { HumanJump = false; };

            inputMap.Human.Gas.started += (context) => { HumanGas = true; };
            inputMap.Human.Gas.canceled += (context) => { HumanGas = false; };

            inputMap.Human.GasBurst.started += (context) => { HumanGasBurst = true; };
            inputMap.Human.GasBurst.canceled += (context) => { HumanGasBurst = false; };

            inputMap.Human.Attack.started += (context) => { HumanAttack = true; };
            inputMap.Human.Attack.canceled += (context) => { HumanAttack = false; };

            inputMap.Human.SpecialAttack.started += (context) => { HumanSpecialAttack = true; };
            inputMap.Human.SpecialAttack.canceled += (context) => { HumanSpecialAttack = false; };

            inputMap.Human.Attack.started += (context) => { HumanAttack = true; };
            inputMap.Human.Attack.canceled += (context) => { HumanAttack = false; };

            inputMap.Human.HookLeft.started += (context) => { HumanHookLeft = true; };
            inputMap.Human.HookLeft.canceled += (context) => { HumanHookLeft = false; };

            inputMap.Human.HookRight.started += (context) => { HumanHookRight = true; };
            inputMap.Human.HookRight.canceled += (context) => { HumanHookRight = false; };

            inputMap.Human.HookBoth.started += (context) => { HumanHookBoth = true; };
            inputMap.Human.HookBoth.canceled += (context) => { HumanHookBoth = false; };

            inputMap.Human.ReelIn.started += (context) => { HumanReelIn = true; };
            inputMap.Human.ReelIn.canceled += (context) => { HumanReelIn = false; };

            inputMap.Human.ReelOut.started += (context) => { HumanReelOut = true; };
            inputMap.Human.ReelOut.canceled += (context) => { HumanReelOut = false; };

            LoadRebinds(typeof(InputCannon));
            LoadRebinds(typeof(InputHorse));
            LoadRebinds(typeof(InputHuman));
            LoadRebinds(typeof(InputTitan));
            LoadRebinds(typeof(InputUi));

            Settings = JsonConvert.DeserializeObject<ControlSettings>(PlayerPrefs.GetString(OtherPlayersPrefs));
            if (Settings == null)
            {
                Settings = new ControlSettings();
                PlayerPrefs.SetString(OtherPlayersPrefs, JsonConvert.SerializeObject(Settings));
            }
        }

        void RegisterEvent(InputAction inputAction, bool input)
        {
            inputAction.started += (context) => { input = true; };
            inputAction.canceled += (context) => { input = false; };
        }
        void DeregisterEvent(InputAction inputAction, bool input)
        {
            inputAction.started -= (context) => { input = true; };
            inputAction.canceled -= (context) => { input = false; };
        }

        private void OnHumanJump(InputAction.CallbackContext context)
        {

        }

        #region Default Rebinds

        private static void SetDefaultCannonKeyBindings()
        {
            var cannonKeys = new Dictionary<InputCannon, KeyCode>
            {
                [InputCannon.Up] = KeyCode.W,
                [InputCannon.Down] = KeyCode.S,
                [InputCannon.Left] = KeyCode.A,
                [InputCannon.Right] = KeyCode.D,
                [InputCannon.Slow] = KeyCode.LeftShift,
                [InputCannon.Shoot] = KeyCode.Q,
                [InputCannon.Mount] = KeyCode.G
            };

            _cannonKeys = cannonKeys.Values.ToArray();
            PlayerPrefs.SetString(CannonPlayerPrefs, JsonConvert.SerializeObject(_cannonKeys));
        }

        private static void SetDefaultHorseKeyBindings()
        {
            var horseKeys = new Dictionary<InputHorse, KeyCode>
            {
                [InputHorse.Forward] = KeyCode.W,
                [InputHorse.Backward] = KeyCode.S,
                [InputHorse.Left] = KeyCode.A,
                [InputHorse.Right] = KeyCode.D,
                [InputHorse.Jump] = KeyCode.LeftShift,
                [InputHorse.Mount] = KeyCode.LeftControl,
                [InputHorse.Walk] = KeyCode.Space,
            };

            _horseKeys = horseKeys.Values.ToArray();
            PlayerPrefs.SetString(HorsePlayerPrefs, JsonConvert.SerializeObject(_horseKeys));
        }

        private static void SetDefaultHumanKeyBindings()
        {
            var humanKeys = new Dictionary<InputHuman, KeyCode>
            {
                [InputHuman.Forward] = KeyCode.W,
                [InputHuman.Backward] = KeyCode.S,
                [InputHuman.Left] = KeyCode.A,
                [InputHuman.Right] = KeyCode.D,
                [InputHuman.Gas] = KeyCode.LeftShift,
                [InputHuman.Jump] = KeyCode.LeftShift,
                [InputHuman.Dodge] = KeyCode.LeftControl,
                [InputHuman.Salute] = KeyCode.N,
                [InputHuman.Reload] = KeyCode.R,
                [InputHuman.ReelIn] = KeyCode.Space,
                [InputHuman.ReelOut] = KeyCode.Mouse2,
                [InputHuman.GasBurst] = KeyCode.LeftAlt,
                [InputHuman.Attack] = KeyCode.Mouse0,
                [InputHuman.AttackSpecial] = KeyCode.Mouse1,
                [InputHuman.HookLeft] = KeyCode.Q,
                [InputHuman.HookRight] = KeyCode.E,
                [InputHuman.HookBoth] = KeyCode.None,
                [InputHuman.Item1] = KeyCode.Alpha1,
                [InputHuman.Item2] = KeyCode.Alpha2,
                [InputHuman.Item3] = KeyCode.Alpha3,
                [InputHuman.Focus] = KeyCode.F
            };

            _humanKeys = humanKeys.Values.ToArray();
            PlayerPrefs.SetString(HumanPlayerPrefs, JsonConvert.SerializeObject(_humanKeys));
        }

        private static void SetDefaultTitanKeyBindings()
        {
            var titanKeys = new Dictionary<InputTitan, KeyCode>
            {
                [InputTitan.Forward] = KeyCode.W,
                [InputTitan.Backward] = KeyCode.S,
                [InputTitan.Left] = KeyCode.A,
                [InputTitan.Right] = KeyCode.D,
                [InputTitan.AttackSlap] = KeyCode.Mouse0,
                [InputTitan.AttackBite] = KeyCode.Alpha2,
                [InputTitan.AttackGrabFront] = KeyCode.Alpha1,
                [InputTitan.AttackGrabNape] = KeyCode.Mouse1,
                [InputTitan.AttackGrabBack] = KeyCode.Alpha3,
                [InputTitan.AttackPunch] = KeyCode.Q,
                [InputTitan.AttackBodySlam] = KeyCode.E,
                [InputTitan.Jump] = KeyCode.Space,
                [InputTitan.Cover] = KeyCode.Z,
                [InputTitan.Walk] = KeyCode.LeftShift,
                [InputTitan.Blend] = KeyCode.F
            };

            _titanKeys = titanKeys.Values.ToArray();
            PlayerPrefs.SetString(TitanPlayerPrefs, JsonConvert.SerializeObject(_titanKeys));
        }

        private static void SetDefaultUiKeyBindings()
        {
            var uiKeys = new Dictionary<InputUi, KeyCode>
            {
                [InputUi.Chat] = KeyCode.Return,
                [InputUi.ToggleCursor] = KeyCode.X,
                [InputUi.LiveCamera] = KeyCode.Y,
                [InputUi.Minimap] = KeyCode.M,
                [InputUi.Fullscreen] = KeyCode.Backspace,
                [InputUi.Camera] = KeyCode.C,
                [InputUi.Pause] = KeyCode.P,
                [InputUi.Restart] = KeyCode.T,
                [InputUi.InteractionWheel] = KeyCode.I, //was Tab
                [InputUi.Screenshot] = KeyCode.F2,
                [InputUi.HideHUD] = KeyCode.F1,
                [InputUi.Scoreboard] = KeyCode.Tab,
                [InputUi.HideHooks] = KeyCode.H
            };

            _uiKeys = uiKeys.Values.ToArray();
            PlayerPrefs.SetString(UiPlayerPrefs, JsonConvert.SerializeObject(_uiKeys));
        }

        #endregion

        public static void LoadRebinds(Type inputType)
        {
            if (inputType == typeof(InputCannon))
            {
                var cannonRebinds = PlayerPrefs.GetString(CannonPlayerPrefs);
                if (string.IsNullOrEmpty(cannonRebinds))
                {
                    SetDefaultCannonKeyBindings();
                    cannonRebinds = PlayerPrefs.GetString(CannonPlayerPrefs);
                }

                _cannonKeys = JsonConvert.DeserializeObject<KeyCode[]>(cannonRebinds);
                if (_cannonKeys.Length != Enum.GetNames(inputType).Length)
                {
                    SetDefaultCannonKeyBindings();
                    LoadRebinds(inputType);
                }
            }
            else if (inputType == typeof(InputHorse))
            {
                var horseRebinds = PlayerPrefs.GetString(HorsePlayerPrefs);
                if (string.IsNullOrEmpty(horseRebinds))
                {
                    SetDefaultHorseKeyBindings();
                    horseRebinds = PlayerPrefs.GetString(HorsePlayerPrefs);
                }

                _horseKeys = JsonConvert.DeserializeObject<KeyCode[]>(horseRebinds);
                if (_horseKeys.Length != Enum.GetNames(inputType).Length)
                {
                    SetDefaultHorseKeyBindings();
                    LoadRebinds(inputType);
                }
            }
            else if (inputType == typeof(InputHuman))
            {
                var humanRebinds = PlayerPrefs.GetString(HumanPlayerPrefs);
                if (string.IsNullOrEmpty(humanRebinds))
                {
                    SetDefaultHumanKeyBindings();
                    humanRebinds = PlayerPrefs.GetString(HumanPlayerPrefs);
                }

                _humanKeys = JsonConvert.DeserializeObject<KeyCode[]>(humanRebinds);
                if (_humanKeys.Length != Enum.GetNames(inputType).Length)
                {
                    SetDefaultHumanKeyBindings();
                    LoadRebinds(inputType);
                }
            }
            else if (inputType == typeof(InputTitan))
            {
                var titanRebinds = PlayerPrefs.GetString(TitanPlayerPrefs);
                if (string.IsNullOrEmpty(titanRebinds))
                {
                    SetDefaultTitanKeyBindings();
                    titanRebinds = PlayerPrefs.GetString(TitanPlayerPrefs);
                }

                _titanKeys = JsonConvert.DeserializeObject<KeyCode[]>(titanRebinds);
                if (_titanKeys.Length != Enum.GetNames(inputType).Length)
                {
                    SetDefaultTitanKeyBindings();
                    LoadRebinds(inputType);
                }
            }
            else if (inputType == typeof(InputUi))
            {
                var uiRebinds = PlayerPrefs.GetString(UiPlayerPrefs);
                if (string.IsNullOrEmpty(uiRebinds))
                {
                    SetDefaultUiKeyBindings();
                    uiRebinds = PlayerPrefs.GetString(UiPlayerPrefs);
                }
                _uiKeys = JsonConvert.DeserializeObject<KeyCode[]>(uiRebinds);
                if (_uiKeys.Length != Enum.GetNames(inputType).Length)
                {
                    SetDefaultUiKeyBindings();
                    LoadRebinds(inputType);
                }
            }
            else
            {
                throw new ArgumentException($"{inputType} is not implemented in InputManager.LoadRebinds");
            }
        }

        public static void SaveRebinds<T>(KeyCode[] newKeys)
        {
            var json = JsonConvert.SerializeObject(newKeys);
            PlayerPrefs.SetString(GetPlayerPrefs<T>(), json);
            LoadRebinds(typeof(T));
        }

        public static void SaveOtherPlayerPrefs()
        {
            PlayerPrefs.SetString(OtherPlayersPrefs, JsonConvert.SerializeObject(Settings));
        }

        public static void SetDefaultRebinds(Type inputEnum)
        {
            if (inputEnum == typeof(InputCannon))
            {
                SetDefaultCannonKeyBindings();
            }
            else if (inputEnum == typeof(InputHorse))
            {
                SetDefaultHorseKeyBindings();
            }
            else if (inputEnum == typeof(InputHuman))
            {
                SetDefaultHumanKeyBindings();
            }
            else if (inputEnum == typeof(InputTitan))
            {
                SetDefaultTitanKeyBindings();
            }
            else if (inputEnum == typeof(InputUi))
            {
                SetDefaultUiKeyBindings();
            }
        }

        private static KeyCode GetRebind<T>(T type)
        {
            var index = (int) (object) type;
            if (type is InputCannon)
            {
                return _cannonKeys[index];
            }

            if (type is InputHorse)
            {
                return _horseKeys[index];
            }

            if (type is InputHuman)
            {
                return _humanKeys[index];
            }

            if (type is InputTitan)
            {
                return _titanKeys[index];
            }

            if (type is InputUi)
            {
                return _uiKeys[index];
            }

            throw new ArgumentException($"{type.GetType()} is not implemented in InputManager.GetRebinds");
        }

        private static string GetPlayerPrefs<T>()
        {
            if (typeof(T) == typeof(InputCannon))
            {
                return CannonPlayerPrefs;
            }

            if (typeof(T) == typeof(InputHorse))
            {
                return HorsePlayerPrefs;
            }

            if (typeof(T) == typeof(InputHuman))
            {
                return HumanPlayerPrefs;
            }

            if (typeof(T) == typeof(InputTitan))
            {
                return TitanPlayerPrefs;
            }

            if (typeof(T) == typeof(InputUi))
            {
                return UiPlayerPrefs;
            }

            throw new ArgumentException($"{typeof(T)} is not implemented in InputManager.GetPlayerPrefs");
        }

        #region KeyDown

        [Obsolete]
        public static bool KeyDown(InputCannon input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_cannonKeys[index])
                ? IsScrolling(_cannonKeys[index])
                : UnityEngine.Input.GetKeyDown(_cannonKeys[index]);
        }

        [Obsolete]
        public static bool KeyDown(InputHuman input)
        {
            if (input != InputHuman.Item1 && MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_humanKeys[index])
                ? IsScrolling(_humanKeys[index])
                : UnityEngine.Input.GetKeyDown(_humanKeys[index]);
        }

        [Obsolete]
        public static bool KeyDown(InputHorse input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_horseKeys[index])
                ? IsScrolling(_horseKeys[index])
                : UnityEngine.Input.GetKeyDown(_horseKeys[index]);
        }

        [Obsolete]
        public static bool KeyDown(InputTitan input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_titanKeys[index])
                ? IsScrolling(_titanKeys[index])
                : UnityEngine.Input.GetKeyDown(_titanKeys[index]);
        }

        [Obsolete]
        public static bool KeyDown(InputUi input)
        {
            if (input != InputUi.Chat && input != InputUi.Screenshot && MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_uiKeys[index])
                ? IsScrolling(_uiKeys[index])
                : UnityEngine.Input.GetKeyDown(_uiKeys[index]);
        }

        #endregion

        #region KeyPressed

        [Obsolete]
        public static bool Key(InputCannon input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_cannonKeys[index])
                ? IsScrolling(_cannonKeys[index])
                : UnityEngine.Input.GetKey(_cannonKeys[index]);
        }

        [Obsolete]
        public static bool Key(InputHuman input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_humanKeys[index])
                ? IsScrolling(_humanKeys[index])
                : UnityEngine.Input.GetKey(_humanKeys[index]);
        }

        [Obsolete]
        public static bool Key(InputHorse input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_horseKeys[index])
                ? IsScrolling(_horseKeys[index])
                : UnityEngine.Input.GetKey(_horseKeys[index]);
        }

        [Obsolete]
        public static bool Key(InputTitan input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_titanKeys[index])
                ? IsScrolling(_titanKeys[index])
                : UnityEngine.Input.GetKey(_titanKeys[index]);
        }

        [Obsolete]
        public static bool Key(InputUi input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_uiKeys[index])
                ? IsScrolling(_uiKeys[index])
                : UnityEngine.Input.GetKey(_uiKeys[index]);
        }

        #endregion

        #region KeyUp

        [Obsolete]
        public static bool KeyUp(InputCannon input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_cannonKeys[index])
                ? IsScrolling(_cannonKeys[index])
                : UnityEngine.Input.GetKeyUp(_cannonKeys[index]);
        }

        [Obsolete]

        public static bool KeyUp(InputHuman input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_humanKeys[index])
                ? IsScrolling(_humanKeys[index])
                : UnityEngine.Input.GetKeyUp(_humanKeys[index]);
        }

        [Obsolete]
        public static bool KeyUp(InputHorse input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_horseKeys[index])
                ? IsScrolling(_horseKeys[index])
                : UnityEngine.Input.GetKeyUp(_horseKeys[index]);
        }

        [Obsolete]
        public static bool KeyUp(InputTitan input)
        {
            if (MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_titanKeys[index])
                ? IsScrolling(_titanKeys[index])
                : UnityEngine.Input.GetKeyUp(_titanKeys[index]);
        }

        [Obsolete]
        public static bool KeyUp(InputUi input)
        {
            if (input != InputUi.InteractionWheel && MenuManager.IsAnyMenuOpen) return false;
            var index = (int) input;
            return IsMouseScrollKeyCode(_uiKeys[index])
                ? IsScrolling(_uiKeys[index])
                : UnityEngine.Input.GetKeyUp(_uiKeys[index]);
        }

        #endregion

        public static KeyCode GetKey<T>(T inputEnum)
        {
            return GetRebind(inputEnum);
        }

        public static bool IsMouseScrollKeyCode(KeyCode keyCode)
        {
            return keyCode == KeyCode.Joystick8Button18 || keyCode == KeyCode.Joystick8Button19;
        }

        public static bool IsScrolling(KeyCode keyCode)
        {
            if (keyCode == ScrollUp)
            {
                return IsScrollUp();
            }

            if (keyCode == ScrollDown)
            {
                return IsScrollDown();
            }

            return false;
        }

        private static bool IsScrollUp()
        {
            return UnityEngine.Input.mouseScrollDelta.y > 0;
        }

        private static bool IsScrollDown()
        {
            return UnityEngine.Input.mouseScrollDelta.y < 0;
        }
    }
}