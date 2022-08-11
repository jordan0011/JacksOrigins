using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class PowerStats : MonoBehaviour
{
    [SerializeField]
    private int enemytype = -1;
    private int[] _powerStats = new int[9];  // se afto ginontai oi prakseis
    public int[] powerStats
    {
        get
        {
            return _powerStats; // 8a feugei sto diktio kai 8a enimervnontai ola
        }
    }
    [SerializeField]
    private bool hasDied = false;
    private int _health = 250;
    public int damage
    {
        set
        {
            _damage = value;
        }
        get
        {
            return _damage; // 8a feugei sto diktio kai 8a enimervnontai ola
        }
    }

    private int _damage = 10;
    private int _critchance = 0;
    private int _magicp = 0;
    private int _cdr = 0;
    [SerializeField]
    private int _maxHealth = 250;
    private int _armor = 3;
    private int _heavyRes = 2;
    private int _lifesteal = 0;
    private int _madness = 0;

    private int _money = 0;
    private int _poweressence = 0;
    public int money { set => _money = value; }
    public int poweressence { set => _poweressence = value; }
    public int level
    {
        set
        {
            _level = value;
        }
        get
        {
            return _level;
        }
    }
    [SerializeField]
    private int _level;
    private string _username;
    public string username
    {
        get
        {
            return _username;
        }
    }

    public int xp
    {
        get
        {
            return _xp;
        }
    }
    [SerializeField]
    private int _xp;
    private int _elo;
    public int elo
    {
        get
        {
            return _elo;
        }
    }
    private int[] myeffects = new int[40];
    [SerializeField]
    private PlayerMotor _playerMotor = null;
    private PlayerDamageSystem _playerDamageSystem = null;
    private MobCombat _mobCombat = null;
    private PlayerInventory _playerInventory = null;
    private PowerEffects _powerEffects = null;
    private int[] UniqueIds = new int[40];
    [SerializeField]
    private GameObject MobRecognition = null;
    [SerializeField]
    private List<PowerStats> _playerwhotouched;
    [SerializeField]
    private PowerStats _killer;
    private BossMobExtension _bossMobExtension;
    // ref to ui;
    // Start is called before the first frame update
    
    void Start()
    {
        if(_playerMotor)
            level = 0;
        AddplayerLevel();
        UpdateLevelStats();
        SetDefaultUsername();
        if (_mobCombat)
        {
            Debug.Log("Is in");
            if (_mobCombat.state.isDead)
            {
                _mobCombat.MakeDead();
                Debug.Log("is executed");
            }
        }
        if (BoltNetwork.IsClient)
        {
            if (MobRecognition)
                Destroy(MobRecognition);
        }
        _bossMobExtension = GetComponent<BossMobExtension>();
    }
    public void SetLevelStats(int mylevel)
    {
        if (BoltNetwork.IsServer)
        {
            Debug.Log("SetLevelStats: "+mylevel);
            _level = mylevel;

            int i = enemytype + 1;

            _mobCombat.state.Level = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetVirtualLevel();

            _damage = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[0];
            _critchance = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[1];
            _magicp = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[2];
            _cdr = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[3];
           // _maxHealth = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[4];
            _armor = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[5];
            _heavyRes = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[6];
            _lifesteal = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[7];
            _madness = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[8];
            _maxHealth = LevelStatList.V1.powerlvl[i].GetArray()[_level].GetLvlStats()[9];
            _health = _maxHealth;

            UpdateStatsView();
            _mobCombat.UpdateStats(_health, _maxHealth);
        }
    }
    public bool IsPlayer()
    {
        bool yes = true;
        if (enemytype <= 0)
            yes = false;
        return yes;
    }
    private void OnEnable()
    {
       // _playerMotor = GetComponent<PlayerMotor>();
        _mobCombat = GetComponent<MobCombat>();
        if (_playerMotor)
            _playerDamageSystem = GetComponent<PlayerDamageSystem>();
        _playerInventory = GetComponent<PlayerInventory>();
        _powerEffects = GetComponent<PowerEffects>();

        if (BoltNetwork.IsServer)
        {
            if (_mobCombat)
            {
                MobRecognition.GetComponent<MobRecognition>().SetScript(this);
            }
        }
        
        UpdateLevelStats();
        UpdateStatsView();
        ReloadStats();
        
       // if (_playerMotor)
       // {
       //if(_playerMotor)
          //  SetPlayerLevel(1);

            //GUI_Controller.Current.UpdateStatsInfoView(myeffects); // make it fake/real

        //}
    }
    public void AddplayerLevel()
    {
        if (BoltNetwork.IsServer)
        {
            if (_playerMotor)
            {                                      //entity type     // level
                _level++;   
                LevelStatList.Level mylevel = LevelStatList.V1.powerlvl[0].GetArray()[_level];
                string levelstats = "";
                for (int i = 0; i < mylevel.GetLvlStats().Length; i++)
                {
                    levelstats += ", " + mylevel.GetLvlStats()[i] + " ";
                }
                Debug.Log(levelstats);

                _damage = mylevel.GetLvlStats()[0];
                _critchance = mylevel.GetLvlStats()[1];
                _magicp = mylevel.GetLvlStats()[2];
                _cdr = mylevel.GetLvlStats()[3] - _cdr;
                //_maxHealth = mylevel.GetLvlStats()[4];
                _armor = mylevel.GetLvlStats()[5];
                _heavyRes = mylevel.GetLvlStats()[6];
                _lifesteal = mylevel.GetLvlStats()[7];
                _madness = mylevel.GetLvlStats()[8];
                _health = _health + mylevel.GetLvlStats()[9] - _maxHealth;
                _maxHealth = mylevel.GetLvlStats()[9];

                UpdateStatsView();
                ReloadStats();
                _playerMotor.UpdateStats(_health, _maxHealth);

                if(level == 1)
                {
                    _money = 50;
                    _poweressence = 100;
                }
                UpdatePlayerMoney();
            }
        }

        //Debug.Log("New Level: "+ _level + " Next Level xp: " + LevelStatList.V1.playerlevelupxp[_level-1]);
    }
    public void AddXp(int xpadd)
    {
        if (BoltNetwork.IsServer)
        {
            Debug.Log("Xp ADDED: "+ xpadd);
            int currentlevel = _level;
            while(_xp + xpadd >= LevelStatList.V1.playerlevelupxp[currentlevel -1])
            {
                Debug.Log("Level Adder: " +( level - 1));
                AddplayerLevel();
                _xp = 0 + _xp + xpadd - LevelStatList.V1.playerlevelupxp[currentlevel -1];
                xpadd = 0;
                currentlevel++;
            }
            _xp += xpadd;

            UpdateLevelStats();
        }
    }
    public void UpdateLevelStats()
    {
        if (BoltNetwork.IsServer)
        {
            if (_playerMotor)
            {
            _playerMotor.state.myXp = _xp;
            _playerMotor.state.myLevel = _level;
            }
        }
    }
    public string Make3digString(int n)
    {
        int k = n;
        int d = 0;
        string a ="";
        while(n > 0)
        {
            n = n / 10;
            d++;
        }
        
        for(int i=0; i< 3- d; i++)
        {
            a += "0";
        }
        a += k.ToString();
        return a;
    }
    public void SetDefaultUsername()
    {
        if (BoltNetwork.IsServer)
        {
            string username = "guest000" + Make3digString(Random.Range(0, 999));
            bool nextTry = true;

            while (nextTry)
            {
                if (LevelStatList.V1.RegisterNewPlayer(username))
                {
                    _username = username;
                    if (_playerMotor)
                    {
                        _playerMotor.state.username = username;
                    }
                        nextTry = false;
                }
                else
                {
                    nextTry = true;
                    //show failled message
                }
            }
        }
    }
    public void ChangeUsername(string newusername)
    {
        if (BoltNetwork.IsServer)
        {
            if(LevelStatList.V1.ChangePlayerName(username, newusername))
            {
                _username = newusername;
                _playerMotor.state.username = newusername;
            }
            else
            {
                //show failed message;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    public GameObject GetMobRecObject()
    {
        return MobRecognition;
    }
    public int GetEnemyType()
    {
        return enemytype;
    }
    public PlayerDamageSystem GetDamageSystem()
    {
        return _playerDamageSystem;
    }
    public PowerEffects GetPowerEffects()
    {
        return _powerEffects;
    }
    public void DeathReset()
    {// called once he dies
        _poweressence = 0;
        _money = 0;
        UpdatePlayerMoney();
        hasDied = false;
        _health = _maxHealth;
        _playerMotor.UpdateStats(_health, _maxHealth);
    }
    public int GetMoney()
    {
        return _money;
    }
    public int GetPoweressence()
    {
        return _poweressence;
    }
    public int GetmaxHealth()
    {
       // state.TotalLifePoints = _maxHealth;
        return _maxHealth;
    }
    public MobCombat GetMobCombat()
    {
        return _mobCombat;
    }
    public PlayerMotor GetPlayerMotor()
    {
        return _playerMotor;
    }
    public Photon.Bolt.BoltEntity GetEntity()
    {
        if (_playerMotor)
            return _playerMotor.entity;
        else if (_mobCombat)
            return _mobCombat.entity;
        else
            return null;
    }
    public void Life(PowerStats killer, int life,int type)
    {
        if (BoltNetwork.IsServer)
        {
            ShowPlayersDamageNumber(-life, type, killer);
            if (_playerMotor)
                PlayerLife(killer, life);
            else if (_mobCombat)
                MobLife(killer, life);
        }
    }
    public void PlayerLife(PowerStats killer, int life)
    {
        if (BoltNetwork.IsServer)
        {
            int value = _health + life;

            if (value <= 0 && !hasDied)
            {
                _killer = killer;
                hasDied = true;
                _health = 0;
                //state.IsDead = true;
                _playerMotor.MakeDead();
                //MobRecognition.GetComponent<MobRecognition>().EraseDeadPlayer(_playerMotor);
                RaiseResetTargetEvent();

                int essence = LevelStatList.V1.powerlvl[0].GetArray()[_level].GetLvlStats()[10];

                killer.money = killer.GetMoney() + essence;
                ShowPlayersDamageNumber(essence, 1, _killer);

                int darkessence = LevelStatList.V1.powerlvl[0].GetArray()[_level].GetLvlStats()[11];

                killer.poweressence = killer.GetPoweressence() + darkessence;
                ShowPlayersDamageNumber(darkessence, 2, _killer);
                killer.UpdatePlayerMoney();
                int xpvalue = 35;
                killer.AddXp(xpvalue);
                ShowPlayersDamageNumber(xpvalue, 3, _killer);

                RewardPlayerswhotouched(30, 1, 10);

                RaiseDeathVfxEvent(transform.position);
            }
            else if (value > _maxHealth)
            {
                _health = _maxHealth;
            }
            else
            {
                _health = value;
            }
            AddPlayerWhoTouched(killer);
            _powerEffects.OnHealth();
            _playerMotor.UpdateStats(_health, _maxHealth);
        }
    }
    public void MobLife(PowerStats killer, int life)
    {
        if (BoltNetwork.IsServer)
        {
            int value = _health + life;

            if (value <= 0 && !hasDied)
            {
                _killer = killer;
                hasDied = true;

                _health = 0;
                _mobCombat.MakeDead();
                _mobCombat.state.isDead = true;
               // killer._playerInventory.InventoryAlteration(0, 10, -1);

                RaiseDeathVfxEvent(transform.position);

                if(_bossMobExtension)
                    _bossMobExtension.RaiseKillNotificationEvent();

                int essence = LevelStatList.V1.powerlvl[enemytype + 1].GetArray()[_level].GetLvlStats()[10];

                killer.money = killer.GetMoney() + essence;
                ShowPlayersDamageNumber(essence, 2, _killer);

                int darkessence = LevelStatList.V1.powerlvl[enemytype + 1].GetArray()[_level].GetLvlStats()[11];
                killer.poweressence = killer.GetPoweressence() + darkessence;
                ShowPlayersDamageNumber(darkessence, 1, _killer);
                killer.UpdatePlayerMoney();
                int xpvalue = LevelStatList.V1.powerlvl[enemytype + 1].GetArray()[_level].GetLvlStats()[12];
                killer.AddXp(xpvalue);
                ShowPlayersDamageNumber(xpvalue, 3, _killer);
                Debug.Log(" killed xp: " + LevelStatList.V1.powerlvl[enemytype +1].GetArray()[_level].GetLvlStats()[12]);
                RewardPlayerswhotouched(30, 0, 10);

                RaiseEnemyDiedEvent();
                RaiseResetTargetEvent();
                MobRecognition.GetComponent<MobRecognition>().SpreadEraseMob(_mobCombat);

                _killer.Life(this, 50, 4); //heal
            }                                      // 0 is for players
            else if (value > _maxHealth)
            {
                _health = _maxHealth;
            }
            else
            {
                _health = value;

            }
            AddPlayerWhoTouched(killer);
            _powerEffects.OnHealth();
            _mobCombat.UpdateStats(_health, _maxHealth);
        }
    }
    public void SetEssence(int a, int b)
    {
        _money += a;
        _poweressence += b;
        UpdatePlayerMoney();
    }
    public void CalculateStats(item[] myequipment)
    {
        if (BoltNetwork.IsServer)
        {
            UpdateStatsView();
           // for(int i=0; i< myeffects.Length; i++)
           // {
           //     myeffects[i] = -1;
           // }
            if (_playerInventory)
            {
                 if (myequipment[0].GetItemID() > -1)
                 {
                    for(int i=0; i< myeffects.Length; i++)
                    {
                        if (i < ItemList.Items.itemInfo[myequipment[0].GetItemID()].getEffectIDs().Length)
                        {
                            myeffects[i] = ItemList.Items.itemInfo[myequipment[0].GetItemID()].getEffectId(i);
                        }
                        else
                        {
                            int n = -1;
                            myeffects[i] = n;
                        }    
                    }
                }
            }
            string effects = "";
            for(int i =0; i<myeffects.Length; i++)
            {
                effects += myeffects[i] + ", ";
            }
            Debug.Log(effects);


            for(int i=0; i<UniqueIds.Length; i++)
            {
                if(UniqueIds[i] != -1)
                {
                    int n = -1;
                    _powerEffects.TurnOffEffect(UniqueIds[i]);
                    UniqueIds[i] = n;
                }
            }
            Debug.Log(UniqueIds[0] +", "+ UniqueIds[1] + ", " + UniqueIds[2] + ", " + UniqueIds[3] + ", " + UniqueIds[4]);


            if(myequipment[0].GetItemID() > -1)
            {
                for(int i=0; i< ItemList.Items.itemInfo[myequipment[0].GetItemID()].getEffectIDs().Length; i++)
                {
                    UniqueIds[i] = _powerEffects.FirstEmptyId();
                    _powerEffects.AddEffect(myeffects[i], UniqueIds[i]);
                    Debug.Log(myeffects[i]+ ", "+ UniqueIds[i]);
                
                }
            }
        }
    }
    public bool GetHasDied()
    {
        return hasDied;
    }
    public void UpdateStatsView()
    {
        if (BoltNetwork.IsServer)
        {
            _powerStats[0] = _damage;
            _powerStats[1] = _critchance;
            _powerStats[2] = _magicp;
            _powerStats[3] = _cdr;
            _powerStats[4] = _armor;
            _powerStats[5] = _heavyRes;
            _powerStats[6] = _lifesteal;
            _powerStats[7] = _madness;
            _powerStats[8] = _maxHealth;

            if (_playerInventory)
            {
                item[] equipeditems = _playerInventory.GetEquipment();
                if (equipeditems != null && equipeditems[0].GetItemID()!= -1)
                {
                    if(equipeditems[0].GetItemID() != -1)
                    {
                        itemObject itemobject = ItemList.Items.itemInfo[equipeditems[0].GetItemID()];
                        for (int i = 0; i < 8; i++)
                        {
                            _powerStats[i] += itemobject.getStat(i);
                        }
                        _maxHealth += itemobject.getStat(8);
                        _health += itemobject.getStat(8);
                        _powerStats[8] = _maxHealth;
                        _playerMotor.UpdateStats(_health, _maxHealth);
                    }
                }
            }

            Debug.Log("new Chance: "+_powerStats[0] + ", " + _powerStats[1] + ", " + _powerStats[2] + ", " + _powerStats[3] + ", " + _powerStats[4] + ", " + _powerStats[5] + ", " + _powerStats[6] + ", " + _powerStats[7] + ", " + _powerStats[8]);
            if (_playerInventory)
            {
                for (int i = 0; i < 9; i++)
                {
                    int s = _powerStats[i];
                    _playerInventory.state.Stats[i] =s;
                }
            }
        }
    }
    public void ReloadStats()
    {
        if (BoltNetwork.IsServer)
        {
            if (_playerInventory)
            {
            for (int i = 0; i < 9; i++)
            {
                int s = _powerStats[i];
                _playerInventory.state.Stats[i] = s;
            }
            }
        }
    }
    public void UpdatePlayerMoney()
    {
        if(_playerMotor) // den kanei update sto start
            _playerMotor.UpdateMoney(_money, _poweressence);
    }
    public void RotateTowards(float rotation)
    {
        _playerMotor.LockedRotationTowards(rotation);
    }
    private void ActivateEffects()
    {

    }
    private void AddPlayerWhoTouched(PowerStats newplayer)
    {
        bool newadd = true;
        for(int i=0; i< _playerwhotouched.Count; i++)
        {
            if(newplayer == _playerwhotouched[i])
            {
                newadd = false;
            }
        }
        if (newadd)
        {
            _playerwhotouched.Add(newplayer);
        }
    }
    private void RewardPlayerswhotouched(int money, int poweressence, int xp)
    {
        if (_playerwhotouched.Count > 1)
        {
            for (int i = 0; i < _playerwhotouched.Count; i++)
            {
                PowerStats playerwhotouched = _playerwhotouched[i];
                if(playerwhotouched != _killer)
                {
                    playerwhotouched.money = playerwhotouched.GetMoney() + money;
                    playerwhotouched.poweressence = playerwhotouched.GetPoweressence() + poweressence;
                    playerwhotouched.UpdatePlayerMoney();
                    playerwhotouched.AddXp(xp);
                    ShowPlayersDamageNumber(money, 1, playerwhotouched);
                    ShowPlayersDamageNumber(poweressence, 2, playerwhotouched);
                    ShowPlayersDamageNumber(xp, 3, playerwhotouched);
                }
            }
        }
    }
    private void RaiseEnemyDiedEvent()
    {
        if (BoltNetwork.IsServer)
        {
            Debug.Log("Event Delivered");
        var evnt = EnityDiedEvent.Create(GlobalTargets.AllClients);
        evnt.myentity = _mobCombat.entity;
        evnt.Send();
        }
    }
    public List<PowerStats> GetPlayersWhoTouched()
    {
        return _playerwhotouched;
    }
    public PowerStats GetKiller()
    {
        return _killer;
    }
    private void RaiseResetTargetEvent()
    {
        if (BoltNetwork.IsServer)
        {
            var evnt = ResetTargetEvent.Create(GlobalTargets.AllClients);
            evnt.myentitydied = GetEntity();
            evnt.Send();
        }
    }
    public void ShowPlayersDamageNumber(int damage, int type, PowerStats entity)
    {  
        if(damage>0)
            RaiseShowDamageEvent(damage, type, entity);
      //  RaiseShowDamageEvent(-damage, 3, entity); 1 dark, 2 essence, 3 xp

    }
    private void RaiseShowDamageEvent(int damage, int type, PowerStats entity)
    {
        if (BoltNetwork.IsServer)
        {
            var evnt = ShowDamageTextEvent.Create(GlobalTargets.AllClients);
            evnt.value = damage;
            evnt.type = type;
            evnt.myentity = entity.GetEntity();
            evnt.targeton = GetEntity();
            evnt.Send();
        }
    }
    private void RaiseDeathVfxEvent(Vector3 position)
    {
        if (BoltNetwork.IsServer)
        {
            var evnt = DeathVfxEvent.Create(GlobalTargets.AllClients);
            evnt.position = position;
            evnt.id = 0;
            evnt.Send();
        }
    }
}
