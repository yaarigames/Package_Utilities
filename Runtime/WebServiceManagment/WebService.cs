using System;
using UnityEngine.Networking;
using SAS.Utilities;

namespace SAS.WebServiceManagement
{
    public static partial class WebService
    {
        public static void Get(string uri, Action<Response> response)
        {
            Get(new RequestData { Uri = uri }, response);
        }

        public static void Get<T>(string uri, Action<Response> response)
        {
            Get<T>(new RequestData { Uri = uri }, response);
        }

        public static void Get(RequestData requestData, Action<Response> response)
        {
            requestData.Method = UnityWebRequest.kHttpVerbGET;
            StaticCoroutine.Start(WebRequest.DoRequestAndRetry(requestData, response));
        }

        public static void Get<T>(RequestData requestData, Action<Response> response)
        {
            requestData.Method = UnityWebRequest.kHttpVerbGET;
            StaticCoroutine.Start(WebRequest.DoRequestAndRetry<T>(requestData, response));
        }

        public static void Post(string uri, object body, Action<Response> response)
        {
            Post(new RequestData { Uri = uri, Body = body }, response);
        }

        public static void Post(string uri, string bodyString, Action<Response> response)
        {
            Post(new RequestData { Uri = uri, BodyString = bodyString }, response);
        }

        public static void Post(RequestData requestData, Action<Response> response)
        {
            requestData.Method = UnityWebRequest.kHttpVerbPOST;
            StaticCoroutine.Start(WebRequest.DoRequestAndRetry(requestData, response));
        }

        public static void Post<T>(string uri, object body, Action<Response> response)
        {
            Post<T>(new RequestData { Uri = uri, Body = body }, response);
        }

        public static void Post<T>(string uri, string bodyString, Action<Response> response)
        {
            Post<T>(new RequestData { Uri = uri, BodyString = bodyString }, response);
        }

        public static void Post<T>(RequestData requestData, Action<Response> response)
        {
            requestData.Method = UnityWebRequest.kHttpVerbPOST;
            StaticCoroutine.Start(WebRequest.DoRequestAndRetry<T>(requestData, response));
        }
    }
}
