using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;
    Vector3 velocity;

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, 0.55f, groundMask))
        {
            return true;
        }
        return false;
    }


    void Gravity()
    {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -0.5f;

        transform.position += velocity * Time.deltaTime;
    }

}
