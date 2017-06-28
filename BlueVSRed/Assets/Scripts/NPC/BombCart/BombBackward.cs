using UnityEngine;
using System.Collections;

public class BombBackward : BombCartState {

    public override void Execute() {
        if(bCart == null)
            bCart = GetComponent<BombCart>();
        if (bCart.percentage <= 0.0f)
        {
            // Go back to idle state.
            bCart.BombCartState = GetComponent<BombIdle>();
        }
        else { 
            // Move backwards.
            bCart.SetCurrentMoveTime(bCart.GetCurrentMoveTime() - Time.deltaTime / 2f);
            if (bCart.GetCurrentMoveTime() > bCart.moveTime)
            {
                bCart.SetCurrentMoveTime(bCart.moveTime);
            }

            bCart.percentage = bCart.GetCurrentMoveTime() / bCart.moveTime;
            transform.position = new Vector3(bCart.GetStartPosition().x, bCart.GetStartPosition().y, Mathf.Lerp(bCart.GetStartPosition().z, bCart.GetEndPosition().z, bCart.percentage));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log (other.transform.name + " Enters.");
        if (other.GetComponent<TeamBlueOrRed>() && (other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("opponent")) && other.GetComponent<TeamBlueOrRed>().teamBlue == GetComponent<TeamBlueOrRed>().teamBlue)
        {
            bCart = GetComponent<BombCart>();
            
            if (!bCart.GetInRangeObjectList().Contains(other.gameObject))
                bCart.GetInRangeObjectList().Add(other.gameObject);
            
            bCart.BombCartState = GetComponent<BombForward>();
        }

    }
	
}
