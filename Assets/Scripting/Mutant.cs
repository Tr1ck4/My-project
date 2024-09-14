using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MutantZombie : Object
{

    public float aggroRange = 20f;
    public float meleeRange = 2f;

    public Player player;
    public GameObject playerGO;
    public Animator animator;

    public float punchAttackDamage = 20f;  // Damage for Punch attack
    public float swipeAttackDamage = 30f;  // Damage for Swiping attack
    public float jumpAttackDamage = 50f;  // Damage for Jump Attack

    private bool isAttacking = false;
    private bool isAggro = false;

    public AudioClip deathSound; // Drag your death sound here in the Inspector
    private AudioSource audioSource;
    public float deathSoundVolumeScale = 1.0f;


    void Start()
    {
        animator.SetBool("isIdle", true);

        audioSource = GetComponent<AudioSource>();  // Get AudioSource component

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

            // Wait for Mutant finishes
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (
                (
                    stateInfo.IsName("Roaring") ||
                    stateInfo.IsName("Punch") ||
                    stateInfo.IsName("Swiping") ||
                    stateInfo.IsName("Jump Attack") ||
                    stateInfo.IsName("Attacking Stance")
                )
                && stateInfo.normalizedTime < 1.0f)
            {
                Debug.Log("mutant current anim not finished " + stateInfo.ToString() );
                return;
            }

            // Roar finished,start chasing Player
            animator.SetBool("isIdle", false);
            animator.SetBool("inAttackingStance", false);
            animator.SetBool("isRunning", true);

            Vector3 direction = distance.normalized;
            direction.y = 0;
            transform.position += direction * Speed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * Speed);
            }
        }
        else if (distance.magnitude <= meleeRange) // in range of attacks
        {
            animator.SetBool("inAttackingStance", true);
            animator.SetBool("isRunning", false);

            // Wait for Mutant finishes
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (
                (
                    stateInfo.IsName("Roaring") ||
                    stateInfo.IsName("Punch") ||
                    stateInfo.IsName("Swiping") ||
                    stateInfo.IsName("Jump Attack")
                )
                && stateInfo.normalizedTime < 1.0f)
            {
                return;
            }

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
        if (isAttacking) return;
        // Get a random attack type
        AttackType attackType = (AttackType)Random.Range(0, 3);  // Randomly select an attack (0 = Jump, 1 = Swipe, 2 = Punch)

        switch (attackType)
        {
            case AttackType.JumpAttack:
                Debug.Log("Random attack: " +  attackType);
                StartCoroutine(JumpAttackRoutine());
                break;

            case AttackType.SwipeAttack:
                Debug.Log("Random attack: " + attackType);
                StartCoroutine(SwipeAttackRoutine());
                break;

            case AttackType.PunchAttack:
                Debug.Log("Random attack: " + attackType);
                StartCoroutine(PunchAttackRoutine());
                break;
        }
    }

    // Punch Attack (default move)
    IEnumerator PunchAttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isPunching", true);

        yield return new WaitForSeconds(1f);

        if (Vector3.Distance(player.transform.position, transform.position) <= 4f)
        {
            player.TakeDamage(punchAttackDamage);
        }
        animator.SetBool("isPunching", false);
        isAttacking = false;
    }

    // Swipe Attack (secondary move)
    IEnumerator SwipeAttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isSwiping", true);


        yield return new WaitForSeconds(2f);

        if (Vector3.Distance(player.transform.position, transform.position) <= 4f)
        {
            player.TakeDamage(swipeAttackDamage);
        }
        animator.SetBool("isSwiping", false);
        isAttacking = false;
    }

    // Jump Attack (ultimate move)
    IEnumerator JumpAttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("isJumpAttacking", true);

        // Wait for the point in the animation where the jump starts
        yield return new WaitForSeconds(0.6f);  // Adjust this based on the animation timing

        // Define jump parameters
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.transform.position;  // Target position (player's position)

        // Calculate jump distance and height
        float jumpDistance = Vector3.Distance(startPosition, new Vector3(targetPosition.x, startPosition.y, targetPosition.z));
        float jumpHeight = 3f;  // Height of the jump (adjust as needed)
        float jumpDuration = 1f;  // Duration of the jump

        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float progress = elapsedTime / jumpDuration;

            // Calculate the current position
            Vector3 forwardPosition = Vector3.Lerp(startPosition, new Vector3(targetPosition.x, startPosition.y, targetPosition.z), progress);
            float heightOffset = Mathf.Sin(Mathf.PI * progress) * jumpHeight;

            // Set the new position
            transform.position = new Vector3(forwardPosition.x, startPosition.y + heightOffset, forwardPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        yield return new WaitForSeconds(0.2f);

        // Ensure the final position is exactly at the target position
        transform.position = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);

        if (Vector3.Distance(player.transform.position, transform.position) <= 6f)
        {
            player.TakeDamage(jumpAttackDamage);
        }
        animator.SetBool("isJumpAttacking", false);
        isAttacking = false;
    }


    // Death logic
    IEnumerator DieRoutine()
    {
        animator.SetBool("isDead", true);
        // Play the death sound if the audio source is available
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, deathSoundVolumeScale);
        }
        yield return new WaitForSeconds(6f);  // Dying animation time
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }

    }

    public override void Die()
    {
        StartCoroutine(DieRoutine());
    }
}

