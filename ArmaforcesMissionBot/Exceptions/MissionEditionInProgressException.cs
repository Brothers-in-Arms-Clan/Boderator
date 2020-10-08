using System;

namespace ArmaforcesMissionBot.Exceptions {
    public class MissionEditionInProgressException : Exception {
        private const string DefaultMessage = "Mission edition in progress!";

        public MissionEditionInProgressException() : this(DefaultMessage) {
        }

        public MissionEditionInProgressException(string message) : base(message) {
        }

        public MissionEditionInProgressException
            (string message, Exception innerException) : base(message, innerException) {
        }
    }
}
