using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{

    public GameObject player;
    public static PlayerInput playerInput;

    private float qCooldown = 5;
    private float wCooldown = 20;
    private float eCooldown = 10;
    private float rCooldown = 60;

    private int qManaCost = 10;
    private int wManaCost = 0;
    private int eManaCost = 0;
    private int rManaCost = 30;

    private bool qOnCooldown = false;
    private bool wOnCooldown = false;
    private bool eOnCooldown = false;
    private bool rOnCooldown = false;

    public AudioSource qSound;
    public AudioSource wSound;
    public AudioSource eSound;
    public AudioSource rSound;
    public AudioSource bSound;

    public Image imageQ;
    public Image imageW;
    public Image imageE;
    public Image imageR;

    IEnumerator instQ;
    IEnumerator instW;
    IEnumerator instE;

    // Use this for initialization
    void Awake()
    {
        playerInput = this;
    }

    // Update is called once per frame
    void Update()
    {
        GetKeyInput();

    }

    void GetKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Q) && imageQ.fillAmount >= 0.989)
        {
            if(GameManager.gameManager.online)
            {
                SkillMessage qMessage = new SkillMessage(player.GetComponent<PlayerInformation>().id);
                string message = new MessageContainer("q", qMessage).ToJson() + "\n";
                NetworkManager.networkManager.SendData(message);
            }
            StartCoroutine("AbilityQ");
        }

        if (Input.GetKeyDown(KeyCode.W) && imageW.fillAmount >= 0.989)
        {
            if(GameManager.gameManager.online)
            {
                SkillMessage wMessage = new SkillMessage(player.GetComponent<PlayerInformation>().id);
                string message = new MessageContainer("w", wMessage).ToJson() + "\n";
                NetworkManager.networkManager.SendData(message);
            }
            StartCoroutine("AbilityW");
        }

        if (Input.GetKeyDown(KeyCode.E) && imageE.fillAmount >= 0.989)
        {
            if(GameManager.gameManager.online)
            {
                SkillMessage eMessage = new SkillMessage(player.GetComponent<PlayerInformation>().id);
                string message = new MessageContainer("e", eMessage).ToJson() + "\n";
                NetworkManager.networkManager.SendData(message);
            }
            StartCoroutine("AbilityE");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine("AbilityR");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if(GameManager.gameManager.online)
            {
                SkillMessage recallMessage = new SkillMessage(player.GetComponent<PlayerInformation>().id);
                string message = new MessageContainer("recall", recallMessage).ToJson() + "\n";
                NetworkManager.networkManager.SendData(message);
            }

            StartCoroutine("AbilityB");
        }
    }

    IEnumerator AbilityQ()
    {
        if (player.GetComponent<PlayerInformation>().health < player.GetComponent<PlayerInformation>().maxHealth)
        {
            instQ = CooldownFillAmount(imageQ, qCooldown);
            StartCoroutine(instQ);
            qOnCooldown = true;
            IncreaseHealth(player);
            qSound.Play();
            yield return new WaitForSeconds(qCooldown);
            qOnCooldown = false;
        }
    }

    public void IncreaseHealth(GameObject player)
    {
        if (player.GetComponent<PlayerInformation>().health < player.GetComponent<PlayerInformation>().maxHealth)
        {
            player.GetComponent<PlayerInformation>().mana -= qManaCost;
            player.GetComponent<PlayerInformation>().health += player.GetComponent<PlayerInformation>().maxHealth / 100 * 10;
        }
    }

    IEnumerator AbilityW()
    {
        instW = CooldownFillAmount(imageW, wCooldown);
        StartCoroutine(instW);
        wOnCooldown = true;
        int waitTime = 10;
        IncreaseAttackSpeed(player, waitTime);
        wSound.Play();
        yield return new WaitForSeconds(wCooldown - waitTime);
        wOnCooldown = false;
    }

    public void IncreaseAttackSpeed(GameObject player, int waitTime)
    {
        player.GetComponent<PlayerInformation>().mana -= wManaCost;
        player.GetComponent<PlayerAttack>().attackSpeed *= 1.3f;
        StartCoroutine(ResetAttackSpeed(player, waitTime));
    }

    IEnumerator AbilityE()
    {
        instE = CooldownFillAmount(imageE, eCooldown);
        StartCoroutine(instE);
        eOnCooldown = true;
        int waitTime = 3;
        IncreaseMoveSpeed(player, waitTime);
        eSound.Play();
        yield return new WaitForSeconds(eCooldown);
        eOnCooldown = false;
    }

    public void IncreaseMoveSpeed(GameObject player, int waitTime)
    {
        player.GetComponent<PlayerInformation>().mana -= eManaCost;
        player.GetComponent<PlayerMovement>().movementSpeed *= 1.5f;
        StartCoroutine(ResetMoveSpeed(player, waitTime));
    }

    IEnumerator AbilityR()
    {
        if (imageR.fillAmount >= 0.989)
        {
            if (instQ != null)
            {
                StopCoroutine(instQ);
                StopCoroutine("AbilityQ");
            }
            if (instW != null)
            {
                StopCoroutine(instW);
                StopCoroutine("AbilityW");

            }
            if (instE != null)
            {
                StopCoroutine(instE);
                StopCoroutine("AbilityE");
            }
            imageQ.fillAmount = 1;
            imageW.fillAmount = 1;
            imageE.fillAmount = 1;

            StartCoroutine(CooldownFillAmount(imageR, rCooldown));
            rOnCooldown = true;
            player.GetComponent<PlayerInformation>().mana -= rManaCost;
            qOnCooldown = false;
            wOnCooldown = false;
            eOnCooldown = false;
            rSound.Play();
            yield return new WaitForSeconds(rCooldown);
            rOnCooldown = false;
        }
    }

    IEnumerator AbilityB()
    {
        player.GetComponent<PlayerAttack>().RecallAnimation();
        yield return null;
    }

    IEnumerator CooldownFillAmount(Image img, float cooldown)
    {
        float maxCooldownTime = cooldown;
        float cooldownTime = maxCooldownTime;

        while (cooldownTime >= 0)
        {
            img.fillAmount = 1 - (cooldownTime / maxCooldownTime);
            yield return new WaitForSeconds(0.1f);
            cooldownTime -= 0.1f;
        }
    }

    IEnumerator ResetMoveSpeed(GameObject player, int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        player.GetComponent<PlayerMovement>().ResetMovementSpeed();
    }
    IEnumerator ResetAttackSpeed(GameObject player, int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        player.GetComponent<PlayerAttack>().attackSpeed = 1.0f;
    }
}
