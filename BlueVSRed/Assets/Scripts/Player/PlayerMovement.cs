using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour {

    //public Transform target;
    public float movementSpeed;
    public float defaultSpeed;
	public float currentSpeed; 
	public Vector3 targetPosition;
    public float RotationSpeed = 6;
    public Vector3[] path;

    private Quaternion _lookRotation;
    private Vector3 _direction;
    private Animator anim;
    private int targetIndex;
	private Pathfinding pathfinding;
    private PathRequestManager pm;
    private PlayerInformation playerInfo;
    

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerInfo = GetComponent<PlayerInformation>();
        pm = GetComponent<PathRequestManager>();
		pathfinding = GetComponent<Pathfinding>();
		currentSpeed = 0.0f;
		//StartCoroutine ("UpdatePath");
        //PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

	IEnumerator UpdatePath(){
		while(targetPosition != null){
			SendPathRequest ();
			yield return new WaitForSeconds (0f);
		}
	}

	public void SendPathRequest(){
        if (Vector3.Distance(transform.position, targetPosition) > 0.3f)
        {
            pm.RequestPath(transform.position, targetPosition, OnPathFound);
        }
	}

    /// <summary>
    /// Walk a new path
    /// </summary>
    /// <param name="newPath">new path to follow</param>
    /// <param name="pathSuccessful">is the new path successfull or not</param>
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{   
        if(pathSuccessful)
        {
            path = newPath;

            if (gameObject.name == "Player_New" && GameManager.gameManager.online){
                    PathMessage pathMessage = new PathMessage(gameObject.GetComponent<PlayerInformation>().id, path);
                    string message = new MessageContainer("playerpath", pathMessage).ToJson() + "\n";
                    NetworkManager.networkManager.SendData(message);
            }

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
       }
    }

    public IEnumerator FollowPath()
    {
		if (path.Length == 0) {
			currentSpeed = 0.0f;
            anim.SetFloat("Forward", 0.0f);
			yield break;
		}
        
		currentSpeed = movementSpeed;
        anim.SetFloat("Forward", 2.0f);
        targetIndex = 0;
        Vector3 currentWaypoint = path[0];    
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;

                if (targetIndex >= path.Length)
                {
					currentSpeed = 0.0f;
                    anim.SetFloat("Forward", 0.0f);
					path = null;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
			//transform.rotation = Quaternion.LookRotation(currentWaypoint - transform.position)/*Quaternion.LookRotation (Vector3.RotateTowards(transform.forward, currentWaypoint, currentSpeed * 10f * Time.deltaTime, 0.0f))*/;
            //find the vector pointing from our position to the target
            _direction = (currentWaypoint - transform.position).normalized;

            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);
            _lookRotation.x = 0;
            _lookRotation.z = 0;

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, currentSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void StopWalking() {
        StopCoroutine("FollowPath");
    }

    public void OnDrawGizmos()
    {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path [i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				} else {
					Gizmos.DrawLine (path [i - 1], path [i]);
				}
			}
		}
    }
    public void ResetMovementSpeed()
    {
        movementSpeed = defaultSpeed;
    }
}
