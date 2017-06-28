using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackEnemy : MonoBehaviour {

    public GameObject tower;
	public GameObject bullet;
    public GameObject towerHead;
	public float shootingRate = 1.0f;

	float timeSinceShot;
    public float RotationSpeed;

    //values for internal use
    private Quaternion _lookRotation;
    private Vector3 _direction;
	List<GameObject> InRangeObjects;

	void Awake () {
		InRangeObjects = new List<GameObject> ();
		timeSinceShot = shootingRate;
	}

	void Start(){
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < InRangeObjects.Count; i++)
        {
            if (!InRangeObjects[i] || ((InRangeObjects[i].name == "Player_New" || InRangeObjects[i].name.Contains("Opponent")) && InRangeObjects[i].GetComponent<PlayerInformation>().health <= 0))
                InRangeObjects.Remove(InRangeObjects[i]);
            else if (!InRangeObjects[i] || (InRangeObjects[i].name.Contains("bomb") && InRangeObjects[i].transform.parent.GetComponent<MinionInformation>().health <= 0))
                InRangeObjects.Remove(InRangeObjects[i]);
        }
        if(timeSinceShot <= shootingRate)
			timeSinceShot += Time.deltaTime;
        if (InRangeObjects.Count > 0) {
            //find the vector pointing from our position to the target
            _direction = (InRangeObjects[0].transform.position - towerHead.transform.position).normalized;

            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);
            _lookRotation.x = 0;
            _lookRotation.z = 0;

            //rotate us over time according to speed until we are in the required rotation
            towerHead.transform.rotation = Quaternion.Slerp(towerHead.transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
        }
		if(InRangeObjects.Count > 0 && timeSinceShot >= shootingRate){
			Shoot ();
			timeSinceShot = 0.0f;
		}
        
	}

	void Shoot(){
		if (InRangeObjects.Count > 0) {
            //if((InRangeObjects[0].GetComponent<PlayerInformation>() && tower.GetComponent<MinionInformation>().team != InRangeObjects[0].GetComponent<PlayerInformation>().team)
                //|| InRangeObjects[0].GetComponent<MinionInformation>() && tower.GetComponent<MinionInformation>().team != InRangeObjects[0].GetComponent<MinionInformation>().team)
            if ((InRangeObjects[0].GetComponent<PlayerInformation>() && tower.GetComponent<TeamBlueOrRed>().teamBlue != InRangeObjects[0].GetComponent<TeamBlueOrRed>().teamBlue)
                || InRangeObjects[0].transform.parent.GetComponent<MinionInformation>() && tower.GetComponent<TeamBlueOrRed>().teamBlue != InRangeObjects[0].GetComponent<TeamBlueOrRed>().teamBlue)
                {
                //Debug.Log ("Attack: " + InRangeObjects[0]);
                
                GameObject tmpBullet = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity) as GameObject;
                tmpBullet.transform.parent = transform;
                tmpBullet.GetComponent<TowerBullet>().EnableChase(InRangeObjects[0]);
            }
		}
			
	}

	void OnTriggerEnter(Collider other) {
		//Debug.Log (other.transform.name + " Enters.");
        if((other.name.Contains("Player") || other.name.Contains("Opponent") || other.name.Contains("bomb")) && other.GetComponent<TeamBlueOrRed>() && other.GetComponent<TeamBlueOrRed>().teamBlue != tower.GetComponent<TeamBlueOrRed>().teamBlue && !InRangeObjects.Contains(other.gameObject))
		    InRangeObjects.Add (other.gameObject);
	}

	void OnTriggerExit(Collider other) {
		//Debug.Log (other.transform.name + " Leaves.");
		InRangeObjects.Remove (other.gameObject);
	}
}
