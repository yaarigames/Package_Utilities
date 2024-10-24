using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace SAS.WebServiceManagement
{
    public static partial class WebService
    {
        public static async Task<Response> GetAsync(string uri)
        {
            Response response = await WebRequest.DoRequestAndRetryAsync(new RequestData { Uri = uri, Method = UnityWebRequest.kHttpVerbGET });
            return response;
        }

        public static async Task<Response> GetAsync<T>(string uri)
        {
            Response response = await WebRequest.DoRequestAndRetryAsync<T>(new RequestData { Uri = uri, Method = UnityWebRequest.kHttpVerbGET });
            return response;
        }

        public static async Task<Response> GetAsync(RequestData requestData)
        {
            requestData.Method = UnityWebRequest.kHttpVerbGET;
            Response response = await WebRequest.DoRequestAndRetryAsync(requestData);
            return response;
        }

        public static async Task<Response> GetAsync<T>(RequestData requestData)
        {
            requestData.Method = UnityWebRequest.kHttpVerbGET;
            Response response = await WebRequest.DoRequestAndRetryAsync<T>(requestData);
            return response;
        }

        public static async Task<Response> PostAsync(string uri, object body)
        {
            Response response = await WebRequest.DoRequestAndRetryAsync(new RequestData { Uri = uri, Body = body, Method = UnityWebRequest.kHttpVerbPOST });
            return response;
        }

        public static async Task<Response> PostAsync(string uri, string bodyString)
        {
            Response response = await WebRequest.DoRequestAndRetryAsync(new RequestData { Uri = uri, BodyString = bodyString, Method = UnityWebRequest.kHttpVerbPOST });
            return response;
        }

        public static async Task<Response> PostAsync(RequestData requestData)
        {
            requestData.Method = UnityWebRequest.kHttpVerbPOST;
            Response response = await WebRequest.DoRequestAndRetryAsync(requestData);
            return response;
        }

        public static async Task<Response> PostAsync<T>(string uri, object body)
        {
            Response response = await WebRequest.DoRequestAndRetryAsync<T>(new RequestData { Uri = uri, Body = body, Method = UnityWebRequest.kHttpVerbPOST });
            return response;
        }

        public static async Task<Response> PostAsync<T>(string uri, string bodyString)
        {
            Response response = await WebRequest.DoRequestAndRetryAsync<T>(new RequestData { Uri = uri, BodyString = bodyString, Method = UnityWebRequest.kHttpVerbPOST });
            return response;
        }

        public static async Task<Response> PostAsync<T>(RequestData requestData)
        {
            requestData.Method = UnityWebRequest.kHttpVerbPOST;
            Response response = await WebRequest.DoRequestAndRetryAsync<T>(requestData);
            return response;
        }

        //WIP
        public static async Task<Response> PutAsync<T>(RequestData requestData)
        {
            requestData.Method = UnityWebRequest.kHttpVerbPUT;
            Response response = await WebRequest.DoRequestAndRetryAsync<T>(requestData);
            return response;
        }

        public static async Task<Response> DeleteAsync<T>(RequestData requestData)
        {
            requestData.Method = UnityWebRequest.kHttpVerbDELETE;
            Response response = await WebRequest.DoRequestAndRetryAsync<T>(requestData);
            return response;
        }

    }
}
