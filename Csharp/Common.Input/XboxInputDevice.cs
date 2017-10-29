using System;
using SharpDX.XInput;

namespace Common.Input
{
    /// <summary>
    /// Defines an XBox controller input device.
    /// </summary>
    /// <remarks>
    /// Reading any of the properties will reset that property to false (if true),
    /// as they correspond to physical buttons. Ideally I'd refactor this to have
    /// a series of 'is pressed', 'is released' queries and also allow for event-based
    /// triggers (when pressed, do this), but for the immediate refactor this will have to do.
    /// </remarks>
    public class XboxInputDevice
    {
        private Controller controller;
        private State lastState;

        // Buttons
        private bool xPressed = false;
        private bool yPressed = false;
        private bool aPressed = false;
        private bool bPressed = false;
        private bool leftBumperPressed = false;
        private bool rightBumperPressed = false;
        private bool leftPadPressed = false;
        private bool rightPadPressed = false;
        private bool upPadPressed = false;
        private bool downPadPressed = false;

        private bool startPressed = false;

        // Analog controls
        private short leftX;
        private short leftY;

        private short rightX;
        private short rightY;

        private byte triggerLeft;
        private byte triggerRight;

        public XboxInputDevice()
        {
            controller = new Controller(UserIndex.One);
            if (!controller.IsConnected)
            {
                throw new ArgumentException("At least one Xbox-known controller must be connected to player one");
            }

            lastState = controller.GetState();
            UpdateSticks(lastState);
        }

        public short LeftX => leftX;
        public short LeftY => leftY;

        public short RightX => rightX;
        public short RightY => rightY;

        public byte TriggerLeft => triggerLeft;
        public byte TriggerRight => triggerRight;

        public bool XPressed
        {
            get
            {
                if (xPressed)
                {
                    xPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool YPressed
        {
            get
            {
                if (yPressed)
                {
                    yPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool APressed
        {
            get
            {
                if (aPressed)
                {
                    aPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool BPressed
        {
            get
            {
                if (bPressed)
                {
                    bPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool LeftBumperPressed
        {
            get
            {
                if (leftBumperPressed)
                {
                    leftBumperPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool RightBumperPressed
        {
            get
            {
                if (rightBumperPressed)
                {
                    rightBumperPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool LeftPadPressed
        {
            get
            {
                if (leftPadPressed)
                {
                    leftPadPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool RightPadPressed
        {
            get
            {
                if (rightPadPressed)
                {
                    rightPadPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool UpPadPressed
        {
            get
            {
                if (upPadPressed)
                {
                    upPadPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool DownPadPressed
        {
            get
            {
                if (downPadPressed)
                {
                    downPadPressed = false;
                    return true;
                }
                return false;
            }
        }

        public bool StartPressed
        {
            get
            {
                if (startPressed)
                {
                    startPressed = false;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Updates the internal state based on what the controller observes.
        /// </summary>
        public void UpdateState()
        {
            State gamepadState = controller.GetState();
            if (gamepadState.PacketNumber == lastState.PacketNumber)
            {
                // No gamepad delta
                return;
            }

            // Set the button booleans if there have been rising-edge events.
            GamepadButtonFlags flags = gamepadState.Gamepad.Buttons;
            CheckButton(GamepadButtonFlags.X, flags, ref xPressed);
            CheckButton(GamepadButtonFlags.Y, flags, ref yPressed);
            CheckButton(GamepadButtonFlags.A, flags, ref aPressed);
            CheckButton(GamepadButtonFlags.B, flags, ref bPressed);

            CheckButton(GamepadButtonFlags.LeftShoulder, flags, ref leftBumperPressed);
            CheckButton(GamepadButtonFlags.RightShoulder, flags, ref rightBumperPressed);

            CheckButton(GamepadButtonFlags.DPadLeft, flags, ref leftPadPressed);
            CheckButton(GamepadButtonFlags.DPadRight, flags, ref rightPadPressed);
            CheckButton(GamepadButtonFlags.DPadUp, flags, ref upPadPressed);
            CheckButton(GamepadButtonFlags.DPadDown, flags, ref downPadPressed);

            CheckButton(GamepadButtonFlags.Start, flags, ref startPressed);

            UpdateSticks(gamepadState);

            lastState = gamepadState;
        }

        private void UpdateSticks(State gamepadState)
        {
            leftX = gamepadState.Gamepad.LeftThumbX;
            leftY = gamepadState.Gamepad.LeftThumbY;

            rightX = gamepadState.Gamepad.RightThumbX;
            rightY = gamepadState.Gamepad.RightThumbY;

            triggerLeft = gamepadState.Gamepad.LeftTrigger;
            triggerRight = gamepadState.Gamepad.RightTrigger;
        }

        private void CheckButton(GamepadButtonFlags button, GamepadButtonFlags newFlags, ref bool flagToTrigger)
        {
            GamepadButtonFlags lastFlags = lastState.Gamepad.Buttons;
            if (newFlags.HasFlag(button))
            {
                if (!lastFlags.HasFlag(button))
                {
                    flagToTrigger = true;
                }
            }
        }
    }
}

