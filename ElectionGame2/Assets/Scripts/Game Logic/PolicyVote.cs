using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolicyVote {

    private Dictionary<string,int> votes;
    public PolicyProposal Policy_One;
    public PolicyProposal Policy_Two;

    public PolicyVote(PolicyProposal p1, PolicyProposal p2){
        Policy_One = p1;
        Policy_Two = p2;

        votes = new Dictionary<string, int>();
    }

    /// <summary>
    /// Casts a vote. This can be done repreatedly by one player; extra votes are ignored.
    /// </summary>
    /// <param name="player">The UNIQUE ID of the player casting votes</param>
    /// <param name="choice">What choice they've made: 1 OR 2</param>
    public void CastVote(string player, int choice){
        if (choice == 1 || choice == 2) {
            if (votes.ContainsKey(player))
                return;
            else
                votes [player] = choice;
        }
    }

    /// <summary>
    /// Calculates how many votes for each choice (1 and 2).
    /// </summary>
    /// <returns>The tally, as a 2-length array. Index 0 is choice 1, Index 1 is choice 2.</returns>
    public int[] getTally(){
        int[] tally = new int[2];
        tally [0] = 0;
        tally [1] = 0;

        foreach (KeyValuePair<string, int> entry in votes)
        {
            tally [entry.Value - 1]++;
        }
        return tally;
    }
}
