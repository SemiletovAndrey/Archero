using System;
using System.Collections.Generic;

namespace Template.Features.Backend.Http
{
    public class UnprocessableEntityException : Exception
    {
        public Dictionary<string, string> Errors { get; }

        public UnprocessableEntityException(string message, Dictionary<string, string> errors) 
            : base(message)
        {
            Errors = errors;
        }
    }
}