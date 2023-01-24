using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StarterManager : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void StartSequence()
    {
        animator.SetTrigger("LaunchAnim");
        PlayerInput[] playersInputs = FindObjectsOfType<PlayerInput>();
        foreach(PlayerInput playerInput in playersInputs)
        {
            playerInput.DeactivateInput();
        }
        SoundManager.sInstance.PlaySound("StartCountdown");
    }

    public void EndSequence()
    {
        PlayerInput[] playersInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput playerInput in playersInputs)
        {
            playerInput.ActivateInput();
        }
    }
}