using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeathState : StateMachineBehaviour
{
    public AudioClip[] deathSounds;
    private AudioSource audioSource;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.Log("ZombieDeathState: Cannot get AudioSource.");
        }

        if (deathSounds.Length == 0)
        {
            Debug.Log("ZombieDeathState: death sound list is empty.");
        }

        // Play a random death sound
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        AudioClip randomClip = deathSounds[Random.Range(0, deathSounds.Length)];
        audioSource.clip = randomClip;
        audioSource.Play();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (audioSource.isPlaying)
    //    {
    //        audioSource.Stop();
    //    }
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
