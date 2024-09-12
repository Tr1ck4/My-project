using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantZombie : Object
{

    public float aggroRange = 20f;
    public float meleeRange = 2f;

    public Player player;
    public GameObject playerGO;
    public Animator animator;

    public float swipingCooldown = 3f;  // Low cooldown for Swiping attack
    public float jumpAttackCooldown = 10f;  // Higher cooldown for Jump Attack
    public float punchAttackDamage = 20f;  // Damage for Punch attack
    public float swipeAttackDamage = 30f;  // Damage for Swiping attack
    public float jumpAttackDamage = 50f;  // Damage for Jump Attack

    private bool isAttacking = false;
    private bool isAggro = false;
    private bool isPunching = false;
    private bool isSwiping = false;
    private bool isJumpAttacking = false;

    private float lastSwipeTime = 0f;
    private float lastJumpAttackTime = 0f;

    void Start()
    {
        animator.SetBool("isIdle", true);

        // Find the player GameObject by tag
        if (playerGO == null)
        {
            playerGO = GameObject.FindWithTag("Player");  // Find player GameObject
        }

        // Get the Player script attached to the player GameObject
        if (playerGO != null)
        {
            player = playerGO.GetComponent<Player>();
        }

        if (player == null || playerGO == null)
        {
            Debug.Log("Mutant cannot find the Player");
        }
    }

    void Update()
    {
        Move();
    }

    public override void Move()
    {
        if (animator.GetBool("isDead")) return;

        Vector3 distance = player.transform.position - transform.position;

        if (distance.magnitude > aggroRange)
        {
            // Mutant is idle when far from the player
            isAggro = false;
            animator.SetBool("isIdle", true);
        }
        else if (distance.magnitude > meleeRange && distance.magnitude <= aggroRange)
        {
            Debug.Log("mutant chasing");
            // Mutant spots player and roars, then runs toward the player
            if (!isAggro)
            {
                isAggro = true;
                MutantRoar();
            }
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttacking", false);

            // Wait for Mutant finishes roaring
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Roaring") && stateInfo.normalizedTime < 1.0f)
            {
                // Roar is still playing, so return and don't move yet
                return;
            }

            // Roar finished,start chasing Player
            animator.SetBool("isRunning", true);

            Vector3 direction = distance.normalized;
            direction.y = 0;
            transform.position += direction * Speed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * Speed);
            }
        }
        else if (distance.magnitude <= meleeRange) // in range of melee attacks (punching, swiping)
        {
            Debug.Log("mutant attacking");
            isAttacking = true;
            animator.SetBool("isAttacking", true);

            // Handle Jump Attack cooldown
            if (Time.time - lastJumpAttackTime > jumpAttackCooldown)
            {
                StartCoroutine(JumpAttackRoutine());
                lastJumpAttackTime = Time.time;
            }
            // Handle Swiping cooldown
            else if (Time.time - lastSwipeTime > swipingCooldown)
            {
                StartCoroutine(SwipeAttackRoutine());
                lastSwipeTime = Time.time;
            }
            // Default to Punch attack
            else if (!isPunching)
            {
                StartCoroutine(PunchAttackRoutine());
            }
        }
    }

    // Roar
    void MutantRoar()
    {
        animator.Play("Roaring");
    }

    // Punch Attack (default move)
    IEnumerator PunchAttackRoutine()
    {
        isPunching = true;
        animator.SetBool("isPunching", true);
        yield return new WaitForSeconds(1f); // Punch animation delay
        if (Vector3.Distance(player.transform.position, transform.position) <= 2f)
        {
            player.TakeDamage(punchAttackDamage);
        }
        animator.SetBool("isPunching", false);
        isPunching = false;
    }

    // Swipe Attack (secondary move)
    IEnumerator SwipeAttackRoutine()
    {
        isSwiping = true;
        animator.SetBool("isSwiping", true);
        yield return new WaitForSeconds(1f); // Swipe animation delay
        if (Vector3.Distance(player.transform.position, transform.position) <= 2f)
        {
            player.TakeDamage(swipeAttackDamage);
        }
        animator.SetBool("isSwiping", false);
        isSwiping = false;
    }

    // Jump Attack (ultimate move)
    IEnumerator JumpAttackRoutine()
    {
        isJumpAttacking = true;
        animator.SetBool("isJumpAttacking", true);
        yield return new WaitForSeconds(1.5f); // Jump attack animation delay
        if (Vector3.Distance(player.transform.position, transform.position) <= 2f)
        {
            player.TakeDamage(jumpAttackDamage);
        }
        animator.SetBool("isJumpAttacking", false);
        isJumpAttacking = false;
    }

    // Death logic
    IEnumerator DieRoutine()
    {
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(6f);  // Dying animation time
        Destroy(gameObject);
    }

    public override void Die()
    {
        StartCoroutine(DieRoutine());
    }
}

