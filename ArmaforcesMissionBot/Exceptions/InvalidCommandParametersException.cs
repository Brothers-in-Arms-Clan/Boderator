using System;

namespace ArmaforcesMissionBot.Exceptions {
    public class InvalidCommandParametersException : Exception {
        private const string DefaultMessage = "Invalid command parameters given!";

        public InvalidCommandParametersException() : this(DefaultMessage)
        {
        }

        public InvalidCommandParametersException(string message) : base(message)
        {
        }

        public InvalidCommandParametersException
            (string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
