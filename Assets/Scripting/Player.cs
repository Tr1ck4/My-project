using UnityEngine;
using TMPro;

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

    void Update()
    {
        mag_text.text = Mag.ToString();
        ammo_text.text = Ammo.ToString();
        ClickShoot();
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
            }
            else
            {
                Reload();
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

    void Reload()
    {
        if (Ammo > 0)
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
            nextShootTime += 2.0f;//change into an IEnumerator
        }
    }
}
