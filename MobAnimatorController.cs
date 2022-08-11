using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class MobAnimatorController : EntityBehaviour<IMobState>
{
    [SerializeField]
    private Animator myanimator;

    // Update is called once per frame

    private void Awake()
    {
        if (BoltNetwork.IsServer)
        {
            this.enabled = false;
        }
    }
    void Update()
    {
        myanimator.SetFloat("Speed", state.AnimSpeed);
        myanimator.SetInteger("AttackCode", state.AnimAtCode);
    }
}
