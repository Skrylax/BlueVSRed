using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombCart : MonoBehaviour {

    public Transform destination;
    public GameObject cartPrefab;
    public GameObject explosionPrefab;
    public float percentage;
    public float moveTime = 40f;

    private float currentLerpTime = 0f;
    private int bombDamage;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private BombCartState state;
    private List<GameObject> InRangeObjects;
    private List<GameObject> enemyTowersInRange;

    public BombCartState BombCartState {
        get {
            return state;
        }
        set {
            state = value;
        }
    }

	// Use this for initialization
	void Awake () {
        this.InRangeObjects = new List<GameObject>();
        this.enemyTowersInRange = new List<GameObject>();
        this.BombCartState = GetComponent<BombIdle>();
        this.startPosition = transform.position;
        if(GetComponent<TeamBlueOrRed>().teamBlue)
            this.endPosition = GameObject.FindGameObjectWithTag("EndPositionBlue").transform.position;
        else
            this.endPosition = GameObject.FindGameObjectWithTag("EndPositionRed").transform.position;
        this.percentage = 0.0f;
        this.bombDamage = 1000;
	}
	
	// Update is called once per frame
	void Update () {
        this.state.Execute();
        SetBombCartDamage();

        if (percentage >= 0.63f && percentage <= 0.68f && enemyTowersInRange.Count>=1)
        {
            DoDamage();
            Destroy(this.gameObject);
            
        }
        if (percentage >= 0.92f && percentage <= 0.97f && enemyTowersInRange.Count >= 1)
        {
            DoDamage();
            Destroy(this.gameObject);

        }
        if (percentage == 1.0f)
        {
            DoDamage();
            Destroy(this.gameObject);           
        }
    }

    private void DoDamage()
    {
        foreach (GameObject tower in enemyTowersInRange)
        {
            Component[] towers = tower.transform.parent.transform.GetComponentsInChildren<MinionInformation>();
            foreach (Component comp in towers)
            {
                comp.GetComponent<MinionInformation>().health -= bombDamage;
            }
        }
    }

    public Vector3 GetStartPosition() {
        return this.startPosition;
    }

    public Vector3 GetEndPosition() {
        return this.endPosition;
    }

    public List<GameObject> GetInRangeObjectList() {
        return this.InRangeObjects;
    }

    public List<GameObject> GetEnemyTowersInRange()
    {
        return this.enemyTowersInRange;
    }

    public float GetCurrentMoveTime() {
        return this.currentLerpTime;
    }

    public void SetCurrentMoveTime(float currentMoveTime) {
        this.currentLerpTime = currentMoveTime;
    }

    public int SetBombCartDamage()
    {
        bombDamage = this.GetComponent<MinionInformation>().health;
        return this.bombDamage;
    }

    void OnDestroy()
    {
        if (!GameManager.gameManager.appClosing)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            GameObject newCart = Instantiate(cartPrefab, startPosition, transform.localRotation) as GameObject;
            newCart.transform.localScale = new Vector3(3,3,3);
            newCart.SetActive(true);
            foreach (Behaviour childComponent in newCart.GetComponentsInChildren<Behaviour>())
             childComponent.enabled = true;
        }
    }
}
