using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAggroState : StateMachineBehaviour
{
    public AudioClip[] angrySounds;
    private AudioSource angryAudioSource;

    public AudioClip[] footstepSounds;
    private AudioSource footstepAudioSource;

    public float angrySoundScale = 1.0f;
    public float footstepSoundScale = 1.0f;


    private float footstepTimer;
    public float footstepInterval = 0.5f;  // Time interval between footstep sounds

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the two audio sources for the zombie
        AudioSource[] audioSources = animator.GetComponents<AudioSource>();

        // Assume the first AudioSource is for angry sounds, and the second is for footsteps
        angryAudioSource = audioSources[0];
        footstepAudioSource = audioSources[1];

        if (angryAudioSource == null)
        {
            Debug.Log("ZombieAggroState: Cannot get angryAudioSource.");
        }

        if (footstepAudioSource == null)
        {
            Debug.Log("ZombieAggroState: Cannot get footstepAudioSource.");
        }

        if (footstepSounds.Length == 0)
        {
            Debug.Log("ZombieAggroState: Footstep sound list is empty.");
        }

        if (angrySounds.Length == 0)
        {
            Debug.Log("ZombieAggroState: Angry sound list is empty.");
        }

        // Play a random angry sound
        if (angryAudioSource.isPlaying)
        {
            angryAudioSource.Stop();
        }
        AudioClip randomClip = angrySounds[Random.Range(0, angrySounds.Length)];
        angryAudioSource.PlayOneShot(randomClip, angrySoundScale);

        footstepTimer = 0f;  // Initialize the footstep timer
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Handle footstep sounds with a timer
        footstepTimer += Time.deltaTime;

        if (footstepTimer >= footstepInterval && !footstepAudioSource.isPlaying)
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
            AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            footstepAudioSource.PlayOneShot(footstepClip, footstepSoundScale);
            footstepTimer = 0f;  // Reset the timer after playing a footstep
        }

        // Play a random angry sound
        if (!angryAudioSource.isPlaying)
        {
            AudioClip randomClip = angrySounds[Random.Range(0, angrySounds.Length)];
            angryAudioSource.PlayOneShot(randomClip, angrySoundScale);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (angryAudioSource.isPlaying)
        {
            angryAudioSource.Stop();
        }

        if (footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
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
