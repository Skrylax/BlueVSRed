using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float scrollSpeed = 8;
    public bool enableBorderMovement = true;
    Vector3 panTranslation;
    bool leftKeyDown = false;
    bool rightKeyDown = false;
    bool spaceKeyDown = false;
    

    void Update()
    {
        // read key inputs
        leftKeyDown = Input.GetKey(KeyCode.LeftArrow);
        rightKeyDown = Input.GetKey(KeyCode.RightArrow);
        spaceKeyDown = Input.GetKey(KeyCode.Space);

        // determine panTranslation
        panTranslation = Vector3.zero;
        if (spaceKeyDown)
            transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z - 1.75f);
        else if (((leftKeyDown && !rightKeyDown) || ((Input.mousePosition.x < 25 || Input.mousePosition.y  > Screen.height - 25) && enableBorderMovement)) && transform.position.z < 20f)
            panTranslation += Vector3.forward * Time.deltaTime * scrollSpeed;
        else if (((rightKeyDown && !leftKeyDown) || ((Input.mousePosition.x > Screen.width - 25 || Input.mousePosition.y < 25) && enableBorderMovement)) && transform.position.z > -30f)
            panTranslation += Vector3.back * Time.deltaTime * scrollSpeed;

        transform.position += panTranslation;
    }
}
