using System.Collections;
using System.Linq;
using UnityEngine;
using Wii.Input.Controllers;
using Wii.Input.DesktopFacades;
using Wii.Input.Events;
using Wii.Input.Exceptions;
using Wii.Input.Contracts;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;
    private BalanceBoard balanceBoard;

    //artificial gravity
    private float vertVelo = 0.5f;
    private float gravity = 12.0f;

    public float Speed = 3.0f;
    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        try
        {
            balanceBoard = WiiInputManager.Current.FindWiiController(ControllerType.WiiBalanceBoard) as BalanceBoard;
        }
        catch (WiiControllerNotFoundException)
        {
            // Kein Controller angeschlossen
        }
        catch (System.Exception e)
        {
            throw e;
        }
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
        if (balanceBoard != null)
        {
            balanceBoard.GetUpdate();
            moveVector.x = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight * Speed;
        }
        else
        {
            moveVector.x = Input.GetAxisRaw("Horizontal") * Speed;
        }
        moveVector.y = vertVelo;
        moveVector.z = Speed;
        controller.Move(moveVector * Time.deltaTime);
    }
}
