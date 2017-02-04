using System.Collections;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;

    //artificial gravity
    private float vertVelo = 0.5f;
    private float gravity = 12.0f;

    public float Speed = 3.0f;
    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = Vector3.zero;
        if (controller.isGrounded)
        {
            vertVelo = 0.5f;
        }
        else
        {
            vertVelo -= gravity * Time.deltaTime;
        }

        if (BalanceBoardManager.Current.IsBalanceBoardConnected)
        {
            moveVector.x = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight * Speed;
        }

        moveVector.x = Input.GetAxisRaw("Horizontal") * Speed;

        moveVector.y = vertVelo;
        moveVector.z = Speed;
        controller.Move(moveVector * Time.deltaTime);
    }
}
