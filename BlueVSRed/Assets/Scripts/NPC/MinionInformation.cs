using UnityEngine;
using System;
using System.Collections;

public class MinionInformation : MonoBehaviour {

    public int buildingId;
    public int health;
    public int maxHealth;
    public int movementSpeed;
    public GameObject timer;
    public GameObject head;
    public GameObject body;

    // Use this for initialization
    void Awake () {
        health = maxHealth;
    }
	
	// Update is called once per frame
	void Update () {
        if (health <= 0 && gameObject.activeSelf && !gameObject.name.Contains("Core"))
            Destroy(gameObject);
        else if (health <= 0 && gameObject.activeSelf && gameObject.name.Contains("Core"))
            GetComponent<Animator>().SetBool("Explode", true);
    }

}
