using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Joystick joystick;
    [SerializeField] float speed = 0f;
    [SerializeField] Vector2 playerClampPos;
    [SerializeField] Vector2 angleClampPos;
    [SerializeField] GameObject angleObject;


    private Vector3 dir;
    private Vector3 playerPos;
    private Vector3 anglePos;

    private void Update()
    {
        dir.x = joystick.Horizontal;
        dir.y = joystick.Vertical;

        playerPos += dir * speed * Time.deltaTime;
        playerPos.x = Mathf.Clamp(playerPos.x, -playerClampPos.x, playerClampPos.x);
        playerPos.y = Mathf.Clamp(playerPos.y, -playerClampPos.y, playerClampPos.y);

        anglePos = playerPos;
        anglePos.x = Mathf.Clamp(anglePos.x, -angleClampPos.x, angleClampPos.x);
        anglePos.y = Mathf.Clamp(anglePos.y, -angleClampPos.y, angleClampPos.y);

        this.transform.position = playerPos;
        angleObject.transform.position = anglePos;




    }
}
