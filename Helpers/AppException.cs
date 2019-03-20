using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApi.Helpers
{
    public class AppException : Exception
    {
        public AppException() : base() {}

        public AppException(string message) : base(message) { }

        public AppException(string message, params object[] args) 
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

    }
}