using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI stateDisplay;
    public GameObject stateDisplayObj;
    public GameObject bannerDisplayObj;

    private float t = 0;

    private void Start()
    {
        stateDisplayObj.SetActive(false);
    }
    private void Update()
    {
        t += Time.deltaTime / 1f;
        DisplayScore();
    }
    public void DisplayScore()
    {
        if (GameManager.Instance.players.Count < 2)
        {
            return;
        }
        scoreDisplay.text = GameManager.Instance.players[0].GetComponent<Player>().GetScore() + "-" +
            GameManager.Instance.players[1].GetComponent<Player>().GetScore();
    }
    public void DisplayState()
    {
        stateDisplayObj.SetActive(true);
    }
    public void HideState()
    {
        stateDisplayObj.SetActive(false);
    }

    public IEnumerator DisplayBanner(string bannerText)
    {
        t = 0;
        bannerDisplayObj.transform.Find("Banner Display").gameObject.GetComponent<TextMeshProUGUI>().text = bannerText;
        bannerDisplayObj.SetActive(true);
        while(t < 1)
        {
            bannerDisplayObj.transform.Find("Banner Display").transform.localScale = Vector3.Lerp(new Vector3(2f, 2f, 2f), new Vector3(1, 1, 1), t/.2f);
            bannerDisplayObj.transform.Find("Banner White").transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(1.5f, 1.5f, 1.5f), t);
            bannerDisplayObj.transform.Find("Banner White").GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0, t/.8f));

            foreach(Transform child in bannerDisplayObj.transform.Find("idk").transform)
            {
                //child.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Lerp(50, 0, t), child.GetComponent<RectTransform>().sizeDelta.y);
                child.GetComponent<RectTransform>().localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), Vector3.zero, t*1.2f);
            }

            yield return null;
        }
        yield return new WaitForSeconds(2f);
        bannerDisplayObj.SetActive(false);
    }
}

