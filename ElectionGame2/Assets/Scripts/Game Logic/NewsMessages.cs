using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a big list of messages that you can draw from. The game creates 8 in total. Each time it is queried, it will DELETE the message it gives you.
/// This is to ensure there are no duplicates.
/// </summary>
public class NewsMessages
{
    //The list of all messages
    private ArrayList messageList;
    //How many messages there are
    public const int MESSAGEMAX = 8;
    //The Demographic class
    private AustralianDemographics ad;

    private System.Random random = new System.Random();

    string[] reportStart =
    {
        "Analyists report that ",
        "Our latest polls show ",
        "It's being reported that ",
        "Breaking news: ",
        "A new commission suggests "
    };
    string[] reportMid =
    {
        "is a top concern for",
        "is the main priority for",
        "is most import to",
    };
    string[] randomReports =
    {
        "Australians are tired of political in-fighting.",
        "Opposition leaders are 47% more vulgar than they were 20 years ago.",
        "Yelling in the House of Representatives is deemed \'Necessary\'",
        "The nation is disillusioned with the dissolution",
    };

	/// <summary>
	/// Constructs the messages. This needs to be done after the demographic data has been created too.
	/// </summary>
    public NewsMessages(AustralianDemographics ad)
    {
        messageList = new ArrayList();
        foreach (VotingArea p in Enum.GetValues(typeof(VotingArea)))
        {
            string m = "" + reportStart [random.Next(reportStart.Length)];
            string ps = p.ToString();
            m += ps.Substring(0, 1) + ps.Substring(1).ToLower();
            m += " " + reportMid [random.Next(reportMid.Length)];
            m += ad.GetRoughVotePercentage(p) + "% of Australian voters";
            messageList.Add(m);
        }
        while (messageList.Count < MESSAGEMAX)
        {
            string m = "" + reportStart [random.Next(reportStart.Length)];
            m += randomReports [random.Next(randomReports.Length)];
            messageList.Add(m);
        }
    }

    /// <summary>
    /// Gets a report from the list. If all reports have been taken, you'll get a random report.
    /// </summary>
    /// <returns>The random message.</returns>
    public string GetRandomMessage(){
        if (messageList.Count == 0)
        {
            string m = "" + reportStart [random.Next(reportStart.Length)];
            m += randomReports [random.Next(randomReports.Length)];
            return m;
        } 
        else
        {
            int rand = random.Next(messageList.Count);
            string m = (String)messageList [rand];
            messageList.RemoveAt(rand);
            return m;
        }
    }
}


