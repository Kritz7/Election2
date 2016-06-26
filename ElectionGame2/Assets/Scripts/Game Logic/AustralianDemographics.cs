using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Construct this class precisely once. This contains a collection of distributions (out of 100) of what each
/// voter is concerned about.
/// Poll this to find out distributions, both exact and rough, as well as the 'policy concern' for each group, created
/// randomly.
/// </summary>
public class AustralianDemographics
{
    public long POPULATION = (long)(23.13 * Math.Pow(10,6));

    private Dictionary<VotingArea, int> demos;
    private static System.Random random = new System.Random();

    /// <summary>
    /// Creates a new demographic, and assigns new policy areas
    /// </summary>
    public AustralianDemographics()
    {
        demos = new Dictionary<VotingArea, int>();

        int remaining = 100;

        int absmax = 40;
        int absmin = 10;
        int cmax = absmax;
        int cmin = absmin;

        Array policies = Enum.GetValues(typeof(VotingArea));
        for (int i = 0; i < policies.Length; i++)
        {
            cmax = Math.Min(cmax, remaining - (4 - i) * absmin);
            cmin = Math.Max(cmin, remaining - (4 - i) * absmax);

            int newval = random.Next(cmin, cmax);
            remaining -= newval;
            demos[(VotingArea)policies.GetValue(i)] = newval;
        }
    }

    /// <summary>
    /// Tells you exactly how many votes there are in a given demographic
    /// </summary>
    /// <param name="p">The area of policy you're interested in.</param>
    private long GetVoterCount(VotingArea p){
        return (long)(((double)(demos[p]) / 100) * POPULATION);
    }

    /// <summary>
    /// Returns the number of voters in a particular demographic, rounded to within 5%
    /// </summary>
    /// <returns>The rough voter percentage.</returns>
    /// <param name="p">The area of policy you're interseted in</param>
    public int GetRoughVotePercentage(VotingArea p){
        int value = demos [p];
        if ((float)value % 5.0 < 2.5)
            return value + 5 - (value % 5);
        else
            return value - (value % 5);
    }

    /// <summary>
    /// Calculates how much each demographic voted, and adds that together into an overall percentage.
    /// Returns that percentage.
    /// </summary>
    /// <returns>The votes.</returns>
    /// <param name="model">The funding model of the game</param>
    public long TallyVotes(FundingModel model){
        Array policies = Enum.GetValues(typeof(PolicyArea));
        long voteTally = 0;
        foreach (PolicyArea p in policies) {
            voteTally += (long)(GetVoterCount(ToVoting(p)) * GetSupportFromPolicy(ToVoting(p), model.GetFundingAmount(p)));
        }
        float bsupport = ((float)(model.BUDGET) + 15000.0f) / 20000.0f;
        Mathf.Clamp(bsupport + ((float)(random.NextDouble() * 0.1 - 0.05)), 0.02f, 0.98f);
        voteTally += (long)(GetVoterCount(VotingArea.ECONOMY) * bsupport);
        return voteTally;
    }

    /// <summary>
    /// Gives a percentage of how much a specific group will vote for the party,
    /// based on the funding level. The higher the funding level, the better.
    /// </summary>
    /// <returns>The vote from policy.</returns>
    /// <param name="p">The policy area of interest</param>
    /// <param name="fundingLevel">The funding given to that policy (from FundingModel object)</param>
    private float GetSupportFromPolicy(VotingArea p, int fundingLevel){
        float support = 0.15f;
        fundingLevel = Math.Max(fundingLevel - 3, 0);
        support += fundingLevel * 0.2f;
        Mathf.Clamp(support + (float)(random.NextDouble() * 0.1 - 0.05), 0.02f, 0.98f);
        return support;
    }

    /// <summary>
    /// My complete mess. It's a bad fix but it'll do
    /// </summary>
    /// <param name="p">The policy you need to convert</param>
    private VotingArea ToVoting(PolicyArea p){
        return (VotingArea)Enum.Parse(typeof(VotingArea), p.ToString());
    }
}