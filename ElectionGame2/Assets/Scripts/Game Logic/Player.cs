using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
    public string PlayerName = "";
    public List<string> Input = new List<string>();

    public Player(string playername)
    {
        PlayerName = playername;
        Input = new List<string>();
    }

    public void Clear()
    {
        Input.Clear();
    }

    public void Add(List<string> data)
    {
        if(data.Count != Input.Count && data.Count > Input.Count)
        {
            // NEW DATA!
            for(int i=Input.Count; i<data.Count; i++)
            {
                Input.Add(data[i]);
                GameManager.gameMan.NewDataRegistered(PlayerName, data[i]);
            }
        }
    }

    public void AddDataToPlayer(List<string> newData)
    {
        Input = newData;
    }
}
