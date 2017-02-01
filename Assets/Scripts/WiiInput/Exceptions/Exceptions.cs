using System;
using System.Runtime.Serialization;

namespace Wii.Input.Exceptions
{
    /// <summary>
    /// Thrown when no WiiControllers are found in the HID device list
    /// </summary>
    [Serializable]
    public class WiiControllerNotFoundException : ApplicationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WiiControllerNotFoundException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public WiiControllerNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public WiiControllerNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected WiiControllerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Represents errors that occur during the execution of the WiiController library
    /// </summary>
    [Serializable]
    public class WiiControllerException : ApplicationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WiiControllerException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public WiiControllerException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public WiiControllerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected WiiControllerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
