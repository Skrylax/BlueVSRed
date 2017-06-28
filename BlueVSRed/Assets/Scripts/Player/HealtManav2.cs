using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthManav2 : MonoBehaviour {

    public float playerHealth;
    //public float playerMana;

    public float currentHealth;
    //public float currentMana;
    
    public RectTransform healthBar;
    //public RectTransform manaBar;

    public GameObject playerObject;
    private PlayerInformation player;
    //public Text displayPlayerHealth;
    //public Text displayPlayerMana;

    // Use this for initialization
    void Start () {
        player = playerObject.GetComponent<PlayerInformation>();
    }
	
	// Update is called once per frame
	void Update () {
        GetHealthAndMana();
    }

    void GetHealthAndMana()
    {
        currentHealth = player.health;
        if (currentHealth >= 0)
        {
            healthBar.localScale = new Vector3((currentHealth / player.maxHealth), 1, 1);
        }

        //currentMana = player.mana;
        //if (currentMana >= 0)
        //{
        //    manaBar.localScale = new Vector3((currentMana / player.maxMana), 1, 1);
        //}

        //displayPlayerHealth.text = string.Format("{0}/{1}", currentHealth , player.maxHealth);
        //displayPlayerMana.text = string.Format("{0}/{1}", currentMana , player.maxMana);
    }

}
