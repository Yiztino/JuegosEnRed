using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class NetworkManager : MonoBehaviour
{
    private string baseURL = "http://localhost/gato/gato.php";
    public static NetworkManager Instance;
    public string lastReceivedData;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        
    }

   
    public IEnumerator GetState()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "?action=2");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success) { Debug.LogError("Error en GetState: " + www.error); }
        else
        {
            lastReceivedData = www.downloadHandler.text;
            Debug.Log("NetworkManager: Resultado de obtener estado:" + lastReceivedData);
        }

    }

    public IEnumerator Tirada(int idPlayer, int pos)
    {
        Debug.Log("Solo por si acaso: " + baseURL + $"?action=3&id=id{idPlayer}&pos={pos}");
        UnityWebRequest www = UnityWebRequest.Get(baseURL + $"?action=3&id=id{idPlayer}&pos={pos}");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success/*www.isNetworkError*/)
        {
            Debug.Log(www.error);

        }
        else
        {
            // Verifica si la tirada fue exitosa y después obtiene el estado del juego.
            string json = www.downloadHandler.text;
            Debug.Log("Resultado de la tirada: " + json);

            // Llama a GetState después de confirmar que la tirada fue exitosa.
            if (json.Contains("OK se colocó la ficha y sigue el juego"))
            {
                
                yield return StartCoroutine(GetState());
            }
            else if(json.Contains("ok, gajó el winner: 1"))
            {
                yield return StartCoroutine(GetState());
                //Debug.LogError("Error al realizar la tirada.");
            }else if(json.Contains("ok, gajó el winner: 2"))
            {
                yield return StartCoroutine(GetState());
                //Debug.LogError("Error al realizar la tirada.");
            }
            else
            {
                Debug.LogError("Error al realizar la tirada. Ni ganó nadie ni se colocó la ficha");
            }
        }
        //else
        //{
        //    string json = www.downloadHandler.text;
        //    Debug.Log("NetworkManager: Resultado de tirada " + www.downloadHandler.ToString());
        //    StartCoroutine(GetState());
        //}
    }

    public IEnumerator ResetGame()
    {

        UnityWebRequest www = UnityWebRequest.Get(baseURL+$"?action=1");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success/*www.isNetworkError*/)
        {
            Debug.LogError(www.error);

        }else
        {
            string json = www.downloadHandler.text;
            //Debug.Log("Juego reiniciado: " + json);
        }
        
    }
}
