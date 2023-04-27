using UnityEngine;
using System.Threading.Tasks;
using System;

namespace SAS.WebServiceManagment
{
    public partial class WebRequest
    {
        public static async Task<Response> DoRequestAndRetryAsync(RequestData requestPrepData)
        {
            Response response;
            int retries = 0;
            var request = CreateRequest(requestPrepData);
            {
                request.Configure(requestPrepData);
                do
                {
                    await request.SendWebRequest();
                    response = request.CreateWebResponse();
                    if (request.IsValidRequest(requestPrepData))
                        break;
                    else if (!requestPrepData.IsAborted && retries < requestPrepData.Retries)
                    {
                        // wait for retrydelay
                        ++retries;
                        requestPrepData.RetryCallback?.Invoke(response, retries);
                    }
                    else
                        break;
                }
                while (retries <= requestPrepData.Retries);
            }
            return response;
        }

        public static async Task<Response> DoRequestAndRetryAsync<T>(RequestData requestPrepData)
        {
            Response response = await DoRequestAndRetryAsync(requestPrepData);
            try
            {
                DeserializeObject<T>(requestPrepData, response);
            }
            catch (Exception error)
            {
                Debug.LogError(string.Format("Invalid JSON format\nError:{0}: {1}", error.Message, typeof(T)));
                return response;
            }

            return response;
        }
    }
}
