using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class PlayerEffects : EntityBehaviour<IPlayerState>
{
    ArrayList ActiveEffects = new ArrayList();

    [SerializeField]
    private GameObject EffectsParent;
    [SerializeField]
    private GameObject VFXParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class PlayerEffect
{
    private GameObject abilityEffect = null;
    private int effectId = -1;
    private GameObject Effectvfx = null;

    public PlayerEffect(int myid, GameObject myAbEffect, GameObject myVFX)
    {
        effectId = myid;
        abilityEffect = myAbEffect;
        Effectvfx = myVFX;
    }
    public int GetID()
    {
        return effectId;
    }
    public GameObject GetEffect()
    {
        return abilityEffect;
    }
    public GameObject GetVFX()
    {
        return Effectvfx;
    }
}
