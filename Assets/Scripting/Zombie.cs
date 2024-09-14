using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Object
{
    public Player player;
    public Animator animator;
    public float RotationSpeed = 5f;

    private bool isAttacking = false;

    public float price;
    public AudioClip deathSound; // Drag your death sound here in the Inspector
    private AudioSource audioSource;
    public float deathSoundVolumeScale = 1.0f;

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
            direction.y = 0; // Ignore vertical movement

            // Apply force in the direction of the player
            this.body.AddForce(direction * Speed, ForceMode.VelocityChange);

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
        GameObject.Find("GameController").GetComponent<GameController>().money += this.price;
        animator.SetBool("isWalking", false);
        animator.SetBool("isNear", false);
        animator.SetBool("isDead", true);
        // Play the death sound if the audio source is available
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, deathSoundVolumeScale);
        }
        yield return new WaitForSeconds(6f);
        Destroy(transform.gameObject);
    }

    public override void Die()
    {
        StartCoroutine(DieRoutine());
    }
}
