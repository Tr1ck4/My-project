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
    private CharacterController m_CharacterController;
    
    public virtual void Move(){
        Debug.Log("Object moving");
    }
    public void PushBack(float damage, Vector3 source, float resist)
    {
        if (gameObject.activeSelf)
        {
            // Calculate the direction and magnitude of the pushback force
            Vector3 angle = transform.position - source; // Reverse the direction for pushback
            angle = angle.normalized * resist * (1 - Resistance); // Adjust for resistance

            // Apply the pushback force using Rigidbody
            body.MovePosition(body.transform.position + angle);
        }
    }

    public void TakeDamage(Object source){
       this.Health -= (source.Damage * (1- this.Ammor));
       PushBack(source.Damage, source.transform.position, source.Resist);
       if (this.Health < 0){
            Destroy(this.gameObject);
       }
    }
}
