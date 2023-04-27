using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SAS.WebServiceManagment
{
    public static class Extensions
    {
        private const string ContentTypeHeader = "Content-Type";
        private const string DefaultContentType = "application/json";

        public static void Configure(this UnityWebRequest request, RequestData requestPrepData)
        {
            byte[] bodyRaw = requestPrepData.BodyRaw;
            string contentType;
            if (!requestPrepData.Headers.TryGetValue(ContentTypeHeader, out contentType) && requestPrepData.UseDefaultContentType)
                contentType = DefaultContentType;

            if (requestPrepData.Body != null || !string.IsNullOrEmpty(requestPrepData.BodyString))
            {
                string bodyString = requestPrepData.BodyString;
                if (requestPrepData.Body != null)
                    bodyString = JsonUtility.ToJson(requestPrepData.Body);
                bodyRaw = Encoding.UTF8.GetBytes(bodyString.ToCharArray());
            }
            else if (requestPrepData.SimpleForm != null && requestPrepData.SimpleForm.Count > 0)
            {
                UnityWebRequest.SerializeSimpleForm(requestPrepData.SimpleForm);
                contentType = "application/x-www-form-urlencoded";
            }
            else if (requestPrepData.FormSections != null && requestPrepData.FormSections.Count > 0)
                contentType = GetFormSectionsContentType(out bodyRaw, requestPrepData);
            else if (requestPrepData.Form is WWWForm)
                contentType = string.Empty;

            if (!string.IsNullOrEmpty(requestPrepData.ContentType))
                contentType = requestPrepData.ContentType;

            if (requestPrepData.CertificateHandler is CertificateHandler)
                request.certificateHandler = requestPrepData.CertificateHandler;

            if (requestPrepData.UploadHandler is UploadHandler)
                request.uploadHandler = requestPrepData.UploadHandler;
            if (bodyRaw != null)
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.uploadHandler.contentType = contentType;
            }

            if (requestPrepData.DownloadHandler is DownloadHandler)
            {
                request.downloadHandler = requestPrepData.DownloadHandler;
                requestPrepData.ParseResponseBody = requestPrepData.DownloadHandler is DownloadHandlerBuffer;
            }
            else
                request.downloadHandler = new DownloadHandlerBuffer();

            if (!string.IsNullOrEmpty(contentType))
                request.SetRequestHeader(ContentTypeHeader, contentType);

            foreach (KeyValuePair<string, string> header in requestPrepData.Headers)
                request.SetRequestHeader(header.Key, header.Value);

            if (requestPrepData.Timeout.HasValue)
                request.timeout = requestPrepData.Timeout.Value;

            if (requestPrepData.UseHttpContinue.HasValue)
                request.useHttpContinue = requestPrepData.UseHttpContinue.Value;

            if (requestPrepData.RedirectLimit.HasValue)
                request.redirectLimit = requestPrepData.RedirectLimit.Value;

            requestPrepData.Request = request;
        }

        private static string GetFormSectionsContentType(out byte[] bodyRaw, RequestData requestPrepData)
        {
            //TODO:
            bodyRaw = null;
            return null;
        }

        public static Response CreateWebResponse(this UnityWebRequest request)
        {
            return new Response(request);
        }

        public static string BuildUrl(this string uri, Dictionary<string, string> queryParam)
        {
            string url = uri;
            if (queryParam.Any())
            {
                var urlParamKey = queryParam.Keys;
                url += (url.Contains("?") ? "&" : "?") + string.Join("&", queryParam.Select(p => string.Format("{0}={1}", p.Key, p.Value.EscapeURL())).ToArray());
            }
            return url;
        }

        public static string EscapeURL(this string queryParam)
        {
            return UnityWebRequest.EscapeURL(queryParam);
        }

        public static bool IsValidRequest(this UnityWebRequest request, RequestData requestData)
        {
            return request.isDone && !request.isNetworkError && (!request.isHttpError || requestData.IgnoreHttpException);
        }
    }
}
