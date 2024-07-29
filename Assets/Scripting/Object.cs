using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public float Health;
    public float Speed;
    public float Ammor;
    public float Damage;
    public float Resistance;
    public float Resist;
    public Rigidbody body;
    
    public virtual void Move(){
        Debug.Log("Object moving");
    }
    public void PushBack(float damage, Vector3 source, float resist)
    {
        if (gameObject.activeSelf)
        {
            Vector3 angle = transform.position - source;
            angle = angle.normalized * resist * (1 - Resistance);

            body.MovePosition(body.transform.position + angle);
        }
    }

    public void TakeDamage(float damage, Vector3 position, float resist){
       this.Health -= (damage * (1- this.Ammor));
       PushBack(damage, position, resist);
       if (this.Health < 0){
            Destroy(this.gameObject);
       }
    }
}
