using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingHealthBarTower: MonoBehaviour {

    public float towerHealth;

    public float currentHealth;
    
    public GameObject healthBar;

    private GameObject towerObject;
    private MinionInformation tower;

    // Use this for initialization
    void Start () {
        towerObject = transform.parent.gameObject;
        tower = towerObject.GetComponent<MinionInformation>();
    }
	
	// Update is called once per frame
	void Update () {
        GetHealth();
    }

    void GetHealth()
    {
        currentHealth = tower.health;
        if (currentHealth >= 0)
        {
            healthBar.GetComponent<Image>().fillAmount = currentHealth / tower.maxHealth;
        }
        
    }

}
