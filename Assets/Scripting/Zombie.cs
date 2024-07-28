using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Object
{
    public Player player;
    public bool isTargetable = true;
    public Rigidbody body;
    public Animator animator;
    public float Speed = 3f;

    void Move()
    {
        Vector3 distance = player.transform.position - body.transform.position;
        if (distance.magnitude > 2f)
        {
            Debug.Log("Walking");
            Vector3 direction = distance.normalized;
            direction.y = 0;
            body.transform.position += direction * Speed * Time.deltaTime;

            // Rotate the zombie to face the movement direction
            if (direction != Vector3.zero)
            {
                body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * Speed);
            }

            animator.Play("walking");
        }
        else
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Attack");
        
        // Rotate the zombie to face the player
        Vector3 direction = (player.transform.position - body.transform.position).normalized;
        direction.y = 0; // Keep the rotation horizontal
        if (direction != Vector3.zero)
        {
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * Speed);
        }

        animator.Play("dance");
    }

    void Update()
    {
        Move();
    }
}
