using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MutantZombie : Object
{

    public float aggroRange = 20f;
    public float meleeRange = 4f;

    public Player player;
    public GameObject playerGO;
    public Animator animator;

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
            animator.SetBool("inAttackingStance", false);

            // Wait for Mutant finishes roaring
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (
                (
                    stateInfo.IsName("Roaring") ||
                    stateInfo.IsName("isPunching") ||
                    stateInfo.IsName("isSwiping") ||
                    stateInfo.IsName("isJumpAttacking")
                )
                && stateInfo.normalizedTime < 1.0f)
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
            animator.SetBool("inAttackingStance", true);

            if (!isAttacking)
            {
                PerformRandomAttack();
            }
        }
    }

    // Roar
    void MutantRoar()
    {
        animator.Play("Roaring");
    }

    // Attacks Handling
    enum AttackType { JumpAttack, SwipeAttack, PunchAttack }
    void PerformRandomAttack()
    {
        // Get a random attack type
        AttackType attackType = (AttackType)Random.Range(0, 3);  // Randomly select an attack (0 = Jump, 1 = Swipe, 2 = Punch)

        switch (attackType)
        {
            case AttackType.JumpAttack:
                Debug.Log("Random attack: " +  attackType);
                if (!isJumpAttacking)  // Optional check to ensure the mutant is not already jumping
                {
                    StartCoroutine(JumpAttackRoutine());
                }
                break;

            case AttackType.SwipeAttack:
                Debug.Log("Random attack: " + attackType);
                if (!isSwiping)  // Optional check to ensure the mutant is not already swiping
                {
                    StartCoroutine(SwipeAttackRoutine());
                }
                break;

            case AttackType.PunchAttack:
                Debug.Log("Random attack: " + attackType);
                if (!isPunching)  // Optional check to ensure the mutant is not already punching
                {
                    StartCoroutine(PunchAttackRoutine());
                }
                break;
        }
    }

    // Punch Attack (default move)
    IEnumerator PunchAttackRoutine()
    {
        isAttacking = true;
        isPunching = true;
        animator.SetBool("isPunching", true);

        yield return new WaitForSeconds(1f);

        if (Vector3.Distance(player.transform.position, transform.position) <= 4f)
        {
            player.TakeDamage(punchAttackDamage);
        }
        animator.SetBool("isPunching", false);
        isPunching = false;
        isAttacking = false;
    }

    // Swipe Attack (secondary move)
    IEnumerator SwipeAttackRoutine()
    {
        isAttacking = true;
        isSwiping = true;
        animator.SetBool("isSwiping", true);


        yield return new WaitForSeconds(2f);

        if (Vector3.Distance(player.transform.position, transform.position) <= 4f)
        {
            player.TakeDamage(swipeAttackDamage);
        }
        animator.SetBool("isSwiping", false);
        isSwiping = false;
        isAttacking = false;
    }

    // Jump Attack (ultimate move)
    IEnumerator JumpAttackRoutine()
    {
        isAttacking = true;
        isJumpAttacking = true;
        animator.SetBool("isJumpAttacking", true);

        yield return new WaitForSeconds(3f);

        if (Vector3.Distance(player.transform.position, transform.position) <= 6f)
        {
            player.TakeDamage(jumpAttackDamage);
        }
        animator.SetBool("isJumpAttacking", false);
        isJumpAttacking = false;
        isAttacking = false;
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

