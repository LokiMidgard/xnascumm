using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Scumm.Engine
{
    public class EventManager
    {
        #region Fields

        KeyboardState keyboardState;
        GamePadState[] gamePadState;

        KeyboardState lastKeyboardState;
        GamePadState[] lastGamePadState;

        #endregion

        #region Properties

        public bool GamePadConnected(int player)
        {
            return gamePadState[player].IsConnected;
        }

        public Vector2 GamePadLeftMovement(int player)
        {
            // gamepad
            if (GamePadConnected(player))
                return gamePadState[player].ThumbSticks.Left;

            // keyboard
            else
            {
                float x = 0.0f, y = 0.0f;

                //if (Pressing(GamePad.Buttons.LeftThumbstickLeft]))
                //    x = -1.0f;
                //if (Pressing(GameInfo.keyConfig[player][Buttons.LeftThumbstickRight]))
                //    x = 1.0f;
                //if ((Pressing(GameInfo.keyConfig[player][Buttons.LeftThumbstickLeft])) &&
                //    (Pressing(GameInfo.keyConfig[player][Buttons.LeftThumbstickRight])))
                //    x = 0.0f;
                //
                //if (Pressing(GameInfo.keyConfig[player][Buttons.LeftThumbstickUp]))
                //    y = 1.0f;
                //if (Pressing(GameInfo.keyConfig[player][Buttons.LeftThumbstickDown]))
                //    y = -1.0f;
                //if ((Pressing(GameInfo.keyConfig[player][Buttons.LeftThumbstickUp])) &&
                //    (Pressing(GameInfo.keyConfig[player][Buttons.LeftThumbstickDown])))
                //    y = 0.0f;

                return new Vector2(x, y);

            }

        }

        public Vector2 GamePadRightMovement(int player)
        {
            return gamePadState[player].ThumbSticks.Left;
        }

        public float GameRightTrigger(int player)
        {
            return gamePadState[player].Triggers.Right;
        }

        #endregion

        #region Initialization

        public EventManager()
        {
            keyboardState = new KeyboardState();
            gamePadState = new GamePadState[2];

            lastKeyboardState = new KeyboardState();
            lastGamePadState = new GamePadState[2];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad
        /// </summary>
        public void Update()
        {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastGamePadState[0] = gamePadState[0];
            lastGamePadState[1] = gamePadState[1];

            gamePadState[0] = GamePad.GetState(PlayerIndex.One);
            gamePadState[1] = GamePad.GetState(PlayerIndex.Two);
        }

        public bool Pressing(int player, Buttons button)
        {
            // gamepad
            if (GamePadConnected(player))
                return gamePadState[player].IsButtonDown(button);
            // keyboard
            else
                return false; // Pressing(GameInfo.keyConfig[player][button]);
        }
        private bool Pressing(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public bool Clicked(int player, Buttons button)
        {
            // gamepad
            if (GamePadConnected(player))
                return gamePadState[player].IsButtonUp(button) && lastGamePadState[player].IsButtonDown(button);

            // keyboard
            else
                return false;// Clicked(GameInfo.keyConfig[player][button]);
        }
        public bool Clicked(Keys key)
        {
            bool up = keyboardState.IsKeyUp(key);
            bool down = lastKeyboardState.IsKeyDown(key);
            return up && down;
        }

        public bool MovedLeftUp(int player)
        {
            // gamepad
            if (GamePadConnected(player))
                return (gamePadState[player].ThumbSticks.Left.Y > 0) &&
                       (lastGamePadState[player].ThumbSticks.Left.Y == 0);

            // keyboard
            else
                return false; // Clicked(GameInfo.keyConfig[player][Buttons.LeftThumbstickUp]);
        }
        public bool MovedLeftDown(int player)
        {
            // gamepad
            if (GamePadConnected(player))
                return (gamePadState[player].ThumbSticks.Left.Y < 0) &&
                       (lastGamePadState[player].ThumbSticks.Left.Y == 0);

            // keyboard
            else
                return false; // Clicked(GameInfo.keyConfig[player][Buttons.LeftThumbstickDown]);
        }
        public bool MovedLeftLeft(int player)
        {
            // gamepad
            if (GamePadConnected(player))
                return (gamePadState[player].ThumbSticks.Left.X < 0) &&
                       (lastGamePadState[player].ThumbSticks.Left.X == 0);

            // keyboard
            else
                return false;// Clicked(GameInfo.keyConfig[player][Buttons.LeftThumbstickLeft]);
        }
        public bool MovedLeftRight(int player)
        {
            // gamepad
            if (GamePadConnected(player))
                return (gamePadState[player].ThumbSticks.Left.X > 0) &&
                       (lastGamePadState[player].ThumbSticks.Left.X == 0);

            // keyboard
            else
                return false; // Clicked(GameInfo.keyConfig[player][Buttons.LeftThumbstickRight]);
        }

        #endregion
    }
}
