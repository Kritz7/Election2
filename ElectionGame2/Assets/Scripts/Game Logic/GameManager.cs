using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

using Random = UnityEngine.Random;

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

    public StatManager DefenseStats;
    public StatManager ChoiceStats;
    public Text BudgetText;
    public Text NewsTicker;
    public Text EndGameText;

    public PolicyVote currentVote;

    public bool GameStarted = false;

    public Dictionary<string, Player> Players = new Dictionary<string, Player>();
    public int GameRound = 0;
    public int MaxGameRounds = 1;
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

        QuestionScreen.gameObject.SetActive(false);
        VotingScreen.gameObject.SetActive(false);
        ResultsScreen.gameObject.SetActive(false);
        BudgetText.gameObject.SetActive(false);
        NewsTicker.gameObject.SetActive(false);
        ChoiceStats.gameObject.SetActive(false);
        mainTimer.gameObject.SetActive(false);
        EndGameText.gameObject.SetActive(false);
        DefenseStats.gameObject.SetActive(false);

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
        float questionTime = 5f;
        float votingTime = 25f;
        float resultsTime = 15f;

        DeinitSetup();

        FundingModel fm = new FundingModel();
        AustralianDemographics ad = new AustralianDemographics();
        NewsMessages nm = new NewsMessages(ad);

        mainTimer.gameObject.SetActive(true);

        DefenseStats.SetBar((int)PolicyArea.DEFENSE, 0.5f);
        DefenseStats.SetBar((int)PolicyArea.INDUSTRY, 0.5f);
        DefenseStats.SetBar((int)PolicyArea.PUBLIC, 0.5f);
        DefenseStats.SetBar((int)PolicyArea.ENVIRONMENT, 0.5f);

        BudgetText.text = "BUDGET: $"+string.Format("{0:n0}", fm.BUDGET)+",000";

        while(GameRound < MaxGameRounds)
        {
            /////////////////////////// QUESTION ///////////////////////////

            RoundNumber.gameObject.SetActive(true);
            GameRound++;
            RoundNumber.text = "ROUND " + GameRound;

            PolicyProposal p1 = new PolicyProposal();
            PolicyProposal p2 = new PolicyProposal();
            currentVote = new PolicyVote(p1, p2);

            ChoiceStats.SetBar(0, 0);
            ChoiceStats.SetBar(1, 0);

            InitQuestion(GameRound);
            StartCoroutine(ChangeTimer(questionTime));

            QuestionScreen.transform.GetChild(2).GetComponent<Text>().text
            = p1.policyString + "\n" + p2.policyString;

            NewsTicker.gameObject.SetActive(false);

            yield return new WaitForSeconds(questionTime);

            /////////////////////////// VOTING ///////////////////////////

            ChoiceStats.gameObject.SetActive(true);
            DefenseStats.gameObject.SetActive(true);
            BudgetText.gameObject.SetActive(true);
            yield return 0;

            DefenseStats.SetBar((int)PolicyArea.DEFENSE, 
                ((float)fm.GetFundingAmount(PolicyArea.DEFENSE))/((float)7));
            DefenseStats.SetBar((int)PolicyArea.INDUSTRY, 
                ((float)fm.GetFundingAmount(PolicyArea.INDUSTRY))/((float)7));
            DefenseStats.SetBar((int)PolicyArea.PUBLIC, 
                ((float)fm.GetFundingAmount(PolicyArea.PUBLIC))/((float)7));
            DefenseStats.SetBar((int)PolicyArea.ENVIRONMENT, 
                ((float)fm.GetFundingAmount(PolicyArea.ENVIRONMENT))/((float)7));

    

            VotingScreen.transform.GetChild(2).GetComponent<Text>().text
            = p1.policyString + "\n" + p2.policyString;

            ChoiceStats.Bars[0].myText.text = p1.ToString();
            ChoiceStats.Bars[1].myText.text = p2.ToString();

            InitVoting(GameRound);
            StartCoroutine(ChangeTimer(votingTime));
            yield return new WaitForSeconds(votingTime);

            /////////////////////////// RESUTS ///////////////////////////
            int[] tally = currentVote.getTally();
            //To implement policy:
            fm.ImplementPolicy(tally[0] > tally[1] ? p1 : p2);

            ChoiceStats.gameObject.SetActive(false);
            NewsTicker.gameObject.SetActive(true);

            NewsTicker.text = nm.GetRandomMessage();

            DefenseStats.SetBar((int)PolicyArea.DEFENSE, 
                ((float)fm.GetFundingAmount(PolicyArea.DEFENSE))/((float)7));
            DefenseStats.SetBar((int)PolicyArea.INDUSTRY, 
                ((float)fm.GetFundingAmount(PolicyArea.INDUSTRY))/((float)7));
            DefenseStats.SetBar((int)PolicyArea.PUBLIC, 
                ((float)fm.GetFundingAmount(PolicyArea.PUBLIC))/((float)7));
            DefenseStats.SetBar((int)PolicyArea.ENVIRONMENT, 
                ((float)fm.GetFundingAmount(PolicyArea.ENVIRONMENT))/((float)7));

            BudgetText.text = "BUDGET: $"+string.Format("{0:n0}", fm.BUDGET)+",000";

            InitResults(GameRound);
            StartCoroutine(ChangeTimer(resultsTime));
            yield return new WaitForSeconds(resultsTime);
        }

        QuestionScreen.gameObject.SetActive(false);
        VotingScreen.gameObject.SetActive(false);
        ResultsScreen.gameObject.SetActive(false);
        ChoiceStats.gameObject.SetActive(false);
        mainTimer.gameObject.SetActive(false);
        NewsTicker.gameObject.SetActive(false);


        ArrayList lowest = new ArrayList(), highest = new ArrayList();
        int lowesti = 8, highesti = -1;
        foreach (PolicyArea p in Enum.GetValues(typeof(PolicyArea)))
        {
            int funding = fm.GetFundingAmount(p);
            if (funding < lowesti)
            {
                lowest.Clear();
                lowest.Add(p);
                lowesti = funding;
            } else if (funding == lowesti)
            {
                lowest.Add(p);
            }

            if (funding > highesti)
            {
                highest.Clear();
                highest.Add(p);
                highesti = funding;
            } else if (funding == highesti)
            {
                highest.Add(p);
            }
        } 

        string hstring = "Our HIGHEST priorities are: \n";
        foreach (PolicyArea p in highest)
        {
            hstring += p.ToString() + " ";
        }
        string lstring = "Our LOWEST priorities are: \n";
        foreach (PolicyArea p in lowest)
        {
            lstring += p.ToString() + " ";
        }



        EndGameText.gameObject.SetActive(true);

        EndGameText.text = lstring + "\n" + hstring;

        long finalTally = ad.TallyVotes(fm);
        double percentage = ad.GetVotePercentage(finalTally);


        EndGameText.text += "\n"+Mathf.Round(((float)percentage)*100f)+
            "% of Australians voted for your party! You were "+(percentage>0.5?"":"not")+" successfully elected!";

        //The final vote tally!



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
            if (currentVote != null) {

                Debug.Log("new vote!");

                int playerVote = int.Parse(newData);
                int notPlayerVote = (playerVote==1?2:1);

                currentVote.CastVote(playerName, playerVote);

                ChoiceStats.SetBar(playerVote-1, GetVotePercentage(playerVote-1));
                ChoiceStats.SetBar(notPlayerVote-1, GetVotePercentage(notPlayerVote-1));
            }

            NotificationManager.notMan.NewNotification(playerName + " voted " + newData);
        }    
    }

    public float GetVotePercentage(int index)
    {
        int[] voteTally = currentVote.getTally();
        int totalVotes = voteTally[0] + voteTally[1];

        Debug.Log(voteTally[index] + " " + totalVotes + " votes");

        if(voteTally[0]==0)
        {
            return (index==0?0:1);
        }
        if(voteTally[1]==0)
        {
            return (index==0?1:0);
        }

        float percentage = ((float)voteTally[index])/((float)totalVotes);
        return percentage;
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
