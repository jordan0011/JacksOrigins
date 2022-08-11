using UnityEngine;
using Photon.Bolt;

public class LaunchObject : Ability
{
    [SerializeField]
    GameObject _objectPreview = null;
    [SerializeField]
    GameObject _objectToLaunch = null;
    GameObject _object = null;
    [SerializeField]
    private Transform _cam = null;

    [SerializeField]
    private float _launchForce = 2f;

    public void Awake()
    {
        _cooldown = 1;
        _UI_Cooldown = GUI_Controller.Current.Grenade;
        _UI_Cooldown.InitView(_abilityInterval);
        _cost = 0;
    }

    public override void UpdateAbility(bool button)
    {
        base.UpdateAbility(button);
        if (_buttonDown && _timer + _abilityInterval <= BoltNetwork.ServerFrame)
        {
            if (entity.HasControl)
            {
                _object = Instantiate(_objectPreview, _cam);
                _object.transform.position = _cam.transform.position + _cam.transform.forward;
            }
        }
        else if (_buttonUp && _timer + _abilityInterval <= BoltNetwork.ServerFrame)
        {
            _timer = BoltNetwork.ServerFrame;
            if (entity.HasControl)
            {
                Destroy(_object);
                _UI_Cooldown.StartCooldown();
            }
            if (entity.IsOwner)
            {
                GameObject o = BoltNetwork.Instantiate(_objectToLaunch, _cam.position + _cam.forward, _cam.rotation);
                o.GetComponent<NetworkRigidbody>().MoveVelocity = _cam.transform.forward * _launchForce;
                o.GetComponent<Grenade>().laucher = GetComponent<PlayerMotor>();
            }
        }
    }
}
