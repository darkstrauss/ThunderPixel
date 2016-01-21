using UnityEngine;
using System.Collections;

public class MockEnemy : MonoBehaviour
{

    public GameObject Player,CBS;
    public bool ActiveEnemy;
    public int Health;

    private bool attacked = false;



    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

        // CHeck for distance between player and enemy

        // Check for attacking state

        if (((gameObject.transform.position - Player.transform.position).magnitude) <= 4.0f && (attacked == false))
        {
            Debug.Log("TO CLOSE");
            Attackcylce();
            attacked = true;
        }

    }

    private void Attackcylce()
    {
        // Call for an attack
        Debug.Log("Starting Attack");
        CBS.SetActive(true);
        CBS.GetComponent<CombatSystem>().IncomingAttack();
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
        Destroy(gameObject);
    }
}
