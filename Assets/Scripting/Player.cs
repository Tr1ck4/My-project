using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Object
{
    public int Mag;
    public int maxMag;
    public int Ammo;
    public float ShootSpeed;
    
    public ProjectTile texture;

    public void ClickShoot(){
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0)){
            if (Mag == 0)
            {
                Reload();
            }else{
                //texture.SpawnTile( vector shoot);
                Invoke("ShootProjectile",ShootSpeed);
            }
        }
    }
    // void ShootProjectile(){
    //     Rigidbody instance = Instantiate(bulletPrefab);
    //     // instance.velocity = 5.0f;
    // }
    public void Reload(){
        if (Input.GetKey(KeyCode.R) && Mag != maxMag){
            //Play reload animation
            int cost = maxMag - Mag;
            Ammo -= cost;
            Mag = maxMag;
        }
        else{
            //Play sound bullet junk
        }
    }

}
