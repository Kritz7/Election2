using System;
using System.Collections.Generic;
using UnityEngine;

public class FundingModel
{
    //Nothing should have more money than this. Top of the bar graph
    const int MAXFUNDING = 7;
    const int BASEFUNDING = 3;

    //How much money's in the bank! You can absolutely have negative budget
    private int Budget = 0;
    public int BUDGET { get { return Budget; } }

    //The funding model. How much is in each area. this is measured between 0 and 7
    private Dictionary<PolicyArea, int> funding;

    /// <summary>
    /// The finance model of the game, this is basically what the game revolves around. Query this to know how much
    /// funding is in each portfolio, and how much money is left in the budget.
    /// </summary>
    public FundingModel()
    {
        Budget = 20000;
        funding = new Dictionary<PolicyArea, int>();
        Array areas = Enum.GetValues(typeof(PolicyArea));
        foreach (PolicyArea area in areas)
        {
            funding [area] = 3;
        }
    }

    /// <summary>
    /// Commits the changes of a given policy to the funding model of the game
    /// </summary>
    /// <param name="p">The policy to implement</param>
    public void ImplementPolicy(PolicyProposal p){
        Budget += p.budgetDifference;
        if (p.type != PolicyType.NOINCREASE)
            funding [p.increasePolicy] = Math.Min(funding [p.increasePolicy] + 1, MAXFUNDING);
        if (p.type != PolicyType.NODECREASE)
            funding [p.decreasePolicy] = Math.Max(funding [p.decreasePolicy] - 1, 0);
    }

    /// <summary>
    /// Returns the amount of funding invested so far in a given policy area
    /// </summary>
    /// <returns>The funding amount.</returns>
    /// <param name="p">The policy area</param>
    public int GetFundingAmount(PolicyArea p){
        return funding [p];
    }

    /// <summary>
    /// String it!
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="FundingModel"/>.</returns>
    public override string ToString(){
        string s = "";

        foreach (PolicyArea p in Enum.GetValues(typeof(PolicyArea))) {
            s += p.ToString() + " " + funding[p] + "\n";
        }
        s += "BUDGET " + BUDGET;
        return s;
    }
}