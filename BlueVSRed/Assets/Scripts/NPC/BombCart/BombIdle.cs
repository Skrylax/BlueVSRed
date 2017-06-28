using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombIdle : BombCartState {

    public override void Execute()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log (other.transform.name + " Enters.");
        if (other.GetComponent<TeamBlueOrRed>() && (other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("opponent")) && other.GetComponent<TeamBlueOrRed>().teamBlue == GetComponent<TeamBlueOrRed>().teamBlue)
        { 
            bCart = GetComponent<BombCart>();
            
            if(!bCart.GetInRangeObjectList().Contains(other.gameObject))
                bCart.GetInRangeObjectList().Add(other.gameObject);
            
            bCart.BombCartState = GetComponent<BombForward>();
        }
            
    }
}
