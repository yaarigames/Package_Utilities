using System;
using UnityEngine.Networking;

namespace SAS.WebServiceManagement
{
    public class Response
    {
        public UnityWebRequest Request { get; private set; }
        public long StatusCode { get => Request.responseCode; }
        public bool IsHttpError { get => Request.result== UnityWebRequest.Result.ProtocolError; }
        public bool IsNetworkError { get => Request.result == UnityWebRequest.Result.ConnectionError; }
        public string Error { get => Request.error; }

        public byte[] Data
        {
            get
            {
                try
                {
                    return Request.downloadHandler.data;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string Text
        {
            get
            {
                try
                {
                    return Request.downloadHandler.text;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public object LoadedObject { get; set; }


        public Response(UnityWebRequest request)
        {
            Request = request;
        }
    }
}
