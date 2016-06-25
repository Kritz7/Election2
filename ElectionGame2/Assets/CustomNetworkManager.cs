using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Networking;

public class CustomNetworkManager : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(PollGameRoom("BUTT"));
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(CreateGameRoom("BUTT"));
        }
    }

    IEnumerator CreateGameRoom(string gameName)
    {
        WWWForm form = new WWWForm();
        form.AddField("unity-create", gameName);

        UnityWebRequest www = UnityWebRequest.Post("http://kritz.net/election/", form);
        yield return www.Send();

        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Form upload complete!");
        }
    }

    IEnumerator PollGameRoom(string gameName)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://kritz.net/election/rooms/GAMEDATA_"+gameName);

        while(true)
        {
            www = UnityWebRequest.Get("http://kritz.net/election/rooms/GAMEDATA_"+gameName);
            yield return www.Send();

            if(www.isError)
            {
                Debug.Log(www.error);

            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
