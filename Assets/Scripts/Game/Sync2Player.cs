using System.Collections;
using UnityEngine;

public class Sync2Player : MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Vector3 movement;
    private Vector3 buffer;
    private Player player;

    // Use this for initialization
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        buffer = new Vector3(0, 0, 0);
        player = gameObject.GetComponent<Player>();
        if (player != null)
        {
            buffer = player.getMovement();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            buffer = player.getMovement();
        }
        movement = buffer;
    }

    void FixedUpdate()
    {
        // Move the game object
        rigidbodyComponent.velocity = movement;
    }
}


