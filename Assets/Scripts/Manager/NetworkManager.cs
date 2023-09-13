using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Seunghak.Common
{
    public class NetworkManager : UnitySingleton<NetworkManager>
    {
        //서버 URL의 경우는 웹 서버 주소에 따라 세팅 처음 로그인할때 웹서버 위치 받아올것
        [SerializeField] private string serverUrl;
        public void SendServerMessage(string message,Action<string> onComplete = null,Action onError = null)
        {
            Dictionary<string, string> formData = new Dictionary<string, string>();
            //formData.Add("MessageType", id.ToString());
            //formData.Add("MessageText", title);
            //
            StartCoroutine(LoadURLWithMethod(formData, onComplete, onError));
            //이것뿐만 아니라, 기본적으로 세팅된 값을 받아서, 갱신 해주는게 필요하다
            //기본 클래스에는 현재 서버 시간과, 유저 통신 시간등을 넣어준다
        }
        public IEnumerator LoadURLWithMethod( Dictionary<string, string> formData, Action<string> onComplete = null, Action onIOError = null)
        {
            //PanelManager.Instance.panelWaiting.SetActive(true);
            UnityWebRequest www = UnityWebRequest.Post(serverUrl, formData);

            if (null == www)
            {
                yield break;
            }

            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Text:" + responseText);
                if (onIOError != null)
                {
                    onIOError();
                }
            }
            else
            {
                string responseText = www.downloadHandler.text;
                if (onComplete != null)
                {
                    onComplete(responseText);
                }
            }

            www.Dispose();
        }
    }
}
