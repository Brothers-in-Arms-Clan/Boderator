using System;

namespace ArmaforcesMissionBot.Exceptions {
    public class NotAuthorizedException : Exception {
        private const string DefaultMessage = "User is not authorized to perform this action!";

        public NotAuthorizedException() : this(DefaultMessage) {
        }

        public NotAuthorizedException(string message) : base(message) {
        }

        public NotAuthorizedException
            (string message, Exception innerException) : base(message, innerException) {
        }
    }
}
