using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingHealthBar: MonoBehaviour {

    public float playerHealth;
    public float currentHealth;
    public GameObject healthBar;
    public GameObject playerObject;
    private PlayerInformation player;

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
            healthBar.GetComponent<Image>().fillAmount = currentHealth / player.maxHealth;
    }

}
