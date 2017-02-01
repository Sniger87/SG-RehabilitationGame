using System;

namespace Wii.Input.Contracts
{
    /// <summary>
    /// Current overall state of the WiiController and all attachments
    /// </summary>
    [Serializable]
    [DataContract]
    public class WiiControllerState
    {
        /// <summary>
        /// Current state of buttons
        /// </summary>
        [DataMember]
        public ButtonState ButtonState;
        /// <summary>
        /// Raw byte value of current battery level
        /// </summary>
        [DataMember]
        public byte BatteryRaw;
        /// <summary>
        /// Calculated current battery level
        /// </summary>
        [DataMember]
        public float Battery;
        /// <summary>
        /// Current state of the Wii Fit Balance Board
        /// </summary>
        public BalanceBoardState BalanceBoardState;
        /// <summary>
        /// Current state of LEDs
        /// </summary>
        [DataMember]
        public LEDState LEDState;

        /// <summary>
        /// Constructor for WiimoteState class
        /// </summary>
        public WiiControllerState()
        {
        }
    }
}
