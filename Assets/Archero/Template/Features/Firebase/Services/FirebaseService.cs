using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Template.Features.Firebase.Services
{
    public class FirebaseService : IFirebaseService
    {
        public bool IsFirebaseInitialized { get; private set; }
        public FirebaseAuth Auth { get; private set; }
        
        public FirebaseService()
        {

        }
        
        public async UniTask InitAsync()
        {
            await InitializeFirebase();
        }
        
        public async UniTask InitializeFirebase()
        {
            DependencyStatus dependencyStatus =
                await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus != DependencyStatus.Available)
                throw new Exception("Firebase init failed");

            IsFirebaseInitialized = true;
            Auth = FirebaseAuth.DefaultInstance;

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            Debug.Log($"CurrentUser: {Auth.CurrentUser?.UserId}");

            if (Auth.CurrentUser != null)
            {
                try
                {
                    await Auth.CurrentUser.TokenAsync(false);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Refresh token warning: {e.Message}");
                }
            }
        }

    }
}