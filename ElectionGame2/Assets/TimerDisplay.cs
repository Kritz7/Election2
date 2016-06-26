using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerDisplay : MonoBehaviour
{
    public Image BarImage;
    public RectTransform myRect;
    public Text myText;

    public void SetPercentage(float percentage, float timeleft)
    {
        BarImage.rectTransform.sizeDelta = new Vector2(myRect.rect.width * percentage, BarImage.rectTransform.sizeDelta.y);
        myText.text = Mathf.Round(timeleft)+"s remaining";
    }

    public void SetPercentage(float percentage, string textnew)
    {
        BarImage.rectTransform.sizeDelta = new Vector2(myRect.rect.width * percentage, BarImage.rectTransform.sizeDelta.y);
        myText.text = textnew;
    }
}
