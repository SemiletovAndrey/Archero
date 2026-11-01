using System;

namespace Template.Scripts.DataHolders
{
    public static class ErrorFactory
    {
        public static Error CreateFromException(Exception exception)
        {
            return new Error(exception.GetType().ToString(), exception.Message);
        }
        
        public static class Purchases
        {
            public static readonly Error NotConnectedToStore = new(
                "Store.NoConnection", 
                "Not connected to store.");
            
            public static readonly Error StoreDisconnected = new(
                "Store.Disconnected", 
                "Purchase canceled because of disconnected.");
            
            public static readonly Error PurchaseFailed = new(
                "Purchase.Failed",
                "Purchase failed.");
            
            public static readonly Error PendingPurchase = new(
                "Purchase.Pending",
                "Another purchase is pending.");
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
            public static readonly Error NoUserId = new ("Auth.NoUserId", "No user id");
            public static class Email
            {
                public static readonly Error InvalidInput = new(
                    "Auth.Empty",
                    "Empty email address or password.");
                
                public static readonly Error WrongPassword = new(
                    "Auth.WrongPassword",
                    "Password does not match.");

                public static readonly Error UserAlreadyExists = new(
                    "Auth.UserAlreadyExists",
                    "Such user already exists.");
                
                public static Error Failed(Exception exception)
                {
                    return new Error(
                        "Email.SignInFailed",
                        $"{exception.Message}");
                }
            }

            public static class Google
            {
                public static readonly Error Canceled = new(
                    "Google.Cancelled",
                    $"Google sign in failed: popup closed.");
                
                public static Error Failed(Exception exception)
                {
                    return new Error(
                        "Google.SignInFailed",
                        $"Google sign in failed.");
                }
            }
        }
    }
}