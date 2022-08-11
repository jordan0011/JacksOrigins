using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Bolt;

public class GUI_Controller : MonoBehaviour
{
    #region Singleton
    private static GUI_Controller _instance = null;

    public static GUI_Controller Current
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GUI_Controller>();

            return _instance;
        }
    }
    #endregion

    [SerializeField]
    private UI_HealthBar _healthBar = null;

    [SerializeField]
    private UI_AmmoPanel _ammoPanel = null;

    [SerializeField]
    private UI_Cooldown _skill = null;

    [SerializeField]
    private UI_Cooldown _grenade = null;

    [SerializeField]
    private Text _energyCount = null;

    public UI_Cooldown Skill { get => _skill; }
    public UI_Cooldown Grenade { get => _grenade; }

    [SerializeField]
    private Image _blindMask = null;
    Coroutine blind;

    [SerializeField]
    private Image _deathMask = null;
    Coroutine death;

    [SerializeField]
    private Text _money = null;

    [SerializeField]
    private Text _powerEssence = null;

    [SerializeField]
    private Text shopmoney = null;

    [SerializeField]
    private Text shopEssence = null;

    [SerializeField]
    private Text[] _StatsInfo = null;

    [SerializeField]
    private Text _inventoryItems = null;
    [SerializeField]
    private Text ServerInventoryItems = null;

    [SerializeField]
    private Text _equipmentItems = null;
    [SerializeField]
    private Text ServerEquipmentItems = null;

    [SerializeField]
    private Text _spellbookAbilities = null;
    [SerializeField]
    private Text _activeSpellsAbilities = null;

    [SerializeField]
    private UI_Inventory _inventory = null;

    [SerializeField]
    private UI_Inventory _equipment = null;

    [SerializeField]
    private UI_Inventory _spellbook = null;

    [SerializeField]
    private UI_Inventory _activeSpells = null;

    [SerializeField]
    private UI_PlayerPlate[] _allayPlates = null;
    [SerializeField]
    private UI_PlayerPlate[] _enemyPlates = null;
    [SerializeField]
    private Sprite[] _icons = null;

    [SerializeField]
    private UI_Shop _shop = null;
    public UI_Shop shop { get => _shop; }

    [SerializeField]
    private GameObject optionPanel = null;
    [SerializeField]
    private Text aimOption = null;
    [SerializeField]
    private GameObject storePanel = null;
    [SerializeField]
    private GameObject Leaderboard = null;

    private PlayerController _playerController = null;

    [SerializeField]
    private Text xp= null;
    [SerializeField]
    private Text levelupxp = null;
    [SerializeField]
    private Text level = null;
    [SerializeField]
    private InputField Username = null;
    [SerializeField]
    private Text currentUsername = null;

    [SerializeField]
    private GameObject damageText = null;

    [SerializeField]
    private GameObject criticaldamageText = null;

    [SerializeField]
    private GameObject darkessenceText = null;

    [SerializeField]
    private GameObject essenceText = null;

    [SerializeField]
    private GameObject XPtext = null;

    [SerializeField]
    private PlayerStatsUI playerstatsUI = null;

    [SerializeField]
    private GameObject inventoryPanel = null;

    [SerializeField]
    private GameObject equipmentPanel = null;

    [SerializeField]
    private GameObject BossNotification = null;

    [SerializeField]
    private Text playerwhokilledboss = null;

    [SerializeField]
    private GameObject EnterGameNotification = null;

    [SerializeField]
    private Text entergameusername = null;

    private bool canAttack = true;

    private int aimrotationposition = 2;

    private bool abilitiesOnOff = true;

    [SerializeField]
    private Text abilitiesOnOFfText = null;

    [SerializeField]
    private FlyingAbilityArray abilitiesdisplay;

    public Texture2D CursorArrow;

    [SerializeField]
    private GameObject InactivityNotification = null;

    public int width = 48;
    public int height = 48;
    Vector2 mouse;
    Vector2 hotspot = new Vector2(20, 4);
    bool showCursor = false;

    private void OnGUI()
    {
        if(showCursor)
            GUI.DrawTexture(new Rect(mouse.x, mouse.y, width, height), CursorArrow);
    }
    private void Update()
    {
        mouse = new Vector2(Input.mousePosition.x -(2* hotspot.x) + width/2 , Screen.height - Input.mousePosition.y -hotspot.y);
    }
    public void ShowCursor(bool yes)
    {
        showCursor = yes;
    }
    public void Start()
    {
        Cursor.visible = false;
        if (BoltNetwork.IsServer)
            Cursor.visible = true;

        //Vector2 hotspot = new Vector2(20, 4);
        Cursor.SetCursor(CursorArrow, hotspot, CursorMode.Auto);

        Show(false);
    }
    public void ShowInactivityPanel()
    {
        InactivityNotification.SetActive(true);
    }
    public void SetPlayerController(PlayerController player)
    {
        _playerController = player;
        _playerController.ChangeAimingType(aimrotationposition);
        aimOption.text = "Show Direction";
        abilitiesOnOFfText.text = "On";
        abilitiesdisplay.showFlyingAbilities = true;
    }
    public void OpenCloseInventoryPanel()
    {
        if (inventoryPanel.activeSelf)
        {
            OpenCloseInventoryPanel(false);
        }
        else
        {
            OpenCloseInventoryPanel(true);
        }
    }
    public void AimChangeRotation()
    {
        if (aimrotationposition >= 2)
            aimrotationposition = 1;
        else
        {
            aimrotationposition++;
        }

        _playerController.ChangeAimingType(aimrotationposition);
        if (aimrotationposition == 1)
            aimOption.text = "AutoMode";
        else if (aimrotationposition == 2)
            aimOption.text = "Show Direction";
    }
    public void AbilitiesOnCooldownSetOnOff()
    {
        abilitiesOnOff = !abilitiesOnOff;

        if (abilitiesOnOff)
            abilitiesOnOFfText.text = "On";
        else
            abilitiesOnOFfText.text = "Off";

        abilitiesdisplay.showFlyingAbilities = abilitiesOnOff;
    }
    public void OpenCloseInventoryPanel(bool yes)
    {
        inventoryPanel.SetActive(yes);
    }
    public void OpenCloseEquipmentPanel()
    {
        if (equipmentPanel.activeSelf)
        {
            OpenCloseEquipmentPanel(false);
        }
        else
        {
            OpenCloseEquipmentPanel(true);
        }
    }
    public void OpenCloseEquipmentPanel(bool yes)
    {
        equipmentPanel.SetActive(yes);
    }
    public void OpenBossNotificantionPanel(string content)
    {
        BossNotification.SetActive(true);
        playerwhokilledboss.text = content;
        canAttack = false;
        SetAttack();
    }
    public void CloseBossNotificationPanel()
    {
        BossNotification.SetActive(false);
        canAttack = true;
        SetAttack();
    }
    public void OpenEnterGameNotificationPanel(string content)
    {
        EnterGameNotification.SetActive(true);
        entergameusername.text = content;
        canAttack = false;
        SetAttack();
    }
    public void CloseEnterGameNotificationPanel()
    {
        EnterGameNotification.SetActive(false);
        canAttack = true;
        SetAttack();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private void SetAttack()
    {
        _playerController.SetAttackAbility(canAttack);
    }
    public void ShowDamageText(int value, int type, Transform target, Transform player)
    {
        if(type == 0)
        {
            // Debug.Log("Here32");
            MobCombat mobcombat = target.gameObject.GetComponent<MobCombat>();
            GameObject canvas;
            if (mobcombat)
            {
                canvas = mobcombat.GetWorldCanvas();
                SetCanvas();
            }
            else 
            {
                canvas = target.gameObject.GetComponent<PlayerRenderer>().GetWorldCanvas();
                SetCanvas();
            }

            void SetCanvas()
            {
                float distance = Vector3.Distance(canvas.transform.position, _playerController.transform.position);

                float scale = 1f + (distance* 0.08f);
                if (canvas)
                {
                   // Debug.Log("Is Canvas");
                    GameObject instance = Instantiate(damageText, canvas.transform);
                    instance.GetComponent<DamageText>().SetDamageValue(value, scale, type);
                   // instance.GetComponent<Text>().text = value.ToString();
                }
            }
        }
        else if (type == 5)
        {
            MobCombat mobcombat = target.gameObject.GetComponent<MobCombat>();
            GameObject canvas;
            if (mobcombat)
            {
                canvas = mobcombat.GetWorldCanvas();
                SetCanvas();
            }
            else
            {
                canvas = target.gameObject.GetComponent<PlayerRenderer>().GetWorldCanvas();
                SetCanvas();
            }

            void SetCanvas()
            {
                float distance = Vector3.Distance(canvas.transform.position, _playerController.transform.position);

                float scale = 1f + (distance * 0.08f);
                if (canvas)
                {
                    // Debug.Log("Is Canvas");
                    GameObject instance = Instantiate(criticaldamageText, canvas.transform); // insert crit combat text
                    instance.GetComponent<DamageText>().SetDamageValue(value, scale, type);
                    // instance.GetComponent<Text>().text = value.ToString();
                }
            }
        }
        else if (type == 1)
        {
            Vector3 point = new Vector3(target.position.x, target.position.y, target.position.z);
            GameObject instance = Instantiate(darkessenceText, point, Quaternion.identity);
            instance.transform.SetParent(null, true);
            float distance = Vector3.Distance(point, _playerController.transform.position);

            float scale = 1f + (distance * 0.08f);
            instance.GetComponent<DarkEssenceText>().SetDarkEssenceValue(value, scale);
        }
        else if (type == 2)
        {
            Vector3 point = new Vector3(target.position.x, target.position.y, target.position.z);
            GameObject instance = Instantiate(essenceText, point, Quaternion.identity);
            instance.transform.SetParent(null, true);
            instance.transform.position += new Vector3(1f, 0, 0);
            float distance = Vector3.Distance(point, _playerController.transform.position);

            float scale = 1f + (distance * 0.08f);
            instance.GetComponent<DarkEssenceText>().SetDarkEssenceValue(value, scale);
        }
        else if(type == 3)
        {
            Vector3 point = new Vector3(target.position.x, target.position.y, target.position.z);
            GameObject instance = Instantiate(XPtext, point, Quaternion.identity);
            instance.transform.SetParent(null, true);
            instance.transform.position += new Vector3(0, 0.3f, 0);
            float distance = Vector3.Distance(point, _playerController.transform.position);

            float scale = 1f + (distance * 0.08f);
            instance.GetComponent<DarkEssenceText>().SetDarkEssenceValue(value, scale);
        }
    }
    public void UpdateLevelView(int _xp, int _levelupxp, int _level)
    {
        xp.text = _xp.ToString();
        levelupxp.text = _levelupxp.ToString();
        level.text = _level.ToString();

        playerstatsUI.UpdateLevelUI(_xp, _level, _levelupxp);
    }
    public void CloseOptions()
    {
        _playerController.OpenCloseOptions();
    }
    public void CloseShop()
    {
        _playerController.OpenCloseShop();
    }
    public void CloseStore()
    {
        _playerController.OpenCloseStore();
    }
    public void CloseLeaderboard()
    {
        _playerController.OpenCloseLeaderboard();
    }
    public void UpdateUsername(string newusername)
    {
        currentUsername.text = newusername;
    }
    public void ChangeUsernameFunct()
    {
        RaiseChangeUsernameEvent(Username.text);
    }
    public void RaiseChangeUsernameEvent(string newUsername1)
    {
        var evnt = ChangeUsernameEvent.Create(GlobalTargets.OnlyServer);
        evnt.myentity = _playerController.entity;
        evnt.newUsername = newUsername1;
        evnt.Send();
    }

    public void Show(bool active)
    {
       /* if (active)
            _healthBar.gameObject.SetActive(active);
        _skill.gameObject.SetActive(active);
        _grenade.gameObject.SetActive(active);
        _energyCount.transform.parent.gameObject.SetActive(active);
        _ammoPanel.gameObject.SetActive(false); // tha ginei strain kapoia stigmi
        if (_shop.gameObject.activeSelf)
            _shop.gameObject.SetActive(active);*/
        _money.transform.parent.gameObject.SetActive(active);
        _powerEssence.transform.parent.gameObject.SetActive(active);
        //_inventory.gameObject.SetActive(active);
        //_equipment.gameObject.SetActive(active);

    }
    public void UpdateLife(int current, int total)
    {
        _healthBar.UpdateLife(current, total);

        playerstatsUI.UpdateHealthUI(current, total);
    }

    public void UpdateAmmo(int current, int total)
    {
        if (!_ammoPanel.gameObject.activeSelf)
            _ammoPanel.gameObject.SetActive(true);

        _ammoPanel.UpdateAmmo(current, total);
    }

    public void HideAmmo()
    {
        _ammoPanel.gameObject.SetActive(false);
    }

    public void UpdateAbilityView(int i)
    {
        _energyCount.text = i.ToString();
        _skill.UpdateCost(i);
        _grenade.UpdateCost(i);
    }

    public void Flash()
    {
        if (blind != null)
            StopCoroutine(blind);
        blind = StartCoroutine(CRT_Blind(3f));
    }
    public void Death(float a)
    {
        if (death != null)
            StopCoroutine(death);
        death = StartCoroutine(CRT_Dead(a));
    }

    IEnumerator CRT_Blind(float f)
    {
        float startTime = Time.time;
        while (startTime + f > Time.time)
        {
            _blindMask.color = new Color(1, 1, 1, 1);
            yield return null;
            while (startTime + f - 1 < Time.time && startTime + f > Time.time)
            {
                _blindMask.color = new Color(1, 1, 1, -(Time.time - (startTime + f)));
                yield return null;
            }
        }
        _blindMask.color = new Color(1, 1, 1, 0);
    }

    IEnumerator CRT_Dead(float f)
    {
        Debug.Log("HEWEWDSFS");
        float diedTime = Time.time;
        while(diedTime + f > Time.time)
        {
            _deathMask.color = new Color(0, 0, 0, 1);
            yield return null;
            while(diedTime + f - 1 < Time.time && diedTime + f > Time.time)
            {
                _deathMask.color = new Color(0, 0, 0, -(Time.time - (diedTime + f)));
                yield return null;
            }
        }
        _deathMask.color = new Color(0, 0, 0, 0);
    }
    public void UpdatePlayersPlate(GameObject[] players, GameObject localPlayer)
    {
        /*PlayerMotor pm;
        PlayerToken pt;

        if (localPlayer != null)
        {
            pm = localPlayer.GetComponent<PlayerMotor>();
            pt = (PlayerToken)pm.entity.AttachToken;

            _allayPlates[(int)pt.playerSquadID].Init(_icons[(int)pt.characterClass]);
            _allayPlates[(int)pt.playerSquadID].Death(pm.state.IsDead);
        }

        foreach (GameObject p in players)
        {
            pm = p.GetComponent<PlayerMotor>();
            pt = (PlayerToken)pm.entity.AttachToken;

            if (pm.IsEnemy)
            {
                _enemyPlates[(int)pt.playerSquadID].Init(_icons[(int)pt.characterClass]);
                _enemyPlates[(int)pt.playerSquadID].Death(pm.state.IsDead);
            }
            else
            {
                _allayPlates[(int)pt.playerSquadID].Init(_icons[(int)pt.characterClass]);
                _allayPlates[(int)pt.playerSquadID].Death(pm.state.IsDead);
            }
        }*/
    }
    public void ShowOptions(bool yes)
    {
        optionPanel.SetActive(yes);
    }
    public void ShowStore(bool yes)
    {
        storePanel.SetActive(yes);
        OpenCloseInventoryPanel(yes);
        OpenCloseEquipmentPanel(yes);
    }
    public void SetAimingType(int type)
    {
        _playerController.ChangeAimingType(type);
    }
    public void UpdateAimOption(int type)
    {
        if(type == 0)
        {
            aimOption.text = "manual";
        }
        if(type == 1)
        {
            aimOption.text = "auto";
        }
    }
    public void UpdateMoney(int i)
    {
        _money.text =  i.ToString();
        shopmoney.text = i.ToString();
        playerstatsUI.detailstatsarray[10] = i.ToString();
        playerstatsUI.UpdateDetailedStats();
    }

    public void UpdatePowerEssence(int i)
    {
        _powerEssence.text = i.ToString();
        shopEssence.text = i.ToString();
        playerstatsUI.detailstatsarray[11] = i.ToString();
        playerstatsUI.UpdateDetailedStats();
    }
    public void ShowShop(bool show)
    {
        _shop.gameObject.SetActive(show);
    }

    public void UpdateShop(int e, int a)
    {
        _shop.UpdateView(e, a);
    }

    public void UpdateInventoryView(item[] items)
    {
        ServerInventoryItems.text = "";

        for (int i = 0; i < items.Length; i++)
        {
            string name = items[i].GetName();
            string upgrades = "";
            upgrades += " " + items[i].GetUpgradeSlot(0) + ", " + items[i].GetUpgradeSlot(1) + ", " + items[i].GetUpgradeSlot(2) + ", " + items[i].GetUpgradeSlot(3);
            ServerInventoryItems.text += "[slot " + i + ": " + name + "](" + upgrades + ")\n";
        }
    }
    public void UpdateEquipmentView(item[] equip)
    {
        ServerEquipmentItems.text = "";

        for (int i = 0; i < equip.Length; i++)
        {
            string name = equip[i].GetName();
            string upgrades = "";
            upgrades += " " + equip[i].GetUpgradeSlot(0) + ", " + equip[i].GetUpgradeSlot(1) + ", " + equip[i].GetUpgradeSlot(2) + ", " + equip[i].GetUpgradeSlot(3);
            ServerEquipmentItems.text += "[slot " + i + ": " + name + "](" + upgrades + ")\n";
        }
    }

    public void UpdateSpellbookView(ability[] abilities)
    {
        _spellbookAbilities.text = "";

        for(int i=0; i< abilities.Length; i++)
        {
            string name = abilities[i].GetName();
            _spellbookAbilities.text += i + ": " + name + "\n";
        }
    }

    public void UpdateActiveSpellsView(ability[] activeabilities)
    {
        _activeSpellsAbilities.text = "";

        for(int i=0; i< activeabilities.Length; i++)
        {
            string name = activeabilities[i].GetName();
            _activeSpellsAbilities.text += ": "+ name + "\n";
        }
    }
    public void UpdateStatsInfoView(int[] stats)
    {
        for(int i=0; i< stats.Length; i++)
        {
            _StatsInfo[i].text = stats[i].ToString();
        }

        playerstatsUI.UpdatePlayerStatsUI(stats);
    }
    public void ShowLeaderboard(bool yes)
    {
        Leaderboard.SetActive(yes);
    }
    public void StartCooldown(int id)
    {
        playerstatsUI.ShowCastedAbility(id);
    }
    /*public void UpdateLocalInventoryView(item[] items)
    {
        _inventoryItems.text = "";

        for(int i = 0; i<items.Length; i++)
        {
            string name = items[i].GetName();
            //if(i ==0 && b)
            // name = "Crown of a Fallen Kingdom";
            string upgrades = "";
            //for (int h = 0; h < items[i].GetUpgradeArray().Length; h++)
            upgrades += " " + items[i].GetUpgradeSlot(0) + ", " + items[i].GetUpgradeSlot(1) + ", " + items[i].GetUpgradeSlot(2) + ", " + items[i].GetUpgradeSlot(3);
            _inventoryItems.text += "[slot " + i +": " + name + "]("+upgrades+")\n";
        }
    }
    public void UpdateLocalEquipmentView(item[] equipeditems)
    {
        _equipmentItems.text = "";

        for(int i=0; i< equipeditems.Length; i++)
        {
            string name = equipeditems[i].GetName();
            string upgrades = "";
            upgrades += " " + equipeditems[i].GetUpgradeSlot(0) + ", " + equipeditems[i].GetUpgradeSlot(1) + ", " + equipeditems[i].GetUpgradeSlot(2) + ", " + equipeditems[i].GetUpgradeSlot(3);
            _equipmentItems.text += "[slot " + i + ": " + name + "](" + upgrades + ")\n";
        }
    }*/
}
