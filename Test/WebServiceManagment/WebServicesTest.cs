using System;
using UnityEngine;

namespace SAS.WebServiceManagement
{
    public class CreateRoom_ReqBody
    {
        public bool IsProtected;
        public int Pin;
        public int Size;
        public string RoomName;
        public string PlayerName;
    }

    [System.Serializable]
    public class CreateRoom_ResponeBody
    {
        public string PlayerId;
        public string Token;
    }

    public class WebServicesTest : MonoBehaviour
    {

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                DoPostServiceAsync();
        }

        private void DoPostService()
        {
            CreateRoom_ReqBody pCreateRoom_ReqBody = new CreateRoom_ReqBody();
            pCreateRoom_ReqBody.PlayerName = "Abhishek";
            pCreateRoom_ReqBody.RoomName = "SessionName21";
            pCreateRoom_ReqBody.IsProtected = true;
            pCreateRoom_ReqBody.Pin = Int32.Parse("123");
            pCreateRoom_ReqBody.Size = 4;

            RequestData requestData = new RequestData();
            requestData.Uri = "https://detectiveapi.selwe.com/api/FreePlayer/CreateRoom";
            requestData.BodyString = Newtonsoft.Json.JsonConvert.SerializeObject(pCreateRoom_ReqBody);
            WebService.Post(requestData, OnLoaded);
        }

        void OnLoaded(Response response)
        {
            Debug.Log(response.IsHttpError + "    " + response.IsNetworkError + "    " + response.Text);
        }

        private async void DoPostServiceAsync()
        {
            CreateRoom_ReqBody pCreateRoom_ReqBody = new CreateRoom_ReqBody();
            pCreateRoom_ReqBody.PlayerName = "Abhishek";
            pCreateRoom_ReqBody.RoomName = "SessionName12212z3";
            pCreateRoom_ReqBody.IsProtected = true;
            pCreateRoom_ReqBody.Pin = Int32.Parse("123");
            pCreateRoom_ReqBody.Size = 4;

            RequestData requestData = new RequestData();
            requestData.Uri = "https://detectiveapi.selwe.com/api/FreePlayer/CreateRoom";
            requestData.Body = pCreateRoom_ReqBody;
            Response response = await WebService.PostAsync<CreateRoom_ResponeBody>(requestData);
            Debug.Log((response.LoadedObject as CreateRoom_ResponeBody).PlayerId);
        }
    }
}
