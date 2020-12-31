using System;

namespace ArmaforcesMissionBot.Exceptions {
    public class CommandNotFound : Exception {
        private const string DefaultMessage = "Command with given name was not found";

        public CommandNotFound() : this(DefaultMessage)
        {
        }

        public CommandNotFound(string message) : base(message)
        {
        }

        public CommandNotFound
            (string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
