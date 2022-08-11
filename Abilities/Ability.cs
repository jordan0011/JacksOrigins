using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class Ability : EntityEventListener<INewPlayerState>
{
    protected bool _pressed = false;
    protected bool _buttonUp;
    protected bool _buttonDown;

    protected float _cooldown = 0;
    protected float _timer = 0f;
    protected int _cost = 0;
    protected UI_Cooldown _UI_Cooldown;
    protected Vector3 direction = Vector3.zero;
    protected PlayerMotor player;

    protected float _abilityInterval
    {
        get { return _cooldown * BoltNetwork.FramesPerSecond; }
    }

    public virtual void UpdateAbility(bool button)
    {
        _buttonUp = false;
        _buttonDown = false;
        if (button)
        {
            _pressed = true;
            _buttonDown = true;
        }
        else
        {
            if(_pressed)
            {
                _pressed = false;
                _buttonUp = true;
            }
        }
    }

    public virtual void ShowVisualEffect()
    {

    }
    public virtual void GetDirection(Vector3 mydirection, PlayerMotor myplayer)
    {
        direction = mydirection;
        player = myplayer;
    }
}
