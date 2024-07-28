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
    
    public virtual void Move(){
        Debug.Log("Object moving");
    }
    private void PushBack(float damage, Vector3 source, float resist){
        if(gameObject.activeSelf){
            Health -= damage;
            Vector3 angle = source - transform.position;
            angle*= (-resist * Resistance);
            transform.position += angle;
        }
    }

    void TakeDamage(Object source){
       this.Health -= (source.Damage * (1- this.Ammor));
       PushBack(source.Damage, source.transform.position, source.Resist);
       if (this.Health < 0){
            Destroy(this.gameObject);
       }
    }
}
