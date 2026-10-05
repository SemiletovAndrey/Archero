using System;
using Template.Scripts.DataHolders;

namespace Template.Utilities.DomainErrors
{
    public static class DomainErrors
    {
        public static Error CreateFromException(Exception exception)
        {
            return new Error("CustomException", exception.Message);
        }
        
        public static class Server
        {
            public static readonly Error NotRespond = new(
                "Server.NotRespond",
                "Server is not responding");
        }

        public static class Lobby
        {
            public static readonly Error LobbyIsFull = new(
                "Lobby.Full",
                "Lobby is full.");
            
            public static readonly Error LobbyIsAlreadyStarted = new(
                "Lobby.AlreadyStarted",
                "Lobby is already started.");
        }

        public static class Auth
        {
            public static class Email
            {
                public static readonly Error WrongPassword = new(
                    "Auth.WrongPassword",
                    "Password does not match.");

                public static readonly Error UserAlreadyExists = new(
                    "Auth.UserAlreadyExists",
                    "Such user already exists.");
            }

            public static class Google
            {
                public static Error Failed(Exception exception)
                {
                    return new Error(
                        "Google.SignInFailed",
                        $"Google sign in failed: {exception.Message}");
                }
            }
            
            public static class Apple
            {
                public static Error Failed(Exception exception)
                {
                    return new Error(
                        "Apple.SignInFailed",
                        $"Apple sign in failed: {exception.Message}");
                }
            }
        }
    }
}