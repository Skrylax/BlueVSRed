using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSwordCollider : MonoBehaviour {

    private int swordDamage = 10;
    public GameObject player;
    public List<GameObject> others = new List<GameObject>();

    void OnTriggerStay(Collider other)
    {
        if (player.GetComponent<PlayerAttack>().IsAttacking() && !others.Contains(other.gameObject))
        {
            if ((other.name.Contains("Tower") || other.name.Contains("Core")) && other.GetComponent<TeamBlueOrRed>() && other.GetComponent<TeamBlueOrRed>().teamBlue != player.GetComponent<TeamBlueOrRed>().teamBlue)
            {
                if (GameManager.gameManager.online) {
                    DamageMessage damageMessage = new DamageMessage(other.GetComponent<MinionInformation>().buildingId, other.GetComponent<MinionInformation>().maxHealth / 100 * swordDamage);
                    string message = new MessageContainer("bDamage", damageMessage).ToJson() + "\n";
                    NetworkManager.networkManager.SendData(message);
                }
                else
                {
                    other.GetComponent<MinionInformation>().health -= (other.GetComponent<MinionInformation>().maxHealth / 100 * swordDamage);
                }
                others.Add(other.gameObject);
            }
            if (other.name.Contains("bomb") && other.GetComponent<TeamBlueOrRed>() && other.GetComponent<TeamBlueOrRed>().teamBlue != player.GetComponent<TeamBlueOrRed>().teamBlue)
            {
                if (GameManager.gameManager.online)
                {
                    DamageMessage damageMessage = new DamageMessage(other.GetComponent<MinionInformation>().buildingId, other.transform.parent.GetComponent<MinionInformation>().maxHealth / 100 * swordDamage);
                    string message = new MessageContainer("bDamage", damageMessage).ToJson() + "\n";
                    NetworkManager.networkManager.SendData(message);
                }
                else
                {
                    other.transform.parent.GetComponent<MinionInformation>().health -= (other.transform.parent.GetComponent<MinionInformation>().maxHealth / 100 * swordDamage);
                }
                others.Add(other.gameObject);
            }
            if ((other.name.Contains("Player") || other.name.Contains("Opponent")) && other.GetComponent<TeamBlueOrRed>() && other.GetComponent<TeamBlueOrRed>().teamBlue != player.GetComponent<TeamBlueOrRed>().teamBlue)
            {
                if (GameManager.gameManager.online)
                {
                    DamageMessage damageMessage = new DamageMessage(other.GetComponent<PlayerInformation>().id, other.GetComponent<PlayerInformation>().maxHealth / 100 * swordDamage);
                    string message = new MessageContainer("pDamage", damageMessage).ToJson() + "\n";
                    NetworkManager.networkManager.SendData(message);
                }
                else
                {
                    other.GetComponent<PlayerInformation>().health -= (other.GetComponent<PlayerInformation>().maxHealth / 100 * swordDamage);
                }
                others.Add(other.gameObject);
            }
        }
    }
}
