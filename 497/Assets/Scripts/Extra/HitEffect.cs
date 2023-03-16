using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    //attached to hit effect object
    private float t = 0;

    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, transform.localPosition.z);
        transform.localEulerAngles = new Vector3(0, 90, 0);
        StartCoroutine(Fuck());
    }
    private void Update()
    {
        t += Time.deltaTime;
    }
    IEnumerator Fuck()
    {
        t = 0;
        while (t < 1)
        {
            foreach (Transform child in transform)
            {
                //Vector3 direction = (child.localPosition - this.transform.localPosition).normalized;

                //child.localPosition += direction * t;
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x*1.2f, transform.localScale.y*1.2f, 1), t/2f);
                child.localScale = Vector3.Lerp(child.localScale, Vector3.zero, t);
                child.GetComponent<Image>().color = new Color(0f, 0.6156863f, 1f, Mathf.Lerp(1f, 0, t / 3f));
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
