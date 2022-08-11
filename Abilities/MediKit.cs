using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class MediKit : Ability
{
    [SerializeField]
    private Transform _cam = null;
    [SerializeField]
    private LayerMask _layerMask;
    private float _maxDistance = 5f;

    private void Awake()
    {
        _cooldown = 10;
        _UI_Cooldown = GUI_Controller.Current.Skill;
        _UI_Cooldown.InitView(_abilityInterval);
        _cost = 1;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);

        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame)
        {
            _timer = BoltNetwork.ServerFrame;

            if (entity.HasControl)
                _UI_Cooldown.StartCooldown();

            if (entity.IsOwner)
            {
                RaycastHit hit;
                if (Physics.Raycast(_cam.position, _cam.transform.forward, out hit, _maxDistance, _layerMask))
                {
                    BoltNetwork.Instantiate(BoltPrefabs.Medikit, hit.point, Quaternion.identity).GetComponent<AreaOfEffect>().launcher = GetComponent<PlayerMotor>();
                }
                else
                {
                    BoltNetwork.Instantiate(BoltPrefabs.Medikit, transform.position, Quaternion.identity).GetComponent<AreaOfEffect>().launcher = GetComponent<PlayerMotor>();
                }
            }
        }
    }
}
