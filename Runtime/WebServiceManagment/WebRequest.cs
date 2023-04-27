using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SAS.WebServiceManagment
{
    public partial class WebRequest
    {
        public static IEnumerator DoRequestAndRetry(RequestData requestPrepData, Action<Response> callback)
        {
            int retries = 0;

            using (var request = CreateRequest(requestPrepData))
            {
                request.Configure(requestPrepData);
                do
                {
                    yield return request.SendWebRequest();
                    Response response = request.CreateWebResponse();
                    if (request.IsValidRequest(requestPrepData))
                    {
                        callback(response);
                        break;
                    }
                    else if (!requestPrepData.IsAborted && retries < requestPrepData.Retries)
                    {
                        yield return new WaitForSeconds(requestPrepData.RetryDelay);
                        ++retries;
                        requestPrepData.RetryCallback?.Invoke(response, retries);
                    }
                    else
                    {
                        callback(response);
                        break;
                    }
                }
                while (retries <= requestPrepData.Retries);
            }
        }

        public static IEnumerator DoRequestAndRetry<T>(RequestData requestPrepData, Action<Response> callback)
        {
            return DoRequestAndRetry(requestPrepData, (Response response) =>
            {
                try
                {
                    DeserializeObject<T>(requestPrepData, response);
                }
                catch (Exception error)
                {
                    Debug.LogError(string.Format("Invalid JSON format\nError:{0}: {1}", error.Message, typeof(T)));
                }
                finally
                {
                    callback(response);
                }
            });
        }

        private static void DeserializeObject<T>(RequestData requestPrepData, Response response)
        {
            if (response.Data != null && requestPrepData.ParseResponseBody)
                response.LoadedObject = JsonConvert.DeserializeObject<T>(response.Text);
            else
                Debug.LogError($"Something us Wrong================ {requestPrepData.ParseResponseBody} ");

        }

        private static UnityWebRequest CreateRequest(RequestData requestData)
        {
            var url = requestData.Uri.BuildUrl(requestData.Params);
            if (requestData.Form is WWWForm && requestData.Method == UnityWebRequest.kHttpVerbPOST)
                return UnityWebRequest.Post(url, requestData.Form);
            return new UnityWebRequest(url, requestData.Method);
        }
    }
}
