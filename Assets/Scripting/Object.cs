using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public float Health;
    public float Speed;
    public float Ammor;
    public float Damage;
    public Rigidbody body;

    public virtual void Move(){
        Debug.Log("Object moving");
    }
    public void TakeDamage(float amount){
        Health -= amount*(1-Ammor/100);
        if (gameObject.tag == "Zombie")
        {
            gameObject.GetComponent<Zombie>().OnShot(transform.position, transform.rotation);
        }
        //if (gameObject.tag == "Player")
        //{
        //    Debug.Log("Player HP: " + gameObject.GetComponent<Player>().Health);
        //}
        if (Health <= 0f){
            Die();
        }
    }
    public virtual void Die(){
        Destroy(gameObject);
    }
}
