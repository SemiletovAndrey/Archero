using Template.Features.Backend.Http;
using UnityEngine;

namespace Template.Features.Backend.Features.Http
{
    public static class ApiLogger
    {
        public static void LogUnprocessableEntityException(UnprocessableEntityException ex)
        {
            Debug.LogError($"Validation failed: {ex.Message}");
        
            foreach (var error in ex.Errors)
            {
                Debug.LogError($"Error field: {error.Key} - {error.Value}");
            }
        }
    }
}