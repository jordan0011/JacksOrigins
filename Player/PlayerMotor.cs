using UnityEngine;
using Photon.Bolt;

public class PlayerMotor : EntityBehaviour<INewPlayerState>
{
    [SerializeField]
    private Camera _cam = null;
    private NetworkRigidbody _networkRigidbody = null;

    private PlayerWorldUIStats _playerWorldStats = null;
    private PowerStats _powerStats = null;

    private float turnSmoothVelocity;
    [SerializeField]
    private float turnSmoothTime = 0.03f;

    private float _speed = 7f;
    private float _speedBase = 7f;

    private Vector3 _lastServerPos = Vector3.zero;
    private bool _firstState = true;

    private bool _jumpPressed = false;
    private float _jumpForce = 9f;

    private bool _isGrounded = false;
    private float _maxAngle = 45f;

    [SerializeField]
    private int _totalLife = 250;

    //SphereCollider _headCollider;
    CapsuleCollider _capsuleCollider;

    [SerializeField]
    private Ability _skill = null;
    [SerializeField]
    private Ability _grenade = null;

    private bool _isEnemy = true;

    public int TotalLife { get => _totalLife; }
    public float Speed { get => _speed; set => _speed = value; }
    public float SpeedBase { get => _speedBase; }

    public bool IsEnemy { get => _isEnemy; }

    private bool calculateFrame = false;
    public bool hasDied = false;

    [SerializeField]
    private float distance = 0.15f;

    private const float k_MinSmoothSpeed = 4.0f;
    private const float k_TargetCatchupTime = 0.1f;

    private const float k_MaxTurnRateDegreesSecond = 280;

    private float m_SmoothedSpeed;
    private float m_SmoothedHeightSp;

    private float turnSmoothVelocity1;
    private float turnSmoothtime1 = 0.04f;

    private float rotation = 0f;
    private float lockedRotation = 0f;

    private Coroutine _immobilizeCrt = null;

    private float immobilizedTime0 = 0f;
    private bool isImmobilized = false;
    private bool senddeathinfo = true;
    [SerializeField]
    private LayerMask ground;

    [SerializeField]
    private GameObject testprefab;
    private GameObject test;

    private void Awake()
    { 
        _networkRigidbody = GetComponent<NetworkRigidbody>();
       // _headCollider = GetComponent<SphereCollider>();
        _playerWorldStats = GetComponent<PlayerWorldUIStats>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _powerStats = GetComponent<PowerStats>();

        _cam.transform.SetParent(null, true);

        if (BoltNetwork.IsServer)
        {
           // test = Instantiate(test);
        }
    }
    public void Init(bool isMine)
    {
        _totalLife = _powerStats.GetmaxHealth();
        if (isMine)
        {
            tag = "LocalPlayer";
            GUI_Controller.Current.UpdateLife(_totalLife, _totalLife);
            //GUI_Controller.Current.UpdateAmmo()
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject go in players)
            {
            //    go.GetComponent<PlayerMotor>().TeamCheck();
                go.GetComponent<PlayerRenderer>().Init();
            }

            _jumpForce *= 1.05f;

        }
        else
        {
            _cam.GetComponent<Camera>().enabled = false;
            _cam.gameObject.tag = "Untagged";
            _cam.GetComponent<AudioListener>().enabled = false;
            //Destroy(_cam.GetComponent<Camera>());
            _cam.gameObject.SetActive(false);
            //Destroy(_cam);
        }
        //TeamCheck();

        //_playerWorldStats.SetStats(state.LifePoints, TotalLife);
    }
    public void TeamCheck()
    {
        GameObject localPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        Team t = Team.AT;
        PlayerToken pt = (PlayerToken)entity.AttachToken;

        if (localPlayer)
        {
            PlayerToken lpt = (PlayerToken)localPlayer.GetComponent<PlayerMotor>().entity.AttachToken;
            t = lpt.team;
        }

        if (pt.team == t)
            _isEnemy = false;
        else
            _isEnemy = true;
    }

    public State ExecuteCommand(bool jump, Vector3 moveDirection1, bool ability1, bool ability2)
    {
        Vector3 straight = Vector3.zero;

        if (!state.IsDead) // an den eisai dead mporoun na pragmatopoiithoun entoles 
        {
            if (jump)
            {
                if (_jumpPressed == false && _isGrounded)
                {
                    _isGrounded = false;
                    _jumpPressed = true;
                    if (entity.IsOwner)
                    {
                        _networkRigidbody.MoveVelocity += Vector3.up * _jumpForce;

                    }
                }
            }
            else
            {
                if (_jumpPressed)
                    _jumpPressed = false;
            }

            if (moveDirection1.magnitude > 0.1f )
            { 
                moveDirection1.Normalize();

                rotation = Mathf.Atan2(moveDirection1.x, moveDirection1.z) * Mathf.Rad2Deg;
                straight = moveDirection1 * _speed;
            }
            if (entity.IsOwner)
            {
                if (!isImmobilized)
                {//move freely
                    if (_isGrounded)
                    {
                        _networkRigidbody.MoveVelocity = new Vector3(straight.x, _networkRigidbody.MoveVelocity.y, straight.z);
                        _networkRigidbody.rotation = rotation;
                    }
                }
                else
                {//cant move
                    _networkRigidbody.MoveVelocity = new Vector3(0, _networkRigidbody.MoveVelocity.y, 0);
                    _networkRigidbody.rotation = lockedRotation;
                    rotation = lockedRotation;
                }
            }

            if (entity.IsOwner)
            {
                float animationSpeed = (straight.magnitude / (_speedBase - 0));
                state.AnimSpeed = animationSpeed;
                //Debug.Log(straight.magnitude + " "+_speedBase+" "+ animationSpeed);
              //  state.Pitch = (int)pitch;
            }

            if (_skill && _isGrounded)
            {
                _skill.GetDirection(moveDirection1, this);
                _skill.UpdateAbility(ability1);
                //if (entity.IsOwner && ability1 && !jump)
                   //state.AnimAttack = 5;
            }
            if (_grenade)
                _grenade.UpdateAbility(ability2);
        }
        //Vector3 rotation2 = Vector3.zero;
        State stateMotor = new State();
        stateMotor.position = transform.position;
        stateMotor.rotation = Mathf.Atan2(transform.forward.x, transform.forward.z) * (180 / Mathf.PI);

        return stateMotor;
    }
    public void RotateTowards(float rotation)
    {
        if (entity.IsOwner)
        {
            _networkRigidbody.rotation = rotation;
        }
    }
    public void LockedRotationTowards(float rotation)
    {
        if (entity.IsOwner)
        {
            lockedRotation = rotation;
        }
    }
    public void QueueAnimation(int animCode)
    {
        if (BoltNetwork.IsServer)
        {
            state.AnimAttack = animCode;
        }
    }
    public void ResetQueue(int checkCode)
    {
        // if (state.AnimAttack == 5 )
        if (BoltNetwork.IsServer)
        {
            if(state.AnimAttack == checkCode) // rolling animation
            state.AnimAttack = 0;
        }
    }
    public void SetPitch()
    {
      //  if (!entity.IsControllerOrOwner)
         //   _cam.transform.localEulerAngles = new Vector3(state.Pitch, 0f, 0f);
    }


    private void FixedUpdate()
    {
        if (entity.IsAttached)
        {
            if (entity.IsOwner)
            {
                bool flag = false;
                RaycastHit[] hit = Physics.RaycastAll(transform.position, Vector3.down, 1.3f, ground);
                
                for(int i =0; i< hit.Length; i++)
                {
                    //Debug.Log(hit[i].transform.gameObject);

                    if(hit[i].transform.gameObject != this.gameObject)
                    {
                        flag = true;
                //{
                    float slopeNormal = Mathf.Abs(Vector3.Angle(hit[i].normal, new Vector3(hit[i].normal.x, 0, hit[i].normal.z)) - 90) % 90;

                    if (_networkRigidbody.MoveVelocity.y < 0)
                        _networkRigidbody.MoveVelocity = Vector3.Scale(_networkRigidbody.MoveVelocity, new Vector3(1, 0, 1));

                    if (!_isGrounded && slopeNormal <= _maxAngle)
                    {
                        _isGrounded = true;
                            state.AnimJump = false;
                    }
                    }
                }
                    if(!flag)
                    {
                        if (_isGrounded)
                        {
                            _isGrounded = false;
                        }
                    state.AnimJump = true;
                    }

                if(transform.position.y < -60)
                {
                    PowerStats powerstats1 = test.GetComponent<PowerStats>();
                    _powerStats.Life(powerstats1, -1000000, 0);
                }
            }
                
                CrowdControll();
            
        }
        calculateFrame = true;
    }
    public static void SmoothMove(Transform moveTransform, Vector3 position, float rotation, float timeDelta, ref float closingSpeed, ref float turnSmoothVel, float SmoothTime)
    {
        var posDiff = position - moveTransform.position;
        //var angleDiff = Quaternion.Angle(targetTransform.transform.rotation, moveTransform.rotation);
        float posDiffMag = posDiff.magnitude;

        if (posDiffMag > 0)
        {
            closingSpeed = Mathf.Max(closingSpeed, Mathf.Max(k_MinSmoothSpeed, posDiffMag / k_TargetCatchupTime));

            float maxMove = timeDelta * closingSpeed;
            float moveDist = Mathf.Min(maxMove, posDiffMag);
            posDiff *= (moveDist / posDiffMag);

           // moveTransform.position += posDiff;
            moveTransform.position += new Vector3(posDiff.x, posDiff.y, posDiff.z);

            if (moveDist == posDiffMag)
            {
                //we capped the move, meaning we exactly reached our target transform. Time to reset our velocity.
                closingSpeed = 0;
            }
        }
        else
        {
            closingSpeed = 0;
        }


        float targetAngle = rotation;

        float angle = Mathf.SmoothDampAngle(moveTransform.eulerAngles.y, targetAngle, ref turnSmoothVel, SmoothTime);

        moveTransform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void SetMyState(Vector3 position, float rotation)
    {
        SmoothMove(transform, position, rotation, Time.deltaTime, ref m_SmoothedSpeed, ref turnSmoothVelocity1, turnSmoothtime1);
       // SetState(position, rotation);
    }
    public void SetState(Vector3 position, float rotation)
    {
        //if (entity.IsOwner)
        if (entity.IsControllerOrOwner)
        {
            if (entity.IsOwner)
            {
                if (Mathf.Abs(rotation - transform.rotation.y) > 5f)
                    transform.rotation = Quaternion.Euler(0, rotation, 0);

                if (_firstState)
                {
                    if (position != Vector3.zero)
                    {
                        transform.position = position;
                        _firstState = false;
                        _lastServerPos = Vector3.zero;
                    }
                }
                else
                {
                    if (position != Vector3.zero)
                    {
                        _lastServerPos = position;
                    }

                    transform.position += (_lastServerPos - transform.position) * 0.5f;
                }
            }
        }
        else
        {
            if (Mathf.Abs(rotation - transform.rotation.y) > 5f)
                transform.rotation = Quaternion.Euler(0, rotation, 0);

            if (_firstState)
            {
                if (position != Vector3.zero)
                {
                    transform.position = position;
                    _firstState = false;
                    _lastServerPos = Vector3.zero;
                }
            }
            else
            {
                if (position != Vector3.zero)
                {
                    _lastServerPos = position;
                }

                transform.position += (_lastServerPos - transform.position) * 0.5f;
            }
        }
    }
    public void UpdateStats(int health, int maxhealth)
    {
        state.LifePoints = health;
        state.TotalLifePoints = maxhealth;
    }
    public void MakeDead()
    {
        state.IsDead = true;
    }
    public void UpdateMoney(int money, int poweress)
    {
        state.Money = money;
        state.PowerEssence = poweress;
    }
    public bool IsHeadshot(Collider c)
    {
        //return c == _headCollider;
        return false;
    }

    
    public void CrowdControll()
    {
        if (entity.IsOwner)
        {
            bool immobilized = false;

            if(immobilizedTime0>0f)
            {
                immobilizedTime0 -= BoltNetwork.FrameDeltaTime;
                immobilized = true;
                _networkRigidbody.MoveVelocity = Vector3.zero;
            }

            isImmobilized = immobilized;
            state.IsImmobilized = isImmobilized;
        }
    }
    public void AddCrowdControll(CCeffect cctype, float duration)
    {
        if(cctype == CCeffect.Immobilize)
        {
            if (duration > immobilizedTime0)
            {
                immobilizedTime0 = duration;
            }
        }
    }
    
    public void OnDeath(bool b)
    {
        _networkRigidbody.enabled = !b;
       // _headCollider.enabled = !b;
        _capsuleCollider.enabled = !b;
        hasDied = b;
        if (!b) //live
            senddeathinfo = true;
        if (entity.IsOwner)
        {
            if(b == true && senddeathinfo)
            {
                _powerStats.DeathReset();
                senddeathinfo = false;
            }
            
        }
    }
    public struct State
    {
        public Vector3 position;
        public float rotation;
    }
}