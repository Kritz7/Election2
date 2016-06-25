using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour {

    public static NotificationManager notMan;
    public GameObject NotificationPrefab;
    public List<GameObject> Notifications = new List<GameObject>();

    void Awake()
    {
        notMan = this;
    }

    public void NewNotification(string text)
    {
        GameObject newObj = Instantiate(NotificationPrefab) as GameObject;
        newObj.transform.SetParent(this.transform, false);
        newObj.GetComponent<Animator>().SetTrigger("AnimateIN");

        newObj.GetComponentInChildren<Text>().text = text;

        StartCoroutine(DestroyNotification(newObj, 2f, 0.5f));
        Notifications.Add(newObj);
    }

    public IEnumerator DestroyNotification(GameObject obj, float delay, float animationLength)
    {
        yield return new WaitForSeconds(delay);

        obj.GetComponent<Animator>().SetTrigger("AnimateOUT");

        yield return new WaitForSeconds(animationLength);

        Notifications.Remove(obj);
        GameObject.Destroy(obj);
    }
}
