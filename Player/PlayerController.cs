using UnityEngine;
using Photon.Bolt;
using System.Collections.Generic;
using Cinemachine;

using System.Collections;

public class PlayerController : EntityBehaviour<INewPlayerState>
{
    private PlayerMotor _playerMotor;
    // private PlayerWeapons _playerWeapons;
    private PlayerDamageSystem _playerDamageSystem;
    private PlayerRenderer _playerRenderer;
    private bool _forward;
    private bool _backward;
    private bool _left;
    private bool _right;
    private float _camRotation;
    private float _yaw;
    private float _pitch;
    private bool _jump;
    private Vector3 movement;

    private bool _fire;
    private bool _aiming;
    private bool _reload;
    private int _wheel =1;
    private bool _drop;
    private bool _ability1;
    private bool _ability2;
    private bool _hotKey1;
    private bool _hotKey2;
    float targetAngle = 0;
    float mousedir2 = 0;
    bool assistance = true;
    private Vector3 delta = Vector3.zero;
    private Vector3 lastPos = Vector3.zero;
    private GameObject displaydrawing = null;
    [SerializeField]
    private GameObject displaydrawingParent;
    [SerializeField]
    private CinemachineFreeLook cam1;
    private float camx;
    private float camy;

    private bool _hasControl = false;
    private bool _isInMenu = false;
    private bool _isInOptions = false;
    private bool _isInStore = false;
    private bool _isInLeaderboard = false;
    private float _mouseSensitivity = 5f;
    [SerializeField]
    private GameObject freelookCam = null;

    public int Wheel { get => _wheel; set => _wheel = value; }

    public List<Targetable> _playersInside = new List<Targetable>();
    public bool focusMode = false;
    public int index = 0;
    public float mousex = 0;
    private Targetable previustarget;

    private Vector3 _aimdirection1 = Vector3.zero;
    private Vector3 localdirection = Vector3.zero;
    private Vector3 serverdirection = Vector3.zero;
    private float cdcounter = 0;
    private float repeatTimer = 0.45f;
    private float repeatTimer0 = 0;
    private bool ShowCursor = false;
    private int aimtype = 2;
    [SerializeField]
    private LayerMask ignorePlayer;
    [SerializeField]
    private bool canOpenStore = false;
    private bool canOpenLeaderboard = false;
    private bool firstpress = false;
    private bool doublepress = false;
    private float doublepressTime = 0.5f;
    private float doublepresstime0 = 0f;
    private float holdOpen0 = 0f;
    private bool canAttack = true;

    public void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        // _playerWeapons = GetComponent<PlayerWeapons>();
        _playerRenderer = GetComponent<PlayerRenderer>();

        _playerDamageSystem = GetComponent<PlayerDamageSystem>();
        //Cursor.SetCursor(CursorArrow, hotspot, CursorMode.ForceSoftware);
    }

    /* public override void MissingCommand(Command previous)
     {

     }*/
    public override void Attached()
    {
        if (entity.HasControl)
        {
            Debug.Log("Im being executed");
            _hasControl = true;
            GameController.Current.localPlayer = gameObject;

            
            camx = cam1.m_XAxis.m_MaxSpeed;
            camy = cam1.m_YAxis.m_MaxSpeed;


            ShowCursor = true;
            if (ShowCursor)
            {
                // Cursor.SetCursor(CursorArrow, hotspot, CursorMode.ForceSoftware);
                GUI_Controller.Current.ShowCursor(true);
                cam1.m_XAxis.m_MaxSpeed = 0;
                cam1.m_YAxis.m_MaxSpeed = 0;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                GUI_Controller.Current.ShowCursor(false);

                // Cursor.SetCursor(CursorArrow, hotspot, CursorMode.Auto);
                cam1.m_XAxis.m_MaxSpeed = camx;
                cam1.m_YAxis.m_MaxSpeed = camy;
            }


            GUI_Controller.Current.SetPlayerController(this);
            gameObject.layer = 6;
            GameObject.Find("ItemStore").GetComponent<ItemStoreRangeChecker>().SetPlayer(this);
            GameObject.Find("LeaderboardStand").GetComponent<LeaderboardRangeChecker>().SetPlayer(this);
        }

        Init(entity.HasControl);
        _playerMotor.Init(entity.HasControl);
        _playerRenderer.Init();
    }


    public void SetAttackAbility(bool yes)
    {
        canAttack = yes;
    }
    public override void ControlGained()
    {
        GUI_Controller.Current.Show(true);
    }
    public void SetOpenCloseStore(bool yes)
    {
        canOpenStore = yes;
    }
    public void SetOpenCloseLeaderboard(bool yes)
    {
        canOpenLeaderboard = yes;
    }
    public void Init(bool isMine)
    {
        if (isMine)
        {
            FindObjectOfType<PlayerSetupController>().SceneCamera.gameObject.SetActive(false);

            Targetable exp = GetComponent<Targetable>();
            Destroy(exp);


            StartCoroutine(StartEnum(3f));
            //_playersInside.RemoveAt(0);
        }
        else if(!BoltNetwork.IsServer)
        {
            PlayerServerEvents evntlistener = GetComponent<PlayerServerEvents>();
            Destroy(evntlistener);

        }
    }
    private void ResetKeys()
    {
        _forward = false;
        _backward = false;
        _left = false;
        _right = false;
        _jump = false;

        _fire = false;
        _aiming = false;
        _reload = false;
        _drop = false;
        _ability1 = false;
        _ability2 = false;
        _aimdirection1 = Vector3.zero;

        _hotKey1 = false;
        _hotKey2 = false;
    }
    private void LockUnlockCamera(bool yes)
    {
        ShowCursor = !ShowCursor;
        if (ShowCursor)
        {
            cam1.m_XAxis.m_MaxSpeed = 0;
            cam1.m_YAxis.m_MaxSpeed = 0;

        }
        else
        {
            cam1.m_XAxis.m_MaxSpeed = camx;
            cam1.m_YAxis.m_MaxSpeed = camy;
            //Cursor.lockState = CursorLockMode.None;
        }
    }
    private void Update()
    {
        if (_hasControl)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ShowCursor = !ShowCursor;
                if (ShowCursor)
                {
                    GUI_Controller.Current.ShowCursor(true);
                    cam1.m_XAxis.m_MaxSpeed = 0;
                    cam1.m_YAxis.m_MaxSpeed = 0;

                }
                else
                {
                    GUI_Controller.Current.ShowCursor(false);

                    cam1.m_XAxis.m_MaxSpeed = camx;
                    cam1.m_YAxis.m_MaxSpeed = camy;
                    //Cursor.lockState = CursorLockMode.None;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                focusMode = !focusMode;
                if (focusMode != true)
                {
                    previustarget.SetTargetSprite(false);
                    previustarget = null;
                }
                else if (_playersInside[0].GetComponent<Targetable>()!=null)
                {
                    previustarget = _playersInside[0];
                    previustarget.SetTargetSprite(true);
                    index = 0;
                }
                else
                {
                    focusMode = false;
                }
                ChanceTargetEvent();
                Debug.Log("focusMode " + focusMode);
            }
            mousex = Input.GetAxis("Mouse ScrollWheel");
            if (mousex != 0 && focusMode)
            {
                if (mousex > 0 && index + 1 <= _playersInside.Count)
                {
                    index++;
                }
                if (mousex > 0 && index + 1 > _playersInside.Count)
                {
                    index = 0;
                }
                if (mousex < 0 && index - 1 < 0)
                {
                    index = _playersInside.Count;
                }
                if (mousex < 0 && index - 1 >= 0)
                {
                    index--;
                }

                if (previustarget)
                {
                    previustarget.SetTargetSprite(false);
                    previustarget = null;
                }
                if (_playersInside.Count > 0)
                {
                    previustarget = _playersInside[index];
                    previustarget.SetTargetSprite(true);
                }
                ChanceTargetEvent();
            }

            //if (Input.GetKeyDown(KeyCode.M))
               // OpenCloseShop();
            if (Input.GetKeyDown(KeyCode.Escape))
                OpenCloseOptions();
            if (Input.GetKeyDown(KeyCode.B) && !_isInOptions)
                OpenCloseStore();
            if (Input.GetKeyDown(KeyCode.N) && !_isInOptions)
                OpenCloseLeaderboard();

            if (!_isInMenu && !_isInOptions)// && isInChat
            {
                PollKeys();
            }

            if (!ShowCursor && !_isInMenu && !_isInOptions && !_isInStore && !_isInLeaderboard)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    public void OpenCloseOptions()
    {
        if (_isInOptions)
        {
            cam1.m_XAxis.m_MaxSpeed = camx;
            cam1.m_YAxis.m_MaxSpeed = camy;

        }
        else
        {
            cam1.m_XAxis.m_MaxSpeed = 0;
            cam1.m_YAxis.m_MaxSpeed = 0;
            ResetKeys();
        }
        _isInOptions ^= true;
        GUI_Controller.Current.ShowOptions(_isInOptions);
    }
    public void OpenCloseShop()
    {
        if (_isInMenu)
        {
            cam1.m_XAxis.m_MaxSpeed = camx;
            cam1.m_YAxis.m_MaxSpeed = camy;
        }
        else
        {
            cam1.m_XAxis.m_MaxSpeed = 0;
            cam1.m_YAxis.m_MaxSpeed = 0;
            ResetKeys();
        }
        _isInMenu ^= true;
        GUI_Controller.Current.ShowShop(_isInMenu);
    }
    public void OpenCloseStore()
    {
        if (canOpenStore && !_isInStore)
        {
            GUI_Controller.Current.ShowCursor(true);

            cam1.m_XAxis.m_MaxSpeed = 0;
            cam1.m_YAxis.m_MaxSpeed = 0;
           // ResetKeys();
            
            _isInStore = true;
            GUI_Controller.Current.ShowStore(_isInStore);
            return;
        }
        if (_isInStore)
        {
            GUI_Controller.Current.ShowCursor(false);

            cam1.m_XAxis.m_MaxSpeed = camx;
            cam1.m_YAxis.m_MaxSpeed = camy;
            _isInStore = false;
        }
        GUI_Controller.Current.ShowStore(_isInStore);

    }
    public void OpenCloseLeaderboard()
    {
        if (canOpenLeaderboard)
        {
            if (_isInLeaderboard)
            {
                cam1.m_XAxis.m_MaxSpeed = camx;
                cam1.m_YAxis.m_MaxSpeed = camy;
            }
            else
            {
                cam1.m_XAxis.m_MaxSpeed = 0;
                cam1.m_YAxis.m_MaxSpeed = 0;
                ResetKeys();
            }
            _isInLeaderboard ^= true;
            GUI_Controller.Current.ShowLeaderboard(_isInLeaderboard);
        }
    }
    public bool IsInStore()
    {
        return _isInStore;
    }
    public bool IsInLeaderboard()
    {
        return _isInLeaderboard;
    }
    public void ChangeAimingType(int mytype)
    {
        aimtype = mytype;
        GUI_Controller.Current.UpdateAimOption(aimtype);
    }
    private void PollKeys()
    {
        _forward = Input.GetKey(KeyCode.W);
        _backward = Input.GetKey(KeyCode.S);
        _left = Input.GetKey(KeyCode.A);
        _right = Input.GetKey(KeyCode.D);
        // _jump = Input.GetKey(KeyCode.Space);

        if (!_isInStore && !_isInMenu && !_isInStore && !_isInOptions && !_isInLeaderboard && canAttack)
            _fire = Input.GetMouseButton(0);
        else
            _fire = false;

        _ability1 = Input.GetKey(KeyCode.Space);
       
        _jump = Input.GetKey(KeyCode.LeftShift);

        _hotKey1 = Input.GetKey(KeyCode.E);
        _hotKey2 = Input.GetKey(KeyCode.R);
       
        movement = Vector3.zero;
        _camRotation = freelookCam.transform.eulerAngles.y;


        if (_forward ^ _backward)
        {
            movement.z += _forward ? 1 : -1;
        }
        if (_left ^ _right)
        {
            movement.x += _right ? 1 : -1;
        }
        movement = Quaternion.Euler(0, _camRotation, 0) * movement;
        movement.Normalize();

        if (cdcounter > 0)
        {
            cdcounter -= Time.deltaTime;
        }
        if(aimtype == 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4))
            { 
                ShowCursor = true;

                lastPos = Input.mousePosition;
                //cancelAbility1 = false;
                displaydrawing = null;
                displaydrawing = Instantiate(AbilityList.V1.AbilitiesPreview[0], transform);
                displaydrawing.transform.SetParent(displaydrawingParent.transform);


                cam1.m_XAxis.m_MaxSpeed = 0;
                cam1.m_YAxis.m_MaxSpeed = 0;

            }
            if (!focusMode)
            {
                localdirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(0, 0, 2f) + transform.position;
            }
            else if (previustarget != null)
            {
                localdirection = previustarget.gameObject.transform.position;
            }

            if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Alpha4))
            {
                
                ShowCursor = false;

                cam1.m_XAxis.m_MaxSpeed = camx;
                cam1.m_YAxis.m_MaxSpeed = camy;

                for (int i = 0; i < displaydrawingParent.transform.childCount; i++)
                    Destroy(displaydrawingParent.transform.GetChild(i).gameObject);

                localdirection = Quaternion.Euler(0, targetAngle, 0) * new Vector3(0, 0, 2f) + transform.position;
                if (Input.GetKeyUp(KeyCode.Alpha3))
                {
                   // CallServerCommand(1, localdirection);
                    CallServerCommandR(1, targetAngle);
                }
                if (Input.GetKeyUp(KeyCode.Alpha4))
                {
                    //CallServerCommand(2, localdirection);
                    CallServerCommandR(2, targetAngle);
                }

                repeatTimer0 = 0;
            }
            if(repeatTimer0 < repeatTimer)
            {
                repeatTimer0 += Time.deltaTime;
                serverdirection = localdirection;
            }
            _aimdirection1 = serverdirection;

            _hotKey1 = false;
            if(assistance&&( Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Alpha4)))
            {
                // mouse direction
                float targetAngle3 = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
                delta = Input.mousePosition - lastPos;

                mousedir2 = delta.x;

                if (mousedir2 > 360)
                    mousedir2 = mousedir2 % 360;
                targetAngle = targetAngle3 + mousedir2;

                if(displaydrawing != null)
                {
                    displaydrawing.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
                }
            }
        }
        else if(aimtype == 1)
        {
            // mouse on environment
            if (Input.GetKeyDown(KeyCode.Alpha3))
                CalculateAimPosition(1);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                CalculateAimPosition(2);
        }
        else if (aimtype == 2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                ShowCursor = true;

                lastPos = Input.mousePosition;
                //cancelAbility1 = false;
                displaydrawing = null;
                displaydrawing = Instantiate(AbilityList.V1.AbilitiesPreview[0], transform);
                displaydrawing.transform.SetParent(displaydrawingParent.transform);


                cam1.m_XAxis.m_MaxSpeed = 0;
                cam1.m_YAxis.m_MaxSpeed = 0;

            }
            if (!focusMode)
            {
                localdirection = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(0, 0, 2f) + transform.position;
            }
            else if (previustarget != null)
            {
                localdirection = previustarget.gameObject.transform.position;
            }

            if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Alpha4))
            {
                //ShowCursor = false;

               // cam1.m_XAxis.m_MaxSpeed = camx;
               // cam1.m_YAxis.m_MaxSpeed = camy;

                for (int i = 0; i < displaydrawingParent.transform.childCount; i++)
                    Destroy(displaydrawingParent.transform.GetChild(i).gameObject);

                localdirection = Quaternion.Euler(0, targetAngle, 0) * new Vector3(0, 0, 2f) + transform.position;
                if (Input.GetKeyUp(KeyCode.Alpha3))
                {
                    // CallServerCommand(1, localdirection);
                    CallServerCommandR(1, targetAngle);
                }
                if (Input.GetKeyUp(KeyCode.Alpha4))
                {
                    //CallServerCommand(2, localdirection);
                    CallServerCommandR(2, targetAngle);
                }
                repeatTimer0 = 0;
            }
            if (repeatTimer0 < repeatTimer)
            {
                repeatTimer0 += Time.deltaTime;
                serverdirection = localdirection;
            }
            _aimdirection1 = serverdirection;

            _hotKey1 = false;
            if (assistance && (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Alpha4)))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 fullsubstraction;
                RaycastHit hit;

                // if (Physics.Raycast(ray, out hit, 100))
                if (Physics.Raycast(ray, out hit, 100, ignorePlayer))
                {
                    Debug.Log(hit.collider.gameObject);
                    fullsubstraction = new Vector3(hit.point.x, hit.point.y, hit.point.z) - transform.position;
                }
                else
                    fullsubstraction = transform.forward;

                targetAngle = Mathf.Atan2(fullsubstraction.x, fullsubstraction.z) * Mathf.Rad2Deg;

                localdirection = Quaternion.Euler(0, targetAngle, 0) * new Vector3(0, 0, 2f) + transform.position;

                if (displaydrawing != null)
                {
                    displaydrawing.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
                }
                //CallServerCommand(id, localdirection);
            }
        }
    }
    public void CalculateAimPosition(int id)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 fullsubstraction;
        RaycastHit hit;

       // if (Physics.Raycast(ray, out hit, 100))
        if (Physics.Raycast(ray, out hit, 100, ignorePlayer))
        {
            fullsubstraction = new Vector3(hit.point.x, hit.point.y, hit.point.z) - transform.position;
        }
        else
            fullsubstraction = transform.forward;

        targetAngle = Mathf.Atan2(fullsubstraction.x, fullsubstraction.z) * Mathf.Rad2Deg;

        localdirection = Quaternion.Euler(0, targetAngle, 0) * new Vector3(0, 0, 2f) + transform.position;

        //CallServerCommand(id, localdirection);
        CallServerCommandR(id, targetAngle);
    }
    public void CallServerCommand(int id, Vector3 diretion)
    {
        Debug.Log("Command called");
        var evnt = CallCommand.Create(GlobalTargets.OnlyServer);
        evnt.Id = id;
        evnt.ActionCaller = entity;
        evnt.direction = diretion;
        evnt.Send();
    }
    public void CallServerCommandR(int id, float xzrotation)
    {
        var evnt = CallCommandR.Create(GlobalTargets.OnlyServer);
        evnt.id = id;
        evnt.myplayer = entity;
        evnt.xzrotation = xzrotation;
        evnt.Send();
    }
    public void ChanceTargetEvent()
    {
        var evnt = MakeTarget.Create(GlobalTargets.OnlyServer);
        evnt.makefocus = focusMode;
        if (previustarget && focusMode)
        {
            if (previustarget.GetComponent<PlayerController>())
                evnt.focusedtarget = previustarget.GetComponent<PlayerController>().entity;
            else
                evnt.focusedtarget = previustarget.GetComponent<MobCombat>().entity;
        }
        else
            evnt.focusedtarget = null;
        evnt.ActionCaller = entity;
        evnt.Send();
    }
    public void ResetTarget(BoltEntity deadtarget)
    {
        BoltEntity currenttarget = null;
        if (previustarget)
        {
        if (previustarget.GetComponent<PlayerController>())
            currenttarget = previustarget.GetComponent<PlayerController>().entity;
        else
            currenttarget = previustarget.GetComponent<MobCombat>().entity;
        }

        if (currenttarget == deadtarget)
        {
            focusMode = false;
            previustarget.SetTargetSprite(false);
            //previustarget = null;
            ChanceTargetEvent();
            Debug.Log("focusMode " + focusMode);
        }
        for (int i = 0; i < _playersInside.Count; i++)
        {
            BoltEntity targetinlist;
            if (previustarget)
            {
            if (previustarget.GetComponent<PlayerController>())
                targetinlist = _playersInside[i].GetComponent<PlayerController>().entity;
            else
                targetinlist = _playersInside[i].GetComponent<MobCombat>().entity;
            if (targetinlist == deadtarget)
            {
                _playersInside.RemoveAt(i);
                if(currenttarget == deadtarget && currenttarget == targetinlist)
                {
                    previustarget = null;
                }
            }
            }
        }
    }
    public override void SimulateController()
    {
        Vector2 movedirection = Vector2.zero;
        IPlayerCommandInput input = PlayerCommand.Create();
        input.Forward = _forward;
        input.Backward = _backward;
        input.Right = _right;
        input.Left = _left;
        input.Yaw = _yaw;
        input.direction = movement;
        input.Pitch = _pitch;
        input.Jump = _jump;
        input.Drop = _drop;

        //input.direction1 = _aimdirection1;

        input.Fire = _fire;
        input.Scope = _aiming;
        input.Reload = _reload;
        input.Wheel = _wheel;

        input.Ability1 = _ability1;
        input.Ability2 = _ability2;

       // input.Hotkey1 = _hotKey1;
        //.Hotkey2 = _hotKey2;

        entity.QueueInput(input);

        _playerMotor.ExecuteCommand( _jump, movement, _ability1, _ability2);
        //_playerWeapons.ExecuteCommand(_fire, _aiming, _reload, _wheel, BoltNetwork.ServerFrame % 1024, _drop);
        _playerDamageSystem.ExecuteCommand(_fire);
    }


    public override void ExecuteCommand(Command command, bool resetState)
    {
        PlayerCommand cmd = (PlayerCommand)command;

        if (resetState)
        {
            if (entity.IsControllerOrOwner)
            {
                if (entity.IsOwner)
                    _playerMotor.SetState(cmd.Result.Position, cmd.Result.Rotation);
                else
                    _playerMotor.SetMyState(cmd.Result.Position, cmd.Result.Rotation);
                    
            }
            else
            {
                _playerMotor.SetState(cmd.Result.Position, cmd.Result.Rotation);
            }
        }
        else
        {
            PlayerMotor.State motorState = new PlayerMotor.State();

            motorState = _playerMotor.ExecuteCommand(
            cmd.Input.Jump,
            cmd.Input.direction,
            cmd.Input.Ability1,
            cmd.Input.Ability2);
                
            
            if (!entity.HasControl)
            {

                /* _playerWeapons.ExecuteCommand(
                 cmd.Input.Fire,
                 cmd.Input.Scope,
                 cmd.Input.Reload,
                 cmd.Input.Wheel,
                 cmd.ServerFrame % 1024, 
                 cmd.Input.Drop);*/
                _playerDamageSystem.ExecuteCommand(
                 cmd.Input.Fire);
                    
            }

            cmd.Result.Position = motorState.position;
            cmd.Result.Rotation = motorState.rotation;
        }
    }
    IEnumerator StartEnum(float time)
    {
        yield return new WaitForSeconds(time);
        GUI_Controller.Current.OpenEnterGameNotificationPanel(state.username);
    }
}