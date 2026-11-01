using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Template.Features.Backend.Http;
using Template.Features.Firebase.Services;
using UnityEngine;

namespace Persistence.Features.Auth.Scripts
{
    public class FirebaseServerTokenProvider : IServerTokenProvider
    {
        private readonly IFirebaseService _firebaseService;

        private string _cachedToken;
        private string _cachedUserId;
        private DateTime _lastRefreshUtc;
        private UniTask<string> _refreshInFlight;
        private bool _isRefreshing;
        
        
        private string _token;
        private string _tokenUserId;
        private DateTime _lastTokenRefresh = DateTime.MinValue;
        private float _minutesBeforeTokenRefresh = 10;

        public FirebaseServerTokenProvider(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }
        
        public async UniTask<string> GetServerToken()
        {
            FirebaseUser user = _firebaseService.Auth.CurrentUser;

            if (user == null)
            {
                Debug.LogError("User is not logged in.");
                throw new Exception("User is not authenticated.");
            }

            bool sameUser = user.UserId == _tokenUserId;
            bool tokenFresh = (DateTime.UtcNow - _lastTokenRefresh).TotalMinutes < _minutesBeforeTokenRefresh;

            if (sameUser && tokenFresh)
                return _token;

            try
            {
                _token = await user.TokenAsync(true);
                _tokenUserId = user.UserId;
                _lastTokenRefresh = DateTime.UtcNow;

                return _token;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to refresh token: {e.Message}");
                throw;
            }
        }
    }
}