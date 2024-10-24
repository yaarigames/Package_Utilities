using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SAS.WebServiceManagement
{
    public class RequestData
    {
        public string Uri { get; set; }
        public int Retries { get; set; }
        public int? Timeout { get; set; }
        public int? RedirectLimit { get; internal set; }
        public bool IgnoreHttpException { get; internal set; }
        public bool IsAborted { get; internal set; }
        public float RetryDelay { get; internal set; }
        public object UserData { get; set; }
        public UnityWebRequest Request { get; internal set; }

        public string Method { get; set; }
        public object Body { get; set; }
        public string BodyString { get; set; }
        public byte[] BodyRaw { get; set; }

        public WWWForm Form { get; set; }
        public Dictionary<string, string> SimpleForm { get; internal set; }
        public List<IMultipartFormSection> FormSections { get; internal set; }


        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        public bool UseDefaultContentType { get; internal set; } = true;
        public bool ParseResponseBody { get; internal set; } = true;

        public string ContentType { get; internal set; }
        public CertificateHandler CertificateHandler { get; internal set; }
        public UploadHandler UploadHandler { get; internal set; }
        public DownloadHandler DownloadHandler { get; internal set; }

        public bool? ChunkedTransfer { get; internal set; }
        public bool? UseHttpContinue { get; internal set; }
        public Action<Response, int> RetryCallback { get; internal set; }
    }
}
