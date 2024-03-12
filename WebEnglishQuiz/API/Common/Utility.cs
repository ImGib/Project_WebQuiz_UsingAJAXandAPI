﻿namespace API.Common
{
    public static class Utility
    {
        public static string NowToString()
        {
            return "";
        }
        public class ResponseStatus
        {
            public string Message { get; set; } = Variables.ResponseOk;
            public object Data { get; set; } = null;
            public ResponseStatus(string message, object data)
            {
                Message = message;
                Data = data;
            }
            public ResponseStatus(string message)
            {
                Message = message;
            }
        }
        public static bool Compare(string a, string b)
        {
            a = a.ToLower().Trim();
            b = b.ToLower().Trim();
            return a.Equals(b);
        }
    }
}
