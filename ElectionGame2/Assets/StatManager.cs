using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StatManager : MonoBehaviour
{
    public List<TimerDisplay> Bars = new List<TimerDisplay>();

    public void SetBar(int index, float percentage)
    {
        Bars[index].SetPercentage(percentage);
    }

    public void SetBar(int index, float percentage, string textnew)
    {
        Bars[index].SetPercentage(percentage, textnew);
    }
}
