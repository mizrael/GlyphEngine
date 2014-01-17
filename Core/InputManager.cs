#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GlyphEngine.Core
{
    public enum eInputState
    {
        Up,
        Down,
        Clicked,
        Released,
        NoMatch
    };

    public enum MouseButtons
    {
        Left,
        Right,
        Middle
    };

    public static class InputManager
    {
        #region Fields

        private static KeyboardState _currKeyboardState = Keyboard.GetState();
        private static KeyboardState _lastKeyboardState = Keyboard.GetState();

        private static MouseState _currMouseState = Mouse.GetState();
        private static MouseState _lastMouseState = Mouse.GetState();

        private static Dictionary<String, Keys> _keyboardConfig = new Dictionary<String, Keys>();

        #endregion

        public static void Update()
        {
            _lastKeyboardState = _currKeyboardState;
            _currKeyboardState = Keyboard.GetState();

            _lastMouseState = _currMouseState;
            _currMouseState = Mouse.GetState();

            MousePosition = new Point(_currMouseState.X, _currMouseState.Y);
            MousePositionVec = new Vector2(_currMouseState.X, _currMouseState.Y);
        }

        public static void AssignKey(string command, Keys key)
        {
            if (_keyboardConfig.ContainsKey(command))
                _keyboardConfig[command] = key;
            else
                _keyboardConfig.Add(command, key);
        }

        public static eInputState CheckKeyState(string command)
        {
            Keys currKey;
            if(!_keyboardConfig.TryGetValue(command, out currKey))
                return eInputState.NoMatch;

            return CheckKeyState(currKey);
        }

        public static eInputState CheckKeyState(Keys k)
        {
            eInputState returnValue;

            if (_currKeyboardState.IsKeyDown(k))
                returnValue = eInputState.Down;
            else if (_lastKeyboardState.IsKeyDown(k))
                returnValue = eInputState.Released;
            else
                returnValue = eInputState.Up;

            return returnValue;
        }

        public static eInputState CheckMouseState(MouseButtons mb)
        {
            switch (mb)
            {
                case MouseButtons.Left:
                    if (_currMouseState.LeftButton == ButtonState.Pressed &&
                        _lastMouseState.LeftButton == ButtonState.Released)
                        return eInputState.Clicked;
                    if(_currMouseState.LeftButton == ButtonState.Pressed)
                        return eInputState.Down;
                    if (_lastMouseState.LeftButton == ButtonState.Pressed)
                        return eInputState.Released;
                    break;
                case MouseButtons.Right:
                    if (_currMouseState.RightButton == ButtonState.Pressed &&
                       _lastMouseState.RightButton == ButtonState.Released)
                        return eInputState.Clicked; 
                    if (_currMouseState.RightButton == ButtonState.Pressed)
                        return eInputState.Down;
                    if (_lastMouseState.RightButton == ButtonState.Pressed)
                        return eInputState.Released;
                    break;
                case MouseButtons.Middle:
                    if (_currMouseState.MiddleButton == ButtonState.Pressed &&
                       _lastMouseState.MiddleButton == ButtonState.Released)
                        return eInputState.Clicked; 
                    if (_currMouseState.MiddleButton == ButtonState.Pressed)
                        return eInputState.Down;
                    if (_lastMouseState.MiddleButton == ButtonState.Pressed)
                        return eInputState.Released;
                    break;
            }


            return eInputState.Up;
        }

        #region Properties

        public static Point MousePosition;

        public static Vector2 MousePositionVec;

        #endregion Properties
    }
}
