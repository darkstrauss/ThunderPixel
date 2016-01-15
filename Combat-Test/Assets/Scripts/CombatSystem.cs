using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour {

    public float dTime;

    public GameObject DodgeCircleL;
    public GameObject DodgeCircleR;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void IncomingAttack()
    {
        DodgeCircleL.transform.localScale = new Vector3(4f, 4f, 4f);
        DodgeCircleR.transform.localScale = new Vector3(4f, 4f, 4f);

        DodgeCircleL.SetActive(true);
        DodgeCircleR.SetActive(true);

        StartCoroutine(DodgeVisualIE(dTime, DodgeCircleL));
        StartCoroutine(DodgeVisualIE(dTime, DodgeCircleR));
    }

    public void DodgeVisual()
    {
        
    }

    IEnumerator DodgeVisualIE(float time, GameObject rORl)
    {
        Vector3 originalScaleL = rORl.transform.localScale;
        Vector3 destinationScale = new Vector3(1.5f, 1.5f, 1.5f);

        //t = t * t * (3f - 2f * t);

        float currentTime = 0.0f;

        while(currentTime <= time)
        {
            rORl.transform.localScale = Vector3.Lerp(rORl.transform.localScale, destinationScale ,  Mathf.SmoothStep(0, 1 ,(currentTime / time)));
            currentTime += Time.deltaTime;
            yield return null;
        }
        rORl.SetActive(false);
       
    }
}
