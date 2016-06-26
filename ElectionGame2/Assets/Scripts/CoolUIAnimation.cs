using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CoolUIAnimation : MonoBehaviour
{
    public RectTransform myRect;
    public Outline outline;
    public Text text;
    public Image image;
    public CanvasGroup canvasGroup;

    protected Vector3 startPosition;
    protected Vector3 startScale;
    protected Color startTextColor;
    protected Color startOutlineColor;

    private float t = 0;

    private float popCurrentValue = 0f;
    private float popEndValue = 0f;
    private float popStartValue = 0f;
    private float forceValue = 0f;
    private bool toForceValue = false;

    public enum LerpStyle
    {
        EaseIn,
        EaseOut,
        Smooth,
        SuperSmooth,
        Linear
    }
    public enum InitAs
    {
        Idle,
        Hue
    }
    public InitAs initAs;

    public static List<CoolUIAnimation> CoolUIs = new List<CoolUIAnimation>();
    public List<CoolUIAnimation> CombinedWith = new List<CoolUIAnimation>();

    void Awake()
    {
        myRect = GetComponent<RectTransform>();

        if(!outline && GetComponent<Outline>()) outline = GetComponent<Outline>();
        if(!text && GetComponent<Text>()) text = GetComponent<Text>();
        if(!canvasGroup && GetComponent<CanvasGroup>()) canvasGroup = GetComponent<CanvasGroup>();
        if(!image && GetComponentInChildren<Image>()) image = GetComponentInChildren<Image>();

        SetAllDefaultValues();
        if(initAs == InitAs.Hue) InitHueShift();
    }

    void Update()
    {
        t += Time.deltaTime;
    }

    void OnEnable()
    {
        if(!gameObject.activeSelf)
        {
            Debug.LogWarning("Object is not active. Can't start coolanimation.");
            Destroy(this);
        }

        if(!CoolUIs.Contains(this))
        {
            CoolUIs.Add(this);
        }
    }

    void OnDisable()
    {
        if(CoolUIs.Contains(this))
        {
            CoolUIs.Remove(this);
        }
    }


    private void SetAllDefaultValues()
    {
        startPosition = myRect.anchoredPosition3D;
        startScale = myRect.localScale;
        if(text) startTextColor = text.color;
        if(outline) startOutlineColor = outline.effectColor;
        CombinedWith.Clear();
    }

    public void InitGlow(Color colour, float pulseLength = 0.5f, float pulseSize = 3f)
    {
        bool destroyOutlineOnCompletion = true;

        if(outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }
        else
        {
            destroyOutlineOnCompletion = false;
        }

        outline.effectDistance = new Vector2(pulseSize,pulseSize);

        StartCoroutine(Glow(colour, pulseLength, destroyOutlineOnCompletion));
    }

    public void StopGlow(bool destroyOutline = false)
    {
        StartCoroutine(SlowGlowDisable(destroyOutline));
    }

    IEnumerator SlowGlowDisable(bool destroyOutline)
    {
        if(outline==null)
            yield break;

        outline.enabled = false;
        yield return new WaitForSeconds(0.1f);
        outline.enabled = true;

        if(destroyOutline)
            Destroy(outline);

        Debug.Log("glow should be disabled now?");
    }

    public void InitWobble()
    {

    }

    public void InitHueShift()
    {
        StartCoroutine(HueShift());
    }

    bool ShouldMerge()
    {
        // merge on-screen pops if touching
        if(name.Contains("Building-Pop-Text-Effect"))
        {
            CoolUIAnimation overlappingWith = null;
            foreach(CoolUIAnimation coolUI in CoolUIs)
            {
                if(coolUI != this && coolUI.name.Contains("Building-Pop-Text-Effect") && !CombinedWith.Contains(coolUI) && !coolUI.CombinedWith.Contains(this))
                {
                    if(coolUI.canvasGroup.alpha == 1 && canvasGroup.alpha == 1)
                    {
                        if(new Rect(coolUI.myRect.anchoredPosition, new Vector2(coolUI.myRect.sizeDelta.x * 0.85f, coolUI.myRect.sizeDelta.y * 0.5f))
                            .Overlaps(
                                new Rect(myRect.anchoredPosition, new Vector2(coolUI.myRect.sizeDelta.x * 0.85f, coolUI.myRect.sizeDelta.y * 0.5f))))
                        {
                            overlappingWith = coolUI;
                        }
                    }
                }
            }

            if(overlappingWith!=null)
            {
                CombinedWith.Add(overlappingWith);
                return true;
            }
        }

        return false;
    }
        
    public void ModifyWorldPopValue(float addOnVal)
    {
        popEndValue += addOnVal;
        popStartValue = popCurrentValue;

        InitScale(1.35f, 1f, LerpStyle.EaseIn, 0.3f);
    }

    public void InitScale(float startScaleMag, float endScaleMag, LerpStyle style, float duration)
    {
        Vector3 endScale = startScale * endScaleMag;
        StartCoroutine(Scale(startScale * startScaleMag, endScale, style, duration));
    }

    public void InitScale(float endScaleMag, LerpStyle style, float duration)
    {
        Vector3 endScale = startScale * endScaleMag;
        StartCoroutine(Scale(startScale, endScale, style, duration));
    }

    public void InitScale(Vector3 endScale, LerpStyle style, float duration)
    {
        startScale = myRect.localScale;
        StartCoroutine(Scale(startScale, endScale, style, duration));
    }

    public void InitScale(Vector3 startScale, Vector3 endScale, LerpStyle style, float duration)
    {
        StartCoroutine(Scale(startScale, endScale, style, duration));
    }

    public void InitShake(Vector3 direction, float intensity, float duration)
    {
        StartCoroutine(Shake(direction, intensity, duration));
    }


    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if(!canvasGroup)
        {
            Debug.LogWarning("Need an active canvasgroup on this object in order to fade. Adding one now.", gameObject);
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        float startT = t;
        float myT = 0;
        float completion = 0;
        LerpStyle myLerpStyle = LerpStyle.SuperSmooth;

        while(myT < duration)
        {
            myT = t - startT;
            completion = GetCompletion(myT, duration, myLerpStyle);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, completion);

            yield return 0;
        }
    }

    private IEnumerator Glow(Color colour, float duration, bool destroyOutline)
    {
        float startT = t;
        float myT = 0;
        bool glowOn = false;
        float completion = 0;

        Color startColour = outline.effectColor;
        Color colourAlpha = new Color(colour.r, colour.g, colour.b, 0f);

        while(outline.enabled)
        {
            glowOn = !glowOn;
            startT = t;
            myT = 0;
            completion = 0;

            while(myT < duration)
            {
                myT = t - startT;
                completion = GetCompletion(myT, duration, LerpStyle.Linear);
                outline.effectColor = Color.Lerp((glowOn?colour:colourAlpha), (glowOn?colourAlpha:colour), completion);

                yield return 0;
            }
        }

        Debug.Log("Glow stopped");

        outline.effectColor = startColour;

        yield break;
    }

    private IEnumerator Shake(Vector3 direction, float intensity, float duration)
    {
        float startT = t;
        float myT = 0;
        Vector3 myTV = Vector3.zero;

        Vector3 startPos = transform.position;

        while(myT < duration)
        {
            myT = t - startT;
            myTV += new Vector3(t * direction.x, t * direction.y, t * direction.z);

            Vector3 newPos = new Vector3(
                Mathf.Sin(myTV.x) * direction.x * intensity,
                Mathf.Sin(myTV.y) * direction.y * intensity,
                Mathf.Sin(myTV.z) * direction.z * intensity);

            transform.position = new Vector3(
                startPos.x + newPos.x,
                startPos.y + newPos.y,
                startPos.z + newPos.z);

            yield return 0;
        }

        transform.position = startPos;
        Destroy(this);
    }

    private IEnumerator Wobble()
    {
        yield break;
    }

    private IEnumerator HueShift()
    {
        float r = 255;
        float g,b;
        g = b = 0;
        float speed = 1;
        float dir = 1;
        int at = 2;

        while(true)
        {
            switch(at)
            {
                case 0:
                    if(r <= 0 || r >= 255) dir = r <= 0 ? 1 : -1;
                    r += dir * speed;
                    if(r <= 0 || r >= 255) at = (at + 1) % 3;
                    break;
                case 1:
                    if(g <= 0 || g >= 255) dir = g <= 0 ? 1 : -1;
                    g += dir * speed;
                    if(g <= 0 || g >= 255) at = (at + 1) % 3;
                    break;
                case 2:
                    if(b <= 0 || b >= 255) dir = b <= 0 ? 1 : -1;
                    b += dir * speed;
                    if(b <= 0 || b >= 255) at = (at + 1) % 3;
                    break;
            }

            if(text!=null)
                text.color = new Color(r / 255f, g / 255f, b / 255f);

            if(image!=null)
                image.color = new Color(r / 255f, g / 255f, b / 255f);

            yield return 0;
        }
    }

    private IEnumerator Scale(Vector3 startScale, Vector3 endScale, LerpStyle style, float duration)
    {
        float startT = t;
        float myT = 0;
        float completion = 0;

        while(myT < duration)
        {
            myT = t - startT;
            completion = GetCompletion(myT, duration, style);

            myRect.localScale = Vector3.Lerp(startScale, endScale, completion);

            yield return 0;
        }

        myRect.localScale = endScale;
    }

    private float GetCompletion(float t, float duration, LerpStyle style)
    {
        float completion = t / duration;

        switch(style)
        {
            case LerpStyle.EaseIn:
                return Mathf.Sin(completion * Mathf.PI * 0.5f);
            case LerpStyle.EaseOut:
                return 1f - Mathf.Cos(completion * Mathf.PI * 0.5f);
            case LerpStyle.Smooth:
                return completion * completion * (3f - 2f * completion);
            case LerpStyle.SuperSmooth:
                return completion * completion * completion * (completion * (6f * completion - 15f) + 10f);
            default:
                return completion;
        }
    }
}