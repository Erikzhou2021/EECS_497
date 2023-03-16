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
    private float w = 0;
    private float u = 0;

    private void Start()
    {
        stateDisplayObj.SetActive(false);
    }
    private void Update()
    {
        t += Time.deltaTime / 1f;
        w += Time.deltaTime;
        u += Time.deltaTime;
        //DisplayScore();
    }
    public void DisplayScore()
    {
        if (GameManager.Instance.players.Count < 2)
        {
            return;
        }
        scoreDisplay.text = GameManager.Instance.players[0].GetComponent<Player>().GetScore() + "-" +
            GameManager.Instance.players[1].GetComponent<Player>().GetScore();

        scoreDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -40); //comment later
        //StartCoroutine(ScoreAnimation()); //uncomment later
    }
    IEnumerator ScoreAnimation() // should pause gameplay / dont let player swing 
    {
        GameManager.Instance.pauseGame = true;
        w = 0;
        while (w < 1)
        {
            scoreDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, Mathf.Lerp(60f,0f, w)-40f);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        w = 0;
        while (w < 1)
        {
            scoreDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, Mathf.Lerp(0, 60f, w)-40f);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        GameManager.Instance.pauseGame = false;
    }
    public IEnumerator DisplayState()
    {
        u = 0;
        stateDisplayObj.SetActive(true);
        while(u < 1)
        {
            stateDisplayObj.transform.localPosition = new Vector3(Mathf.Lerp(1000, 0, u), stateDisplayObj.transform.localPosition.y, 0);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        u = 0;
        while (u < 1)
        {
            //scoreDisplay.transform.position = Vector3.Lerp(scoreDisplay.transform.position, new Vector3(scoreDisplay.transform.position.x, scoreDisplay.transform.position.y + 2, scoreDisplay.transform.position.z), t/.5f);
            stateDisplayObj.transform.localPosition = new Vector3(Mathf.Lerp(1000, 0, u)-1000f, stateDisplayObj.transform.localPosition.y, 0);
            yield return null;
        }
        stateDisplayObj.SetActive(false);
    }
    //public void HideState()
    //{
    //    stateDisplayObj.SetActive(false);
    //}

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

