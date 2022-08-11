using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability
{
    private NetworkRigidbody _networkRigidbody = null;
    [SerializeField]
    private Transform _cam = null;
    private float _dashForce = 20f;
    private float _dashDuration = 0.25f;
    private bool _dashing = false;
    [SerializeField]
    private bool dashpressed = false;

    public void Awake()
    {
        _cooldown = 0.5f;
        _networkRigidbody = GetComponent<NetworkRigidbody>();
        _UI_Cooldown = GUI_Controller.Current.Skill;
        _UI_Cooldown.InitView(_abilityInterval);
        _cost = 0;
    }
    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);

        if (_buttonUp)
        {
            dashpressed = true;
        }
        if(dashpressed  && _dashing)
        {
            _dashing = false;
            dashpressed = false;

            player.ResetQueue(5);
        }

        if(_buttonDown && _timer + _abilityInterval <= Photon.Bolt.BoltNetwork.ServerFrame)
        {
            _timer = Photon.Bolt.BoltNetwork.ServerFrame;
            if (entity.HasControl)
              _UI_Cooldown.StartCooldown();
            _Dash();
            if(!dashpressed)
                player.QueueAnimation(5); //Rolling animation
        }
        

        if (_dashing)
        {
            if(direction.magnitude > 0.1f)
                _networkRigidbody.MoveVelocity = Vector3.Scale(direction, new Vector3(1, 0, 1)).normalized * _dashForce;
            else
                _networkRigidbody.MoveVelocity = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized * _dashForce;
        }
        else
        {
            dashpressed = false;
        }
    }

    private void _Dash()
    {
        StartCoroutine(Dashing());
    }

    IEnumerator Dashing()
    {
        _dashing = true;
        yield return new WaitForSeconds(_dashDuration);
        _dashing = false;
        player.ResetQueue(5);
    }
}
