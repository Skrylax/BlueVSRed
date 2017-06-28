using UnityEngine;
using System.Collections;

public class BombForward : BombCartState {


    void Awake() {
        bCart = GetComponent<BombCart>();
    }

    public override void Execute()
    {
        bCart.SetCurrentMoveTime(bCart.GetCurrentMoveTime() + Time.deltaTime);
        if (bCart.GetCurrentMoveTime() > bCart.moveTime)
        {
            bCart.SetCurrentMoveTime(bCart.moveTime);
        }

        bCart.percentage = bCart.GetCurrentMoveTime() / bCart.moveTime;
        transform.position = new Vector3(bCart.GetStartPosition().x, bCart.GetStartPosition().y, Mathf.Lerp(bCart.GetStartPosition().z, bCart.GetEndPosition().z, bCart.percentage));

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TeamBlueOrRed>() && (other.gameObject.name.ToLower().Contains("player") || other.gameObject.name.ToLower().Contains("opponent")) && other.GetComponent<TeamBlueOrRed>().teamBlue == GetComponent<TeamBlueOrRed>().teamBlue)
        {
            if (!bCart.GetInRangeObjectList().Contains(other.gameObject))
                bCart.GetInRangeObjectList().Add(other.gameObject);
        }
        if ((other.name.Contains("Tower") || other.name.Contains("Core")) && other.GetComponent<TeamBlueOrRed>() && other.GetComponent<TeamBlueOrRed>().teamBlue != GetComponent<TeamBlueOrRed>().teamBlue)
        {
            bCart.GetEnemyTowersInRange().Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<TeamBlueOrRed>() && other.GetComponent<TeamBlueOrRed>().teamBlue == GetComponent<TeamBlueOrRed>().teamBlue)
        {

            bCart.GetInRangeObjectList().Remove(other.gameObject);


            // Start moving backwards if there are no more teammembers in range.
            if (bCart.GetInRangeObjectList().Count == 0)
            {
                bCart.BombCartState = GetComponent<BombBackward>();
            }
        }
        if ((other.name.Contains("Tower") || other.name.Contains("Core") && other.GetComponent<TeamBlueOrRed>() && other.GetComponent<TeamBlueOrRed>().teamBlue != GetComponent<TeamBlueOrRed>().teamBlue))
        {
            bCart.GetEnemyTowersInRange().Remove(other.gameObject);
        }
    }
}
