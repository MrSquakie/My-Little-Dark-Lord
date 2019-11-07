using UnityEngine;
using System.Collections.Generic;

namespace GeNa
{
    /// <summary>
    /// Keyboard and other GeNa defaults
    /// </summary>
    public class GenaDefaults : ScriptableObject
    {
        [Header("Deletion CTRL")]
        public KeyCode m_keyDeleteSpawnedResources = KeyCode.Backspace;

        [Header("Position CTRL, Rotation+Height SHIFT+CTRL")]
        public KeyCode m_keyLeft = KeyCode.LeftArrow;
        public KeyCode m_keyRight = KeyCode.RightArrow;
        public KeyCode m_keyForward = KeyCode.UpArrow;
        public KeyCode m_keyBackward = KeyCode.DownArrow;

        [Header("Light Probe Defaults")]
        public bool m_autoLightProbe = true;
        public float m_minProbeGroupDistance = Constants.MinimimProbeGroupDistance;
        public float m_minProbeDistance = Constants.MinimimProbeDistance;

        [Header("Optimization Defaults")]
        public bool m_autoOptimize = false;
        public float m_maxOptimizeSize = Constants.MaximumOptimisationSize;

        [Header("Help Defaults")]
        public bool m_showTooltips = true;
        public bool m_showDetailedHelp = true;

        /// <summary>
        /// Get the key delete event
        /// </summary>
        /// <param name="shift">Check for shift key</param>
        /// <param name="control">Check for control key</param>
        /// <returns></returns>
        public Event KeyDeleteEvent(bool shift = false, bool control = false)
        {
            Event retEvent = EventMapper(m_keyDeleteSpawnedResources, shift, control);
            if (retEvent != null)
            {
                return retEvent;
            }
            Debug.LogWarning("KeyDeleteSpawnedResources keycode not supported");
            return null;
        }

        /// <summary>
        /// Get the key left event
        /// </summary>
        /// <param name="shift">Check for shift key</param>
        /// <param name="control">Check for control key</param>
        /// <returns></returns>
        public Event KeyLeftEvent(bool shift = false, bool control = false)
        {
            Event retEvent = EventMapper(m_keyLeft, shift, control);
            if (retEvent != null)
            {
                return retEvent;
            }
            Debug.LogWarning("KeyLeft keycode not supported");
            return null;
        }

        /// <summary>
        /// Get the key right event
        /// </summary>
        /// <param name="shift">Check for shift key</param>
        /// <param name="control">Check for control key</param>
        /// <returns></returns>
        public Event KeyRightEvent(bool shift = false, bool control = false)
        {
            Event retEvent = EventMapper(m_keyRight, shift, control);
            if (retEvent != null)
            {
                return retEvent;
            }
            Debug.LogWarning("KeyRight keycode not supported");
            return null;
        }

        /// <summary>
        /// Get the key forward event
        /// </summary>
        /// <param name="shift">Check for shift key</param>
        /// <param name="control">Check for control key</param>
        /// <returns></returns>
        public Event KeyForwardEvent(bool shift = false, bool control = false)
        {
            Event retEvent = EventMapper(m_keyForward, shift, control);
            if (retEvent != null)
            {
                return retEvent;
            }
            Debug.LogWarning("KeyForward keycode not supported");
            return null;
        }

        /// <summary>
        /// Get the key backward event
        /// </summary>
        /// <param name="shift">Check for shift key</param>
        /// <param name="control">Check for control key</param>
        /// <returns></returns>
        public Event KeyBackwardEvent(bool shift = false, bool control = false)
        {
            Event retEvent = EventMapper(m_keyBackward, shift, control);
            if (retEvent != null)
            {
                return retEvent;
            }
            Debug.LogWarning("KeyBackward keycode not supported");
            return null;
        }

        /// <summary>
        /// Map key codes to usable keyboard events
        /// </summary>
        /// <param name="keyCode">Key code to map</param>
        /// <returns>Valid string or ""</returns>
        private Event EventMapper(KeyCode keyCode, bool shift = false, bool control = false)
        {
            string prefix = "";
            string keyString = "";
            if (GenaDefaults.m_keyCodeMap.TryGetValue(keyCode, out keyString))
            {
                if (shift)
                {
                    prefix += "#";
                }
                if (control)
                {
                    prefix += "^";
                }
                return Event.KeyboardEvent(prefix + keyString);
            }
            return null;
        }

        /// <summary>
        /// Keycode to string conversion - needed by keyboard event mapper
        /// </summary>
        private static Dictionary<KeyCode, string> m_keyCodeMap = new Dictionary<KeyCode, string>()
        {
            {KeyCode.Keypad0, "[0]"},
            {KeyCode.Keypad1, "[1]"},
            {KeyCode.Keypad2, "[2]"},
            {KeyCode.Keypad3, "[3]"},
            {KeyCode.Keypad4, "[4]"},
            {KeyCode.Keypad5, "[5]"},
            {KeyCode.Keypad6, "[6]"},
            {KeyCode.Keypad7, "[7]"},
            {KeyCode.Keypad8, "[8]"},
            {KeyCode.Keypad9, "[9]"},
            {KeyCode.KeypadPeriod, "[.]"},
            {KeyCode.KeypadDivide, "[/]"},
            {KeyCode.KeypadMinus, "[-]"},
            {KeyCode.KeypadPlus, "[+]"},
            {KeyCode.KeypadEquals, "[=]"},
            {KeyCode.KeypadEnter, "[enter]"},
            {KeyCode.UpArrow, "up"},
            {KeyCode.DownArrow, "down"},
            {KeyCode.LeftArrow, "left"},
            {KeyCode.RightArrow, "right"},
            {KeyCode.Insert, "insert"},
            {KeyCode.Home, "home"},
            {KeyCode.End, "end"},
            {KeyCode.PageDown, "pgdown"},
            {KeyCode.PageUp, "pgup"},
            {KeyCode.Backspace, "backspace"},
            {KeyCode.Delete, "delete"},
            {KeyCode.Tab, "tab"},
            {KeyCode.F1, "f1"},
            {KeyCode.F2, "f2"},
            {KeyCode.F3, "f3"},
            {KeyCode.F4, "f4"},
            {KeyCode.F5, "f5"},
            {KeyCode.F6, "f6"},
            {KeyCode.F7, "f7"},
            {KeyCode.F8, "f8"},
            {KeyCode.F9, "f9"},
            {KeyCode.F10, "f10"},
            {KeyCode.F11, "f11"},
            {KeyCode.F12, "f12"},
            {KeyCode.F13, "f13"},
            {KeyCode.F14, "f14"},
            {KeyCode.F15, "f15"},
            {KeyCode.Escape, "[esc]"},
            {KeyCode.Return, "return"},
            {KeyCode.Space, "space"}
        };
    }
}
