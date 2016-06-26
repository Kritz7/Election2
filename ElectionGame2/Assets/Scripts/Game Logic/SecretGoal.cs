using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// This goal is basically 'do this to REALLY win the game'. It's a hidden additional requirement to win the game properly.
/// These are constructed and given to each player, as a string.
/// </summary>
public class SecretGoal
{
    /// <summary>
    /// Whether it's a high or low priority for the player
    /// </summary>
    enum InterestType{
        MAXIMUM, MINIMUM
    }

    //What area the player cares about
    VotingArea interestArea;
    //What they want that area to be like
    InterestType interestType;

    //The string displayed on their device when voting
    public string goalString = "";

    private static System.Random random = new System.Random();

    /// <summary>
    /// Constructs a new random secret goal
    /// </summary>
    public SecretGoal()
    {
        Array interests = Enum.GetValues(typeof(VotingArea));
        Array interestAmounts = Enum.GetValues(typeof(InterestType));

        interestArea = (VotingArea)interests.GetValue(random.Next(interests.Length));
        interestType = (InterestType)interestAmounts.GetValue(random.Next(interestAmounts.Length));

        goalString = GenerateGoalString();
        Debug.Log(goalString);
    }

    /// <summary>
    /// Generates the goal string.
    /// </summary>
    /// <returns>The goal string.</returns>
    private string GenerateGoalString(){
        string s = "Ensure that ";

        if (interestArea != VotingArea.ECONOMY)
        {
            string interest = interestArea.ToString();
            interest = interest.Substring(0, 1) + interest.Substring(1).ToLower();
            s += interest + " spending is our " + (interestType == InterestType.MAXIMUM ? "HIGHEST" : "LOWEST") + " priority";
        } else
        {
            s += " our budget is running a " + (interestType == InterestType.MAXIMUM ? "SURPLUS" : "DEFICIT");
        }
        return s;
    }
}


