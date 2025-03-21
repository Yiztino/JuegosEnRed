using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chat : MonoBehaviour
{

    public string baseUrl = "http://chat_u3d.test/chat.php";

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void getRooms ()
    {
        StartCoroutine(GetRequest(baseUrl+"?action=1"));
    }

    public void getMessages(string room)
    {
        StartCoroutine(GetRequest(baseUrl + "?action=2&room="+room));
    }

    public void sendAnimeMessage(string message)
    {
        string room = "anime",
            user = "frank";
        StartCoroutine(GetRequest(baseUrl + "?action=3&room="+room+"&username="+user+"&message="+message));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(uri+"\nError: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(uri + "\nHTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(uri + "\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}
