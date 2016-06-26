using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Policy Proposals are the binary choices people are faced with at each area of the game. These are procedurally generated, and
/// CURRENTLY are purely random. This may be a disaster, but meh. Should be fine.
/// </summary>
public class PolicyProposal
{
    //The value that'll be increased by this policy
    public PolicyArea increasePolicy;
    //The value that'll be decreased by this policy
    public PolicyArea decreasePolicy;
    //How much this policy will cost or save
    public int budgetDifference;
    //What kind of policy this one is
    public PolicyType type;

    //How the policy will be presented. Use this when displaying the policy.
    public string policyString = "";


    string[] p1starts =
    {
        "We must prioritize ", "It's time to focus on ", "We should favour ", "We must shift focus to ", "Our future lies with ", "Australia demands prioritizing "
    };
    string[] p2starts =
    {
        "We should invest more in ", "We should increase spending in ", "The government must spend more on ", "It's obvious we need more money in ", "Australia demands more "
    };
    string[] p3starts =
        {
        "We could save money by cutting on ", "We should cut back on ", "We don't need as much funding in ", "Lets reduce funding in ", "Australia expects less "
    };

    string[] diffStrings = { " over ", " rather than ", " instead of " };

    string[] dwords =
    {
        "Securing Our Borders",
        "Stopping The Boats",
        "The War on Terror",
        "The Defense Force"
    };
    string[] iwords =
        {
        "Our Small Businesses",
        "A Fair Go for Working Families",
        "Moving Forward with Trade",
        "Supporting our Big Banks"
    };
    string[] pwords =
    {
        "The Gonsky Education Program",
        "Laptops in Every School",
        "Our Arts Programs",
        "Nation Building Programs"
    };
    string[] ewords =
        {
        "Saving our Rainforests",
        "Renewable Energy Technology",
        "Pushing the Carbon Tax",
    };

    //Obiviously.
    private static System.Random random = new System.Random();

    /// <summary>
    /// Constructs a PURELY RANDOM policy proposal. Enjoy all those hardcoded values!
    /// </summary>
    public PolicyProposal()
    {
        Array areas = Enum.GetValues(typeof(PolicyArea));
        int proposalType = random.Next(0, 9);
        if (proposalType < 2)
        {
            int index = random.Next(areas.Length);
            increasePolicy = (PolicyArea)areas.GetValue(index);
            int index2 = random.Next(areas.Length);
            while (index2 == index)
                index2 = random.Next(areas.Length);
            decreasePolicy = (PolicyArea)areas.GetValue(index2);
            budgetDifference = 0;
            type = PolicyType.NOBUDGET;

        } 
        else if (proposalType < 8)
        {
            int index = random.Next(areas.Length);
            increasePolicy = (PolicyArea)areas.GetValue(index);
            budgetDifference = -random.Next(2800, 5200);
            type = PolicyType.NODECREASE;
        } 
        else
        {
            int index = random.Next(areas.Length);
            decreasePolicy = (PolicyArea)areas.GetValue(index);
            budgetDifference = random.Next(1400, 2600);
            type = PolicyType.NOINCREASE;
        }

        GeneratePolicyTopic();
    }

    public void GeneratePolicyTopic() {
        if (type == PolicyType.NOINCREASE)
            policyString = p3starts [random.Next(0, p3starts.Length)];
        else if (type == PolicyType.NODECREASE)
            policyString = p2starts [random.Next(0, p2starts.Length)];
        else
            policyString = p1starts [random.Next(0, p1starts.Length)];
            
        PolicyArea area = (type ==  PolicyType.NOINCREASE ? decreasePolicy : increasePolicy);
        
        switch (area)
        {
            case PolicyArea.DEFENSE:
                policyString += dwords [random.Next(0, dwords.Length)];
                break;
            case PolicyArea.ENVIRONMENT:
                policyString += ewords [random.Next(0, ewords.Length)];
                break;
            case PolicyArea.INDUSTRY:
                policyString += iwords [random.Next(0, iwords.Length)];
                break;
            case PolicyArea.PUBLIC:
                policyString += pwords [random.Next(0, pwords.Length)];
                break;
        }

        if (type == PolicyType.NOBUDGET)
        {
            policyString += diffStrings [random.Next(0, diffStrings.Length)];
            string pstring = decreasePolicy.ToString();
            policyString += pstring.Substring(0, 1) + pstring.Substring(1).ToLower();
            policyString += " spending.";
        }
    }
}