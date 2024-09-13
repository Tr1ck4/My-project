using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : StateMachineBehaviour
{
    public AudioClip[] idleSounds;
    private AudioSource audioSource;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.GetComponent<AudioSource>();
        if (audioSource == null )
        {
            Debug.Log("ZombieIdleState: Cannot get AudioSource.");
        }

        if (idleSounds.Length == 0)
        {
            Debug.Log("ZombieIdleState: Idle sound list is empty.");
        }

        // Play a random idle sound
        AudioClip randomClip = idleSounds[Random.Range(0, idleSounds.Length)];
        audioSource.PlayOneShot(randomClip, 0.25f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Play a random idle sound
        if (!audioSource.isPlaying)
        {
            AudioClip randomClip = idleSounds[Random.Range(0, idleSounds.Length)];
            audioSource.PlayOneShot(randomClip, 0.25f);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop the sound when the state exits
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

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
