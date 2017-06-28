using UnityEngine;
using System.Collections;

public class SimpleMinion : MonoBehaviour {

    //public Transform target;
	public float defaultSpeed = 5.0f;
	public float currentSpeed;
	public Vector3 targetPosition;
    public string teamColor;

    private PathRequestManager pm;
    //private Pathfinding pathfinder;
    private Transform target;
    Vector3 blueTarget = new Vector3(-47, 3, 47);
    Vector3 redTarget = new Vector3(47, 3, -47);
    private Vector3[] path;
    private int targetIndex;
	//private Pathfinding pathfinding;
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
        pm = GetComponent<PathRequestManager>();
		currentSpeed = 0.0f;
        //pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        //target = GameObject.FindGameObjectWithTag("CoreRed").transform;

        if (teamColor == "Blue")
        {
            pm.RequestPath(transform.position, blueTarget, OnPathFound);
        }
        else
        {
            pm.RequestPath(transform.position, redTarget, OnPathFound);
        }
        InvokeRepeating("RequestPathForMinion", 0.01f, 0.5f);
      
    }

    private void RequestPathForMinion()
    {
        if (teamColor == "Blue")
        {
            pm.RequestPath(transform.position, blueTarget, OnPathFound);
        }
        else
        {
            pm.RequestPath(transform.position, redTarget, OnPathFound);
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
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
       }
    }

    private IEnumerator FollowPath()
    {
		if (path.Length == 0) {
			currentSpeed = 0.0f;
            anim.SetFloat("Forward", 0.0f);
			yield break;
		}
		currentSpeed = defaultSpeed;
        anim.SetFloat("Forward", 2.0f);
        targetIndex = 0;
        Vector3 currentWaypoint = path[0];    
        while (true)
        {
			if (Mathf.Abs(Vector3.Distance(path[path.Length - 1], transform.position)) < 2.0f)
				currentSpeed = 2.0f;
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
			transform.rotation = Quaternion.LookRotation(currentWaypoint - transform.position)/*Quaternion.LookRotation (Vector3.RotateTowards(transform.forward, currentWaypoint, currentSpeed * 10f * Time.deltaTime, 0.0f))*/;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, currentSpeed * Time.deltaTime);
            yield return null;
        }
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
}
