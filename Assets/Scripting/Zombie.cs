using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Object
{
    public Player player;
    public Animator animator;
    public float RotationSpeed = 5f;

    public AudioSource audioSource;

    private bool isAttacking = false;

    protected void Start(){
        animator.SetBool("isWalking", false);
        animator.SetBool("isNear", false);
        animator.SetBool("isDead", false);
        player = GameObject.Find("Player").GetComponent<Player>();
        audioSource = gameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Zombie: Cannot find AudioSource.");
        }
    }
    void Update()
    {
        Move();
    }

    public override void Move()
    {
        Vector3 distance = player.transform.position - transform.position;
        
        if (distance.magnitude > 2f && !animator.GetBool("isDead"))
        {
            Vector3 direction = distance.normalized;
            direction.y = 0; 

            Vector3 newPosition = transform.position + direction * Speed * Time.deltaTime;
            this.body.MovePosition(newPosition);

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * Speed);
            }

            animator.SetBool("isWalking", true);
        }
        else
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    public override void OnShot(Vector3 hitPosition, Quaternion hitRotation)
    {
        base.OnShot(hitPosition, hitRotation);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        while (player != null && Vector3.Distance(player.transform.position, transform.position) <= 2f)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isNear", true);
            Attack();
            yield return new WaitForSeconds(4f);
            animator.SetBool("isNear", false);
        }

        isAttacking = false;
    }

    void Attack()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        player.TakeDamage(this.Damage);
    }

    IEnumerator DieRoutine()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isNear", false);
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(audioSource.clip.length);
        yield return new WaitForSeconds(6f);
        Destroy(transform.gameObject);
    }

    public override void Die()
    {
        StartCoroutine(DieRoutine());
    }
}
