using System.Collections;
using System.Collections.Generic;
using Invector.vCamera;
using UnityEngine;
using Invector.vCharacterController;

public class LockCharacterControl : MonoBehaviour
{

    public vMeleeCombatInput vmeleController;
    public vThirdPersonInput vController;
    private Animator anim => GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    private vThirdPersonCamera cam => GameObject.FindGameObjectWithTag("MainCamera").GetComponent<vThirdPersonCamera>();
    void Start()
    {

        vmeleController = GameObject.FindGameObjectWithTag("Player").GetComponent<vMeleeCombatInput>();
        vController = GameObject.FindGameObjectWithTag("Player").GetComponent<vThirdPersonInput>();
    }

    public void Lock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        vmeleController.lockMeleeInput = true;
        vController.lockInput = true;
        cam.lockCamera = true;
        
        
    }

    public void unLock()
    {
        cam.lockCamera = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        vmeleController.lockMeleeInput = false;
        vController.lockInput = false;
    }
}
