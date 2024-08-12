using UnityEngine;
using TMPro;
using System.Collections;

public class Player : Object
{
    public int Mag;
    public int maxMag;
    public int Ammo;
    public float ShootSpeed = 2f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    private float nextShootTime;
    public TMP_Text mag_text;
    public TMP_Text ammo_text;

    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null )
        {
            Debug.Log("cannot get animator");
        }
    }

    void Update()
    {
        mag_text.text = Mag.ToString();
        ammo_text.text = Ammo.ToString();
        ClickShoot();

        // Update isShooting parameter based on input state
        if (Input.GetKey(KeyCode.Mouse0) && Mag > 0)
        {
            animator.SetBool("firing_rifle", true);
        }
        else
        {
            animator.SetBool("firing_rifle", false);
        }

        if (Input.GetKeyDown(KeyCode.R)) // R to reload
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if (Ammo > 0 && Mag < maxMag)
        {
            animator.SetBool("reloading_rifle", true);
        }
        
        // Wait until the current animation is finished
        yield return new WaitForSeconds(GetCurrentAnimationLength());
        if (Ammo > 0 && Mag < maxMag)
        {
            int ammoNeeded = maxMag - Mag;
            
            if (Ammo >= ammoNeeded)
            {
                Mag = maxMag;
                Ammo -= ammoNeeded;
            }
            else
            {
                Mag += Ammo;
                Ammo = 0;
            }
            nextShootTime += 2.0f;
        }
        animator.SetBool("reloading_rifle", false);
    }

    float GetCurrentAnimationLength()
    {
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animStateInfo.length;
    }

    public void ClickShoot()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0)) && Time.time >= nextShootTime)
        {
            if (Mag > 0)
            {
                nextShootTime = Time.time + ShootSpeed;
                ShootProjectile();
                Mag--;

                animator.SetTrigger("firing_rifle");
            }
            else
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    void ShootProjectile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000); 
        }

        Vector3 direction = (targetPoint - Camera.main.transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, Camera.main.transform.position, Quaternion.LookRotation(direction));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = direction * bulletSpeed;
        
    }
}
