using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using Profiles;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;
    private StringBuilder positionBuilder;
    private StringBuilder movementBuilder;
    private Collider playerCollider;
    private Animator animator;

    private Thread updateThread;

    //artificial gravity
    private float vertVelo = 0.5f;
    private float gravity = 12.0f;
    private float nextActionTime = 0.0f;
    private float period = 0.1f;
    private float minthreshold = 0.0f;
    private float maxthreshold = 0.0f;

    public float Speed = 3.0f;

    // Use this for initialization
    private void Start()
    {
        this.positionBuilder = new StringBuilder();
        this.positionBuilder.AppendLine("x;y;");

        this.movementBuilder = new StringBuilder();
        this.positionBuilder.AppendLine("TopLeft;TopRight;BottomLeft;BottomRight;");

        controller = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        float thresholdX = (BalanceBoardManager.Current.XMax - BalanceBoardManager.Current.XMin) / 2.5f;
        this.maxthreshold = (BalanceBoardManager.Current.XMax + thresholdX);
        this.minthreshold = (BalanceBoardManager.Current.XMax - thresholdX);

        this.updateThread = new Thread(UpdateBalanceBoardMovement);
        this.updateThread.Start();
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameUI.Instance.IsPause)
        {
            //if (animator != null && animator.enabled)
            //{
            //    animator.enabled = false;
            //}
            return;
        }
        //else
        //{
        //    if (animator != null && !animator.enabled)
        //    {
        //        animator.enabled = true;
        //    }
        //}

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
            float centerOfGravityX = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.X;
            //Debug.Log(centerOfGravityX);
            if (centerOfGravityX > this.maxthreshold)
            {
                moveVector.x = -1 * Speed;
            }
            else if (centerOfGravityX < this.minthreshold)
            {
                moveVector.x = 1 * Speed;
            }
        }
        else
        {
            moveVector.x = Input.GetAxisRaw("Horizontal") * Speed;
        }

        moveVector.y = vertVelo;
        moveVector.z = Speed;
        controller.Move(moveVector * Time.deltaTime);

        if (playerCollider != null && Time.time > nextActionTime)
        {
            nextActionTime += period;
            AppendPositionLine(playerCollider.bounds);
        }
    }

    private void AppendPositionLine(Bounds bounds)
    {
        this.positionBuilder.AppendLine(string.Format("{0};{1};", bounds.center.x, bounds.center.z));
    }

    private void OnDestroy()
    {
        if (this.updateThread != null)
        {
            this.updateThread.Abort();
            this.updateThread = null;
        }

        if (this.positionBuilder != null && this.positionBuilder.Length > 0)
        {
            ProfileManager.Current.WritePlayerPositionFile(this.positionBuilder.ToString());
            this.positionBuilder = null;
        }

        if (this.movementBuilder != null && this.movementBuilder.Length > 0)
        {
            ProfileManager.Current.WriteMovementFile(this.movementBuilder.ToString());
            this.movementBuilder = null;
        }
    }

    private void UpdateBalanceBoardMovement()
    {
        while (true)
        {
            if (BalanceBoardManager.Current.IsBalanceBoardConnected)
            {
                BalanceBoardManager.Current.BalanceBoard.GetUpdate();
                this.movementBuilder.AppendLine(string.Format("{0};{1};{2};{3};",
                    BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesRaw.TopLeft,
                    BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesRaw.TopRight,
                    BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesRaw.BottomLeft,
                    BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesRaw.BottomRight));
            }
            Thread.Sleep(20);
        }
    }
}
