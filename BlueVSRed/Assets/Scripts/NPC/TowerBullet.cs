using UnityEngine;
using System.Collections;

public class TowerBullet : MonoBehaviour {

	public float bulletSpeed = 1.0f;

	bool chase = false;
	GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(chase && target != null) {
			transform.position = Vector3.MoveTowards (transform.position, target.transform.position, Time.deltaTime * bulletSpeed);
		}
	}

	/// <summary>
	/// Enable chasing the specified target.
	/// </summary>
	/// <param name="target">Target.</param>
	public void EnableChase(GameObject target){
		this.target = target;
		chase = true;
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == target) {
			//Debug.Log ("Target hit!");
			chase = false;
            if (other.gameObject.name == "Player_New" || other.name.Contains("Opponent"))
                target.gameObject.GetComponent<PlayerInformation>().health -= (int)(target.gameObject.GetComponent<PlayerInformation>().maxHealth * 0.1f);
            else if(other.gameObject.name.Contains("bomb"))
                target.gameObject.transform.parent.GetComponent<MinionInformation>().health -= (int)(target.gameObject.transform.parent.GetComponent<MinionInformation>().maxHealth * 0.1f);
            Destroy(gameObject);
		}
	}
}
