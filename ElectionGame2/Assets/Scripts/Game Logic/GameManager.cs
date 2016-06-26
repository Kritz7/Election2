using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public CustomNetworkManager cnm;
    public static GameManager gameMan;

    public GameObject LoginScreen;
    public GameObject QuestionScreen;
    public GameObject VotingScreen;
    public GameObject ResultsScreen;

    public TimerDisplay mainTimer;

    public GameObject PlayerGrid;
    public GameObject PlayerTextPrefab;
    public GameObject RoomCodeText;
    public Text RoundNumber;

    public bool GameStarted = false;

    public Dictionary<string, Player> Players = new Dictionary<string, Player>();
    public int GameRound = 0;
    public int MaxGameRounds = 8;
    public string GameRoomName;

    enum VotingStage
    {
        Idle,
        Login,
        Question,
        Voting,
        Results
    }
    VotingStage CurrentStage = VotingStage.Idle;

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

        GameRoomName = GenID();
        RoomCodeText.GetComponent<Text>().text = "room code: " + GameRoomName;

        StartCoroutine(cnm.CreateGameRoom(GameRoomName));
        StartCoroutine(cnm.PollGameRoom(GameRoomName));

        CurrentStage = VotingStage.Login;
	}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DeinitSetup();
            StartCoroutine(GameRoundLogic());
        }
    }

    void DeinitSetup()
    {
        LoginScreen.gameObject.SetActive(false);
        cnm.Post(new string[] {"unity-gamestate", "unity-gameroom"}, new string[] {"endvote-all-"+GameManager.gameMan.GenID(), GameManager.gameMan.GameRoomName});

    }

    IEnumerator GameRoundLogic()
    {
        float questionTime = 10f;
        float votingTime = 20f;
        float resultsTime = 10f;

        DeinitSetup();

        FundingModel fm = new FundingModel();
        AustralianDemographics ad = new AustralianDemographics();
        NewsMessages nm = new NewsMessages(ad);

        while(GameRound < MaxGameRounds)
        {
            RoundNumber.gameObject.SetActive(true);
            GameRound++;
            RoundNumber.text = "ROUND " + GameRound;

            PolicyProposal p1 = new PolicyProposal();
            PolicyProposal p2 = new PolicyProposal();


            InitQuestion(GameRound);
            StartCoroutine(ChangeTimer(questionTime));
            yield return new WaitForSeconds(questionTime);

            InitVoting(GameRound);
            StartCoroutine(ChangeTimer(votingTime));
            yield return new WaitForSeconds(votingTime);

            fm.ImplementPolicy(p);

            InitResults(GameRound);
            StartCoroutine(ChangeTimer(resultsTime));
            yield return new WaitForSeconds(resultsTime);
        }

        yield break;
    }

    IEnumerator ChangeTimer(float time)
    {
        float t = 0;

        while(t<time)
        {
            t+=Time.deltaTime;

            mainTimer.SetPercentage(1-(t/time), (time-t));

            yield return 0;
        }
    }


    public void NewDataRegistered(string playerName, string newData)
    {
        if(CurrentStage == VotingStage.Voting)
        {
            // New vote
            NotificationManager.notMan.NewNotification(playerName + " voted " + newData);
        }    
    }

    public void DeinitQuestion()
    {
        QuestionScreen.gameObject.SetActive(false);
    }

    public void InitQuestion(int roundNumber)
    {
        DeinitResults();

        QuestionScreen.gameObject.SetActive(true);
        CurrentStage = VotingStage.Question;
    }

    public void DeinitVoting()
    {
        cnm.Post(new string[] {"unity-gamestate", "unity-gameroom"}, new string[] {"endvote-all-"+GameManager.gameMan.GenID(), GameManager.gameMan.GameRoomName});

        VotingScreen.gameObject.SetActive(false);
    }

    public void InitVoting(int roundNumber)
    {
        DeinitQuestion();

        cnm.Post(new string[] {"unity-gamestate", "unity-gameroom"}, new string[] {"voting-all-"+GameManager.gameMan.GenID(), GameManager.gameMan.GameRoomName});


        VotingScreen.gameObject.SetActive(true);
        CurrentStage = VotingStage.Voting;
    }

    public void DeinitResults()
    {
        ResultsScreen.gameObject.SetActive(false);
    }

    public void InitResults(int roundNumber)
    {
        DeinitVoting();

        ResultsScreen.gameObject.SetActive(true);
        CurrentStage = VotingStage.Results;
    }

    public void AddNewPlayerToScreen(string playerName)
    {
        GameObject newPlayerName = Instantiate(PlayerTextPrefab);
        newPlayerName.transform.SetParent(PlayerGrid.transform, false);
        newPlayerName.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(Random.Range(-Screen.width*0.2f, Screen.width*0.2f), Random.Range(-Screen.height*0.2f,Screen.height*0.2f));
        newPlayerName.GetComponentInChildren<Text>().text = playerName;
    }
        
    public void HandleGameData(string gameData)
    {
        Debug.Log("Game data: " + gameData);

    //    string GameID = gameData.Substring(0,6);
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
                    AddNewPlayerToScreen(playerName);
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
