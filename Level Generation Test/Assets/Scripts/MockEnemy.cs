using UnityEngine;
using System.Collections;

public class MockEnemy : MonoBehaviour
{
<<<<<<< HEAD
    public static float STARTCOMBATDISTANCE = 1.5f;
    public GameObject player,CBS;
    public bool activeEnemy;
=======

    public GameObject Player,CBS;
    public bool ActiveEnemy;
>>>>>>> 32e1eb6ae895a6fda8c382280ce46c6edf2c7b1f
    public int Health;
    public int Dmg = 15;

    private bool attacked = false;



    // Use this for initialization
    void Start()
    {
<<<<<<< HEAD


=======
        player = Camera.main.GetComponent<PlayerMovement>().player;
        CBS = Camera.main.GetComponent<PlayerMovement>().CBS;
<<<<<<< HEAD
=======

        StartCoroutine(InitEnemy());
    }

    IEnumerator InitEnemy()
    {
        yield return new WaitForSeconds(2);

        gameObject.GetComponent<EnemyBehaviour>().Initialize();
>>>>>>> origin/master
>>>>>>> 32e1eb6ae895a6fda8c382280ce46c6edf2c7b1f
    }

    // Update is called once per frame
    void Update()
    {

        // CHeck for distance between player and enemy

        // Check for attacking state

<<<<<<< HEAD
        if (((gameObject.transform.position - player.transform.position).magnitude) <= STARTCOMBATDISTANCE && (attacked == false))
=======
        if (((gameObject.transform.position - Player.transform.position).magnitude) <= 4.0f && (attacked == false))
>>>>>>> 32e1eb6ae895a6fda8c382280ce46c6edf2c7b1f
        {
            Debug.Log("TO CLOSE");
            StartCoroutine(Attackcylce());
            attacked = true;
        }

        if (Health <= 0)
        {
            DEAD();
        }

    }

    IEnumerator Attackcylce()
    {
        // Call for an attack
        Debug.Log("Starting Attack");
        CBS.SetActive(true);
        CBS.GetComponent<CombatSystem>().IncomingAttack();
        yield return new WaitForSeconds(5.0f);
        StartCoroutine(Attackcylce());
    }

    public void HealthDown(int amountDOWN)
    {
        Health -= amountDOWN;
    }

    // Funchtion to increase health
    public void HealthUp(int amountUP)
    {
        Health += amountUP;
    }

    private void DEAD()
    {
        CBS.SetActive(false);
        Destroy(gameObject);
    }


}
