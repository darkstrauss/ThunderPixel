﻿using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour {

    public float dTime;

    public float Stamina, StaminaIncrease;
    public int Health;
    public GameObject DodgeCircleL, DodgeCircleR;
    public GameObject ActiveEnemy;

    private IEnumerator coroutine1, coroutine2;

    private void Awake()
    {
        ResetCo();
        StartCombat();
    }
    // Use this for initialization
    void Start () {
        Health = 100;
    }

    void OnEnable()
    {
        // Pause game
        // Update avaiblibe strikes
    }

    // Update is called once per frame
    void Update () {
        if (Health <= 0)
        {
            GameOver();
        }

        Stamina += StaminaIncrease;
        // check health
        // if health reach zero gameover
        // check stamina
	}

    public void StartCombat()
    {
       
    }

    // Reset IEnumerators for use later
    private void ResetCo()
    {
        coroutine1 = DodgeVisualIE(dTime, DodgeCircleL);
        coroutine2 = DodgeVisualIE(dTime, DodgeCircleR);

        DodgeCircleL.SetActive(false);
        DodgeCircleR.SetActive(false);
    }

    // Start when a new attack comes in
    public void IncomingAttack()
    {
        DodgeCircleL.transform.localScale = new Vector3(4f, 4f, 4f);
        DodgeCircleR.transform.localScale = new Vector3(4f, 4f, 4f);

        DodgeCircleL.SetActive(true);
        DodgeCircleR.SetActive(true);

        StartCoroutine(coroutine1);
        StartCoroutine(coroutine2);
    }

    // Used to cancel coroutines and dodge incoming attack
    public void DodgeVisual()
    {
        StopCoroutine(coroutine1);
        StopCoroutine(coroutine2);

        DodgeCircleL.SetActive(false);
        DodgeCircleR.SetActive(false);

        ResetCo();
    }

    // Function to decrease health
    public void HealthDown(int amountDOWN)
    {
        Health -= amountDOWN;
    }

    // Funchtion to increase health
    public void HealthUp (int amountUP)
    {
        Health += amountUP;
    }

    // Decrease enemyhealth
    public void EnemyHealthDown(int amountDOWN)
    {
        ActiveEnemy.GetComponent<MockEnemy>().HealthDown(amountDOWN);
    }

    // Funchtion to increase enemyhealth
    public void EnemyHealthUp(int amountUP)
    {
        ActiveEnemy.GetComponent<MockEnemy>().HealthUp(amountUP);
    }

    // Dislpay game over screen + score etc
    private void GameOver()
    {
        //Play animtion

        // Show Drag back to cell scene

    } 

    IEnumerator DodgeVisualIE(float time, GameObject rORl)
    {
        Vector3 originalScale = rORl.transform.localScale;
        Vector3 destinationScale = new Vector3(1.5f, 1.5f, 1.5f);

        float currentTime = 0.0f;

        while(currentTime <= time)
        {
            rORl.transform.localScale = Vector3.Lerp(originalScale, destinationScale ,  Mathf.SmoothStep(0, 1 ,(currentTime / time)));
            currentTime += Time.deltaTime;
            yield return null;
        }
        rORl.SetActive(false);
        HealthDown(ActiveEnemy.GetComponent<MockEnemy>().Dmg);
        DodgeVisual();
    }
}
