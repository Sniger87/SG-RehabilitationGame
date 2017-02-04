using System;
using Wii.Contracts;

//  See http://www.codeplex.com/WiimoteLib

namespace Wii.Events
{
    /// <summary>
    /// Argument sent through the WiiControllerStateChanged Event
    /// </summary>
    public class WiiControllerStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The current state of the WiiController
        /// </summary>
        public WiiControllerState WiiControllerState;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="state">WiiController state</param>
        public WiiControllerStateChangedEventArgs(WiiControllerState state)
        {
            this.WiiControllerState = state;
        }
    }
}
