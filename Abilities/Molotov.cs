using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : Grenade
{
    [SerializeField]
    private GameObject _flameZone = null;
    public override void Detached()
    {
        base.Detached();
        Instantiate(_flameZone, transform.position, Quaternion.identity).GetComponent<AreaOfEffect>().launcher = _launcher;
    }
}
