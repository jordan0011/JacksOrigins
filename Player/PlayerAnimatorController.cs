using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class PlayerAnimatorController : EntityBehaviour<INewPlayerState>
{
    [SerializeField]
    private Animator playerAnim;

    // Update is called once per frame
    void Update()
    {
        playerAnim.SetFloat("RunningSpeed", state.AnimSpeed);
        playerAnim.SetInteger("AttackOrder", state.AnimAttack);
        playerAnim.SetBool("Jump", state.AnimJump);
    }
}
