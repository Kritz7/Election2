using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Networking;

public class CustomNetworkManager : MonoBehaviour
{
    public GameManager GameManager;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.W))
        {
            Post(new string[] {"unity-gamestate", "unity-gameroom"}, new string[] {"voting-all-"+GameManager.gameMan.GenID(), GameManager.gameMan.name});
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
       //     PostStuff("unitycommands", "startvote");
        }
    }

    public void Post(string[] postNames, string[] postVars)
    {
        StartCoroutine(PostStuff(postNames, postVars));
    }

    IEnumerator PostStuff(string[] postNames, string[] postVars)
    {
        WWWForm form = new WWWForm();
        for(int i=0; i<postNames.Length; i++)
        {
            form.AddField(postNames[i], postVars[i]);
        }

        Debug.Log("Howdy!");

        UnityWebRequest www = UnityWebRequest.Post("http://kritz.net/election/", form);
        yield return www.Send();

        if(www.isError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("Post complete!");
        }
    }

    public IEnumerator CreateGameRoom(string gameName)
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

    public IEnumerator PollGameRoom(string gameName)
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
                GameManager.HandleGameData(www.downloadHandler.text);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
