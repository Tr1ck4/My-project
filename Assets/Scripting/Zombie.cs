using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Object
{
    public Player player;
    public GameObject playerGO;
    public bool isTargetable = true;
    public Animator animator;

    private bool isAttacking = false;

    // Array or List of blood splatter prefabs
    public GameObject[] bloodSplatterPrefabs;

    // Time before the blood splatter disappears
    public float splatterLifetime = 5f;

    void Start(){
        animator.SetBool("isWalking", false);
        animator.SetBool("isNear", false);
        animator.SetBool("isDead", false);

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
    }
    void Update()
    {
        Move();
    }

    public override void Move()
    {
        Vector3 distance = player.transform.position - transform.position;
        if (distance.magnitude > 2f && animator.GetBool("isDead") == false)
        {
            Vector3 direction = distance.normalized;
            direction.y = 0;
            transform.position += direction * Speed * Time.deltaTime;

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

    public virtual void OnShot(Vector3 hitPosition, Quaternion hitRotation)
    {
        // Select a random blood splatter decal
        GameObject bloodSplatter = bloodSplatterPrefabs[Random.Range(0, bloodSplatterPrefabs.Length)];

        // Instantiate the blood splatter at the hit position
        GameObject instantiatedSplatter = Instantiate(bloodSplatter, hitPosition, hitRotation);

        // Start coroutine to destroy the splatter after the set lifetime
        StartCoroutine(RemoveSplatterAfterTime(instantiatedSplatter, splatterLifetime));
    }

    // Coroutine to destroy the blood splatter after a set time
    IEnumerator RemoveSplatterAfterTime(GameObject splatter, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(splatter);
    }

    IEnumerator DieRoutine()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isNear", false);
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(6f);
        Destroy(transform.gameObject);
    }

    public void ZombieDie()
    {
        StartCoroutine(DieRoutine());
    }
}
