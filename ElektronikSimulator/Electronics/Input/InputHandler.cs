using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ESim.Input
{
    public struct Drag
    {
        public Drag(Vector2 start)
        {
            this.start = start;
            current = start;
            end = null;
        }

        public Vector2 start;
        public Vector2 current;
        public Vector2? end;
    }

    /// <summary>
    /// A list of mouse buttons.
    /// </summary>
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    /// <summary>
    /// A class for input handling.
    /// </summary>
    public static class InputHandler
    {
        #region Variables
        private static readonly float DoubleClickTime = 250f;

        private static MouseState _currentMouseState, _lastMouseState;
        private static KeyboardState _currentKeyboardState, _lastKeyboardState;

        private static float _doubleClickTimer;
        private static bool _doubleClicked;

        private static Vector2 _mouseAnchor;
        private static bool _mouseAnchored;

        private static Dictionary<MouseButton, Drag> _mouseDrags;
        #endregion

        /// <summary>
        /// Methods for polling current input states.
        /// </summary>
        #region InputPolling

        public static bool FixedMousePosDrag { get; set; }

        public static Vector2 MousePosition { get; private set; }
        public static Vector2 DeltaMousePosition { get; private set; }
        public static float ScrollWheelPosition { get; private set; }
        public static float DeltaScrollWheel { get; private set; }

        public static bool IsKeyPressed(Keys key) => _currentKeyboardState.IsKeyDown(key);

        public static bool IsKeyJustPressed(Keys key) => _currentKeyboardState.IsKeyDown(key) && !_lastKeyboardState.IsKeyDown(key);

        public static bool IsKeyJustReleased(Keys key) => !_currentKeyboardState.IsKeyDown(key) && _lastKeyboardState.IsKeyDown(key);

        public static bool IsMouseButtonPressed(MouseButton button) => GetMouseButtonPressed(_currentMouseState, button);

        public static bool IsMouseButtonJustPressed(MouseButton button) => GetMouseButtonPressed(_currentMouseState, button) && !GetMouseButtonPressed(_lastMouseState, button);

        public static bool IsMouseButtonJustReleased(MouseButton button) => !GetMouseButtonPressed(_currentMouseState, button) && GetMouseButtonPressed(_lastMouseState, button);

        public static bool DoubleClicked() => _doubleClicked;

        public static bool IsMouseAnchored() => _mouseAnchored;

        public static Drag? GetDrag(MouseButton button)
        {
            if (_mouseDrags.TryGetValue(button, out Drag drag))
                return drag;
            else
                return null;
        }

        #endregion InputPolling

        /// <summary>
        /// Initializes the input handler.
        /// </summary>
        public static void Initialize()
        {
            _currentMouseState = Mouse.GetState();
            _currentKeyboardState = Keyboard.GetState();
            _doubleClickTimer = 0;
            _mouseAnchored = false;
            _mouseDrags = new Dictionary<MouseButton, Drag>(Enum.GetValues(typeof(MouseButton)).Length);
        }

        /// <summary>
        /// Updates the mouse and keyboard states.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        public static void Update(GameTime gameTime)
        {
            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (_doubleClickTimer > 0)
                _doubleClickTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                _doubleClicked = false;

            if (IsMouseButtonJustPressed(MouseButton.Left))
            {
                if (_doubleClickTimer > 0)
                    _doubleClicked = true;
                else
                    _doubleClickTimer = DoubleClickTime;
            }

            MousePosition = _currentMouseState.Position.ToVector2();

            foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
                CheckDrag(button);

            if (_mouseAnchored)
                DeltaMousePosition = _mouseAnchor - MousePosition;
            else
                DeltaMousePosition = _lastMouseState.Position.ToVector2() - MousePosition;

            ScrollWheelPosition = _currentMouseState.ScrollWheelValue;
            DeltaScrollWheel = ScrollWheelPosition - _lastMouseState.ScrollWheelValue;

            if (_mouseAnchored)
                Mouse.SetPosition((int)_mouseAnchor.X, (int)_mouseAnchor.Y);
        }


        /// <summary>
        /// Anchores the mouse at a position.
        /// </summary>
        /// <param name="anchor">The anchor position.</param>
        public static void SetAnchor(Vector2 anchor)
        {
            _mouseAnchor = anchor;
            _mouseAnchored = true;
        }

        /// <summary>
        /// Sets the mouse position.
        /// </summary>
        /// <param name="updateStates">Set the last mouse state to the set position so that the delta position is accurate.</param>
        /// <param name="position">The position.</param>
        public static void SetMousePosition(bool updateStates, Vector2 position)
        {
            Mouse.SetPosition((int)position.X, (int)position.Y);

            if (updateStates)
            {
                _currentMouseState = Mouse.GetState();
                _lastMouseState = _currentMouseState;
            }
        }

        /// <summary>
        /// Anchores the mouse at a position.
        /// </summary>
        /// <param name="x">The x value of the position.</param>
        /// <param name="y">The y value of the position.</param>
        public static void SetAnchor(float x, float y)
        {
            _mouseAnchor = new Vector2(x, y);
            _mouseAnchored = true;
        }

        /// <summary>
        /// Releases the anchor.
        /// </summary>
        public static void ReleaseAnchor() => _mouseAnchored = false;

        /// <summary>
        /// Projects the mouse position to world space.
        /// </summary>
        /// <param name="projection">The cameras projection matrix.</param>
        /// <param name="view">The cameras view matrix.</param>
        /// <param name="graphics">The <see cref="GraphicsDeviceManager"/>.</param>
        /// <returns>The direction of the mouse in world space.</returns>
        public static Vector3 UpdateWorldSpace(Matrix projection, Matrix view, GraphicsDeviceManager graphics)
        {
            Vector3 nearSource = new Vector3(MousePosition, 0f);
            Vector3 farSource = new Vector3(MousePosition, 1f);
            Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearSource, projection, view, Matrix.Identity);
            Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farSource, projection, view, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            
            return direction;
        }

        /// <summary>
        /// Checks if the specified MouseButton is pressed.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <param name="button">The button to check.</param>
        /// <returns>Is the mouse button pressed.</returns>
        private static bool GetMouseButtonPressed(MouseState state, MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return state.LeftButton == ButtonState.Pressed;

                case MouseButton.Right:
                    return state.RightButton == ButtonState.Pressed;

                case MouseButton.Middle:
                    return state.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }

        /// <summary>
        /// Updates the drag event of a specific button.
        /// </summary>
        /// <param name="button">The button to check.</param>
        private static void CheckDrag(MouseButton button)
        {
            Drag drag;
            bool hasValue;

            if (hasValue = _mouseDrags.TryGetValue(button, out drag))
            {
                if (drag.end.HasValue)
                {
                    _mouseDrags.Remove(button);
                    return;
                }
            }

            if (IsMouseButtonPressed(button))
            {
                if (hasValue)
                    drag.current = MousePosition;
                else
                {
                    if (!IsMouseAnchored() && FixedMousePosDrag)
                        SetAnchor(MousePosition);

                    _mouseDrags.Add(button, new Drag(MousePosition));
                }
            }
            else if (IsMouseButtonJustReleased(button))
            {
                if (hasValue)
                {
                    drag.end = MousePosition;
                    if (drag.start == _mouseAnchor && FixedMousePosDrag)
                        ReleaseAnchor();
                }
            }

            if (hasValue)
                _mouseDrags[button] = drag;
        }
    }
}