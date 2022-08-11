using UnityEngine;
using Photon.Bolt;

public class NetworkRigidbody : EntityBehaviour<INewPhysicState>
{
    private Vector3 _moveVelocity;
    private PlayerMotor _playermotor;
    private Rigidbody _rb;
    [SerializeField]
    private float _gravityForce = 1f;
    private bool _useGravity = true;
    public float rotation = 0;
    private CapsuleCollider _collider;

    public Vector3 MoveVelocity
    {
        set
        {
            if (entity.IsControllerOrOwner)
            {
                _moveVelocity = value;
            }
        }

        get
        {
            return _moveVelocity;
        }
    }

    public float GravityForce
    {
        get
        {
            return Physics.gravity.y * _gravityForce * BoltNetwork.FrameDeltaTime;
        }
    }

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _playermotor = GetComponent<PlayerMotor>();
    }

    private void OnEnable()
    {
        _rb.isKinematic = false;
    }
    private void OnDisable()
    {
        _rb.isKinematic = true;
    }
    public override void Attached()
    {
        if (entity.IsControllerOrOwner)
        {
            if (entity.IsOwner)
                state.SetTransforms(state.newTransform, transform);
            else
            {
                Destroy(_rb);
                _collider.isTrigger = false;
            }
        }
        else
        {
            state.SetTransforms(state.newTransform, transform);
            _collider.isTrigger = false;
        }
    }

    public void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if(entity.IsOwner)
            {

                float g = _moveVelocity.y;

                if (_useGravity)
                {
                    if (_moveVelocity.y < 0f)
                        g += 1.5f * GravityForce;
                    else if (_moveVelocity.y > 0f)
                        g += 1f * GravityForce;
                    else
                        g = _rb.velocity.y;
                }

                _moveVelocity = new Vector3(_moveVelocity.x, g, _moveVelocity.z);
                if (entity.IsOwner)
                {
                    _rb.velocity = _moveVelocity;
                    transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
                }
            }
        }
    }
}
