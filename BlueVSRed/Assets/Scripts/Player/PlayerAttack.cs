using UnityEngine;
using System.Collections;
using System;

public class PlayerAttack : MonoBehaviour
{

    public float attackSpeed = 1.0f;

    private Animator anim;
    private PlayerMovement pMovement;
    private ClickToMove2 clickMovement;
    private bool isAttacking = false;
    public AudioSource cSound;
    public GameObject sword;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        pMovement = GetComponent<PlayerMovement>();
        clickMovement = GetComponent<ClickToMove2>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set the attack animation to false when the player is attacking to make sure that he only attacks once.
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            anim.SetBool("Attack", false);
            
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dance"))
        {
            anim.SetBool("Recalling", false);
        }

        // Enable walking when leaving the attack state.
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetNextAnimatorStateInfo(0).IsName("Grounded"))
        {
            isAttacking = false;
            anim.speed = 1.0f;
            if (sword != null)
                sword.GetComponent<PlayerSwordCollider>().others.Clear();
            GetComponent<ClickToMove2>().canWalk = true;
        }
            

        // Enable walking when leaving the recall state.
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dance") && anim.GetNextAnimatorStateInfo(0).IsName("Grounded"))
        {
            anim.speed = 1.0f;
            GetComponent<ClickToMove2>().canWalk = true;
            if (GetComponent<TeamBlueOrRed>().teamBlue)
            {
                transform.position = new Vector3(0, 0, -20.5f);
            }
            else
            {
                transform.position = new Vector3(0, 0, 20.5f);
            }
        }

        if (gameObject.name == "Player_New" && Input.GetKeyDown(KeyCode.C))
        {
            if(GameManager.gameManager.online)
            {
                SkillMessage attackMessage = new SkillMessage(GetComponent<PlayerInformation>().id);
                string message = new MessageContainer("attack", attackMessage).ToJson() + "\n";
                NetworkManager.networkManager.SendData(message);
            }
            SwingSword();
        }
    }

    /// <summary>
    /// Let the player swing his sword.
    /// </summary>
    public void SwingSword()
    {
        anim.speed = attackSpeed;
        // Start the attack animation.
        anim.SetBool("Attack", true);
        isAttacking = true;
        // Disable walking when entering the attack state.
        pMovement.StopWalking();
        anim.SetFloat("Forward", 0.0f);
        clickMovement.canWalk = false;
        cSound.Play();
    }

    public void RecallAnimation()
    {
        anim.SetBool("Recalling", true);
        // Disable walking when entering the recall state.
        pMovement.StopWalking();
        anim.SetFloat("Forward", 0.0f);
        clickMovement.canWalk = false;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

}
