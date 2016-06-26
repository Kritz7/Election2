using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public CustomNetworkManager cnm;
    public static GameManager gameMan;

    public Dictionary<string, Player> Players = new Dictionary<string, Player>();
    public string GameRoomName;

    public string GenID(int roomLength=4)
    {
        char[] alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U' };
        string name = "";

        for(int i=0; i<roomLength; i++)
        {
            name += ""+alphabet[Random.Range(0,alphabet.Length)];
        }

        return name;
    }

	// Use this for initialization
	void Start ()
    {
        gameMan = this;
        FundingModel fm = new FundingModel();
        AustralianDemographics ad = new AustralianDemographics();
        Debug.Log(ad.ToString());
        NewsMessages nm = new NewsMessages(ad);
        Debug.Log(fm.ToString());
        for (int i = 0; i < 8; i++)
        {
            PolicyProposal p = new PolicyProposal();
            Debug.Log(p.policyString);
            fm.ImplementPolicy(p);
            Debug.Log(fm.ToString());
            Debug.Log(nm.GetRandomMessage());
        }
        long votes = ad.TallyVotes(fm);
        Debug.Log("Final score: " + ad.GetVotePercentage(votes));

        //GameRoomID = GenerateGameRoomName();
        GameRoomName = "BUTT";
        StartCoroutine(cnm.CreateGameRoom(GameRoomName));
        StartCoroutine(cnm.PollGameRoom(GameRoomName));
	}

    public void NewDataRegistered(string playerName, string newData)
    {
        NotificationManager.notMan.NewNotification(playerName + " did a " + newData + " x" + Players[playerName].Input.Count);
    }
	
    public void HandleGameData(string gameData)
    {
        Debug.Log("Game data: " + gameData);

        string GameID = gameData.Substring(0,6);
        string playerData = gameData.Substring(6);
        SeperatePlayerData(playerData);
    }

    public void SeperatePlayerData(string gameData)
    {
        List<string> playerdata = new List<string>(gameData.Split(':'));

        foreach(string s in playerdata)
        {
            if(s.Contains("{") && s.Contains("}"))
            {
                string playerName = s.Substring(0, s.IndexOf("{"));
                List<string> playerInput = new List<string>(s.Substring(s.IndexOf("{")+1).Split(','));
                List<string> parsedPlayerInput = new List<string>();

                if(!Players.ContainsKey(playerName))
                {
                    Players.Add(playerName, new Player(playerName));
                }

                foreach(string inputthingy in playerInput)
                {
                    if(!inputthingy.Contains("}"))
                    {
                        parsedPlayerInput.Add(inputthingy);
                    }
                }

                Players[playerName].Add(parsedPlayerInput);
            }
        }
    }
}
