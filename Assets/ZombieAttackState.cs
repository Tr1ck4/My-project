using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackState : StateMachineBehaviour
{
    public AudioClip[] attackSounds;
    private AudioSource attackAudioSource;

    public AudioClip[] impactSounds;
    private AudioSource impactAudioSource;

    public float attackSoundScale = 1.0f;
    public float impactSoundScale = 1.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioSource[] audioSources = animator.GetComponents<AudioSource>();

        attackAudioSource = audioSources[0];
        impactAudioSource = audioSources[1];

        if (attackAudioSource == null)
        {
            Debug.Log("ZombieAttackState: Cannot get attackAudioSource.");
        }
        if (impactAudioSource == null)
        {
            Debug.Log("ZombieAttackState: Cannot get impactAudioSource.");
        }

        if (attackSounds.Length == 0)
        {
            Debug.Log("ZombieAttackState: Attack sound list is empty.");
        }
        if (impactSounds.Length == 0)
        {
            Debug.Log("ZombieAttackState: Impact sound list is empty.");
        }

        // Play a random attack sound
        if (attackAudioSource.isPlaying)
        {
            attackAudioSource.Stop();
        }

        AudioClip randomClip = attackSounds[Random.Range(0, attackSounds.Length)];
        attackAudioSource.PlayOneShot(randomClip, attackSoundScale);

        AudioClip randomImpactClip = impactSounds[Random.Range(0, impactSounds.Length)];
        impactAudioSource.PlayOneShot(randomImpactClip, impactSoundScale);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Play a random attack sound
        if (!attackAudioSource.isPlaying)
        {
            AudioClip randomClip = attackSounds[Random.Range(0, attackSounds.Length)];
            attackAudioSource.PlayOneShot(randomClip, attackSoundScale);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop the sound when the state exits
        if (attackAudioSource.isPlaying)
        {
            attackAudioSource.Stop();
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
