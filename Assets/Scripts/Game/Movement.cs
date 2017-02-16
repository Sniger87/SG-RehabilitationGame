using System.Collections;
using System.Linq;
using System.Text;
using Profiles;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;
    private StringBuilder stringBuilder;
    private Collider playerCollider;

    //artificial gravity
    private float vertVelo = 0.5f;
    private float gravity = 12.0f;
    private float nextActionTime = 0.0f;
    private float period = 0.1f;

    public float Speed = 3.0f;

    // Use this for initialization
    private void Start()
    {
        this.stringBuilder = new StringBuilder();
        this.stringBuilder.AppendLine("x;y;");

        controller = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update()
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
            //BalanceBoardManager.Current.BalanceBoard.GetUpdate();
            moveVector.x = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight * Speed;
        }

        moveVector.x = Input.GetAxisRaw("Horizontal") * Speed;

        moveVector.y = vertVelo;
        moveVector.z = Speed;
        controller.Move(moveVector * Time.deltaTime);

        if (playerCollider != null && Time.time > nextActionTime)
        {
            nextActionTime += period;
            AppendMovementLine(playerCollider.bounds);
        }
    }

    private void AppendMovementLine(Bounds bounds)
    {
        this.stringBuilder.AppendLine(string.Format("{0};{1}", bounds.center.x, bounds.center.z));
    }

    private void OnDestroy()
    {
        if (this.stringBuilder != null)
        {
            ProfileManager.Current.WritePlayerPositionFile(this.stringBuilder.ToString());
            this.stringBuilder = null;
        }
    }
}
