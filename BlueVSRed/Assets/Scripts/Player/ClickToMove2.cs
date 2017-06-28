using UnityEngine;
using System.Collections;

public class ClickToMove2 : MonoBehaviour {

	public float Speed;                         // Speed at which the character moves
	public float Direction;                    // The Speed the character will move
	public Animator anim;                     // Animator to Anim converter
    public bool canWalk = true;
	private PlayerMovement pathfinding;
    private float timeSinceLast;
    



	void Start()
	{
		pathfinding = GetComponent<PlayerMovement> ();
		anim = GetComponent<Animator>();
	}

    void Update()
    {
        timeSinceLast += Time.deltaTime;
    }

	void FixedUpdate() {
       
        //if (pathfinding.currentSpeed >= 6f){
        //    anim.SetFloat("Forward", 2.0f);
        //}
        //else{
        //    anim.SetFloat("Forward", 0.0f);
        //} //


        // Moves the Player if the Left Mouse Button was clicked

        if (timeSinceLast >= 0.01f)
        {
            if (Input.GetMouseButton(1) && canWalk)
            {

                //Plane playerPlane = new Plane(Vector3.up, myTransform.position);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitdist;

                if (Physics.Raycast(ray, out hitdist, 1000))
                {
                    Vector3 targetPoint = hitdist.point;
                    //pathfinding.SendPathRequest (targetPoint);
                    pathfinding.targetPosition = targetPoint;
                    pathfinding.SendPathRequest();
                }

            }
            timeSinceLast = 0.0f;
        }
        if(Input.GetMouseButtonDown(1) && canWalk)
        {

            //Plane playerPlane = new Plane(Vector3.up, myTransform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitdist;

            if (Physics.Raycast(ray, out hitdist, 1000))
            {
                Vector3 targetPoint = hitdist.point;
                //pathfinding.SendPathRequest (targetPoint);
                pathfinding.targetPosition = targetPoint;
                pathfinding.SendPathRequest();
            }
            timeSinceLast = 0.0f;
        }
    }
}
