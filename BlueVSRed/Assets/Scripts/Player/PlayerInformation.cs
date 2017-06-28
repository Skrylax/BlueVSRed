using UnityEngine;
using System.Collections;

public class PlayerInformation : MonoBehaviour {

    // Use this for initialization
    void Awake()
    {
        targetPosition = transform.position;
    }
    public int id;
    public string pName;
    public Vector3 targetPosition;
    public bool otherPlayer = false;
    public TextMesh playerNameText;
    public GameObject head;
    public GameObject body;

    //Player stats
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;
    public float baseMoveSpeed;
    public float movementSpeed;
    private float manaIncrement = 0.0f;

    bool isAlive;
    bool isRespawning;

    // Use this for initialization
    void Start()
    {
        maxHealth = health;
        maxMana = mana;
        isAlive = true;

        if (GetComponent<TeamBlueOrRed>().teamBlue)
        {
            transform.position = new Vector3(0, 0, -20.5f);
        }
        else
        {
            transform.position = new Vector3(0, 0, 20.5f);
        }
    }

    // Update is called once per frame
    void Update () {

        if (health <= 0 && gameObject.activeSelf)
        {
            gameObject.GetComponent<PlayerInformation>().isAlive = false;
            gameObject.GetComponent<PlayerInformation>().isRespawning = true;
        }


        if (health > maxHealth)
            health = maxHealth;

        if (!isAlive)
        {
            if (isRespawning)
            {
                isRespawning = false;
                StartCoroutine("RespawnPlayer");
            }
        }

        manaIncrement += Time.deltaTime * 0.5f;

        if ((int)manaIncrement >= 1) {
            if (mana < maxMana)
                mana++;
            manaIncrement--;
        }

        if (mana >= maxMana)
            mana = maxMana;

        if (health < 0)
            health = 0;
        
        if (mana < 0)
            mana = 0;
	}
    
    public void ResetMoveSpeed()
    {
        movementSpeed = baseMoveSpeed;
    }

    IEnumerator RespawnPlayer()
    {
        if (GetComponent<TeamBlueOrRed>().teamBlue)
        {
            transform.position = new Vector3(0, 0, -20.5f);
        }
        else
        {
            transform.position = new Vector3(0, 0, 20.5f);
        }

        movementSpeed = 0;

        yield return new WaitForSeconds(5);

        ResetMoveSpeed();
        health = maxHealth;
        mana = maxMana;
        isAlive = true;

    }
}
