using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class SphereParentCode : MonoBehaviour {

    private List<int> Strike1 = new List<int>();
    private List<int> Strike2 = new List<int>();
    private List<int> Strike3 = new List<int>();
    private List<int> Strike4 = new List<int>();

    private string IDString1, IDString2, IDString3, IDString4;
    private bool striking;
    private int currentStrike = 1;

    public GameObject Temp;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
   
	}

    public void AddSphereID (int IDinput)
    {
        striking = true;
        if (currentStrike == 1 )
            Strike1.Add(IDinput);
        if (currentStrike == 2)
            Strike2.Add(IDinput);
        if (currentStrike == 3)
            Strike3.Add(IDinput);
        if (currentStrike == 4)
            Strike4.Add(IDinput);
    }

    public void EndStrike()
    {
        striking = false;
        currentStrike++;
        StopCoroutine(WaitForStrike());
        StartCoroutine(WaitForStrike());
    }  

    public void ReturnSphereIDs()
    {
        foreach (var item in Strike1)
        {
            IDString1 += item + " ";
        }
        foreach (var item in Strike2)
        {
            IDString2 += item + " ";
        }
        foreach (var item in Strike3)
        {
            IDString3 += item + " ";
        }
        foreach (var item in Strike4)
        {
            IDString4 += item + " ";
        }
        Debug.Log("returing ID's Strike 1:" + IDString1);
        Debug.Log("returing ID's Strike 2:" + IDString2);
        Debug.Log("returing ID's Strike 3:" + IDString3);
        Debug.Log("returing ID's Strike 4:" + IDString4);
        IDString1 = "";
        IDString2 = "";
        IDString3 = "";
        IDString4 = "";
        Strike1.Clear();
        Strike2.Clear();
        Strike3.Clear();
        Strike4.Clear();
        currentStrike = 1;
    }

    IEnumerator WaitForStrike()
    {
        yield return new WaitForSeconds(0.8f);
        if (striking == false)
        {
            ReturnSphereIDs();
        } 
    }

}
