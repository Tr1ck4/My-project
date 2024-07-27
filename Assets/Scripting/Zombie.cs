using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Object
{
    public Player player;
    public bool isTargetable = true;
    public Rigidbody body;
    

    void Move(){
        if (player){
            Vector3 pos = (player.transform.position - body.transform.position) * this.Speed;
            body.MovePosition(pos);
        }
    }
    //public virtual void RandomAttack(){};
}
