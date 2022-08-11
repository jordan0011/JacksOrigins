using UnityEngine;
using Photon.Bolt;
using System.Collections;

public class PlayerCallback : EntityEventListener<INewPlayerState>
{
    private PlayerMotor _playerMotor;
    private PlayerWeapons _playerWeapons;
    private PlayerController _playerController;
    private PlayerRenderer _playerRenderer;
    private PlayerInventory _playerInventory;
    private PlayerSpellbook _playerSpellbook;
    private PowerStats _powerStats;

    private PlayerDamageSystem _playerDamageSystem;
    private PlayerWorldUIStats _playerWorldUI;

    private float deadTimer = 5f;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();
        _playerWeapons = GetComponent<PlayerWeapons>();
        _playerController = GetComponent<PlayerController>();
        _playerRenderer = GetComponent<PlayerRenderer>();
        _playerInventory = GetComponent<PlayerInventory>();
        _playerSpellbook = GetComponent<PlayerSpellbook>();

        _playerDamageSystem = GetComponent<PlayerDamageSystem>();
        _playerWorldUI = GetComponent<PlayerWorldUIStats>();
        _powerStats = GetComponent<PowerStats>();
    }

    public override void Attached()
    {
        state.AddCallback("LifePoints", UpdatePlayerLife);
        state.AddCallback("IsDead", UpdateDeathState);
        state.AddCallback("Money", UpdateMoney);
        state.AddCallback("PowerEssence", UpdatePowerEssence);
        state.AddCallback("Items[]", UpdateItemState);
        state.AddCallback("Items[].Upgrade[]", UpdateItemUpgradeState);
        state.AddCallback("equiped[]", UpdateEquipedState);
        state.AddCallback("equiped[].Upgrade[]", UpdateEquipedUpgradeState);
        state.AddCallback("spellbook[]", UpdateSpellbookState);
        state.AddCallback("activeSpells[]", UpdateActiveSpellsState);
        state.AddCallback("Stats[]", UpdateStats);
        state.AddCallback("myLevel", UpdateLevel);
        state.AddCallback("myXp", UpdateXP);
        state.AddCallback("username", UpdateUsername);

        if (entity.IsOwner) // variable assinging!!!
        {
            state.IsDead = false;
            state.LifePoints = _playerMotor.TotalLife;
            state.TotalLifePoints = _playerMotor.TotalLife;
            state.Money = 5;
            state.PowerEssence = 0;


            for(int i=0; i< state.spellbook.Length; i++)
            {
                state.spellbook[i].abilityID = -1;
            }

            for(int i=0; i<state.activeSpells.Length; i++)
            {
                state.activeSpells[i].abilityID = -1;
            }

            GameController.Current.state.AlivePlayers++;
        }
    }
    public void UpdateUsername()
    {
        _playerWorldUI.UsernameView(state.username);
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdateUsername(state.username);
        }
    }
    public void UpdateLevel()
    {
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdateLevelView(state.myXp, LevelStatList.V1.playerlevelupxp[state.myLevel -1], state.myLevel);
        }

        _playerWorldUI.LevelViewWorld(state.myLevel);
    }
    public void UpdateXP()
    {
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdateLevelView(state.myXp, LevelStatList.V1.playerlevelupxp[state.myLevel -1], state.myLevel);
        }
    }
    public void UpdateStats()
    {
        if (entity.HasControl)
        {
            int[] stats = new int[9];
            for (int i = 0; i < stats.Length; i++)
            {
                int s = state.Stats[i];
                stats[i] = s;
            }
            GUI_Controller.Current.UpdateStatsInfoView(stats);
        }
    }
    public void UpdateItemState()
    {
        if (entity.HasControl)
        {
            _playerInventory.UpdateInventoryViaServer();
        }
    }
    public void UpdateSpellbookState()
    {
        if (entity.HasControl)
        {
            _playerSpellbook.UpdateSpellbookViaServer();
        }
    }
    public void UpdateActiveSpellsState()
    {
        if (entity.HasControl)
        {
            _playerSpellbook.UpdateActiveSpellsViaServer();
        }
    }
    public void UpdateItemUpgradeState()
    {
        if (entity.HasControl)
        {
            _playerInventory.DeleteItem(0);

            _playerInventory.UpdateInventoryViaServer();
            
        }
    }
    public void UpdateEquipedState()
    {

        if (entity.HasControl)
        {
            _playerInventory.UpdateEquipmentViaServer();
        }
        if (state.equiped[0].itemID == -1)
        {
            _playerWeapons.DeleteWeapon(0);
            _playerWeapons.prefabId = -1;
        }

        if (_playerWeapons.prefabId != state.equiped[0].itemID && state.equiped[0].itemID > -1)
        {
            _playerWeapons.UpdateWeapon(state.equiped[0].itemID, 0);
            _playerWeapons.prefabId = state.equiped[0].itemID;
        }

        if (state.equiped[1].itemID == -1)
        {
            _playerWeapons.DeleteWeapon(1);
            _playerWeapons.prefabid2 = -1;
        }

        if (_playerWeapons.prefabid2 != state.equiped[1].itemID && state.equiped[1].itemID > -1)
        {
            _playerWeapons.UpdateWeapon(state.equiped[1].itemID, 1);
            _playerWeapons.prefabid2 = state.equiped[1].itemID;
        }
    }
    public void UpdateEquipedUpgradeState()
    {
        if (entity.HasControl)
        {
            _playerInventory.UpdateEquipmentViaServer();
        }
    }
    public void RaiseChangeFocusEvent(GameObject enemy)
    {
        ChangeFocusEvent evnt = ChangeFocusEvent.Create(entity, EntityTargets.Everyone);
        evnt.target = enemy.GetComponent<BoltEntity>(); // mporei na to kanw Vector3
        evnt.Send();
    }
    public override void OnEvent(ChangeFocusEvent evnt)
    {
        // allakse to focus sto enemy pou exei to entity;
    }
    public void FireEffect(float precision, int seed)
    {
        FireEffectEvent evnt = FireEffectEvent.Create(entity, EntityTargets.EveryoneExceptOwner);
        evnt.Precision = precision;
        evnt.Seed = seed;
        evnt.Send();
    }

    public override void OnEvent(FireEffectEvent evnt)
    {
        //_playerWeapons.FireEffect(evnt.Seed, evnt.Precision);
       // _playerDamageSystem.FireEffect(evnt.Seed, evnt.)
    }
    public void BasicAttackEffect(int attackid,int seed)
    {
        BasicAttackEvent evnt = BasicAttackEvent.Create(entity, EntityTargets.EveryoneExceptOwner);
        evnt.BattackId = attackid;
        evnt.Seed = seed;
        evnt.Send();
    }
    public override void OnEvent(BasicAttackEvent evnt)
    {
        _playerDamageSystem.FireEffect(evnt.Seed, evnt.BattackId, transform.position, transform.rotation);
    }
    public void RaiseAttackFromSummoningEvent(int attackid)
    {
        AttackFromSummoningEvent evnt = AttackFromSummoningEvent.Create(entity, EntityTargets.EveryoneExceptOwner);
        evnt.attackVfxid = attackid;
        evnt.Send();
    }
    public override void OnEvent(AttackFromSummoningEvent evnt)
    {
        _playerDamageSystem.FireEffectFromSummoning(evnt.attackVfxid);
    }
    public void UpdatePlayerLife()
    {
        if (entity.HasControl)
            GUI_Controller.Current.UpdateLife(state.LifePoints, state.TotalLifePoints);
    }

    public void UpdateEnergy()
    {
        if (entity.HasControl)
        {
            //GUI_Controller.Current.UpdateAbilityView(state.Energy);
            //GUI_Controller.Current.UpdateShop(state.Energy, state.Money);
        }
    }
    public void RaiseFlashEvent()
    {
        FlashEvent evnt = FlashEvent.Create(entity, EntityTargets.OnlyController);
        evnt.Send();
    }
    public override void OnEvent(FlashEvent evnt)
    {
        GUI_Controller.Current.Flash();
    }

    private void UpdateDeathState()
    {
        //if (entity.HasControl)
           // GUI_Controller.Current.Show(false);

        _playerMotor.OnDeath(state.IsDead);
        _playerRenderer.OnDeath(state.IsDead);
        if(state.IsDead && entity.HasControl)
            GUI_Controller.Current.Death(deadTimer + 1);
       // _playerWeapons.OnDeath(state.IsDead);

        if (entity.IsOwner && state.IsDead)
            StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
       // yield return new WaitForSeconds(1f);
       // GameController.Current.state.AlivePlayers--;
        yield return new WaitForSeconds(deadTimer);
        state.IsDead = false;
        //state.LifePoints = _playerMotor.TotalLife;
        state.SetTeleport(state.newTransform);
        PlayerToken token = (PlayerToken)entity.AttachToken;
        transform.position = FindObjectOfType<PlayerSetupController>().GetSpawnPoint(token.team);
        if (entity.HasControl)
            GUI_Controller.Current.Show(true);
      //  yield return new WaitForSeconds(1f);
      //  GameController.Current.state.AlivePlayers++;
    }
    private void UpdateMoney()
    {
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdateMoney(state.Money);
            //GUI_Controller.Current.UpdateShop(state.Energy, state.Money);
        }
    }
    private void UpdatePowerEssence()
    {
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdatePowerEssence(state.PowerEssence);
            // to be added;
        }
    }
    public void Update()
    {
        if (entity.HasControl)
        {
            GUI_Controller.Current.UpdateMoney(state.Money);

            GUI_Controller.Current.UpdatePowerEssence(state.PowerEssence);
        }
    }

    public void RaiseBuyWeaponEvent(int index)
    {
        BuyWeaponEvent evnt = BuyWeaponEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.index = index;
        evnt.Send();
    }

    public override void OnEvent(BuyWeaponEvent evnt)
    {
        if (state.Money >= GUI_Controller.Current.shop.ItemCost(evnt.index))
        {
            state.Money -= GUI_Controller.Current.shop.ItemCost(evnt.index);
           // GetComponent<PlayerWeapons>().AddWeaponEvent(GUI_Controller.Current.shop.ItemID(evnt.index));
        }
    }

    public void RaiseBuyEnergyEvent()
    {
        BuyEnergyEvent evnt = BuyEnergyEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.Send();
    }

    public override void OnEvent(BuyEnergyEvent evnt)
    {
        if (state.Money >= GUI_Controller.Current.shop.EnergyCost())
        {
            state.Money -= GUI_Controller.Current.shop.EnergyCost();
            //state.Energy++;
        }
    }
    public void RaiseAddItemEvent(int a)
    {
        AddItemEvent evnt = AddItemEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.ID = a;
        evnt.Send();
    }
    public override void OnEvent(AddItemEvent evnt)
    {
        _playerInventory.InventoryAlteration(0, evnt.ID, -1);
    }
    
    public void RaiseDeleteItemEvent(int d)
    {
        DeleteItemEvent evnt = DeleteItemEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.position = d;
        evnt.Send();
    }
    public override void OnEvent(DeleteItemEvent evnt)
    {
        _playerInventory.InventoryAlteration(1, evnt.position, -1);
    }
    public void RaiseAddUpgradetoitemEvent(int a)
    {
        AddUpgradeToItemEvent evnt = AddUpgradeToItemEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.UpgradeID = a;
        // need int for pointer
        evnt.Send();
    }
    public override void OnEvent(AddUpgradeToItemEvent evnt)
    {
        /*int upgradeId = evnt.UpgradeID;

        int pos = -1;
        for(int i = 0; i < state.Items[0].Upgrade.Length; i++)
        {
            if (state.Items[0].Upgrade[i] == 0 && pos == -1)
                pos = i;
        }

        if(pos > -1)
        {
            state.Items[0].Upgrade[pos] = upgradeId;
        }*/

    }
    public void RaiseAddUpgradeSlotToItemEvent()
    {
        AddUpgradeSlotToItemEvent evnt = AddUpgradeSlotToItemEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.Send();
    }
    public override void OnEvent(AddUpgradeSlotToItemEvent evnt)
    {
        _playerInventory.InventoryAlteration(3, 0, -1);
    }
    public void RaiseSwitchItemPositionsEvent(int pos1, int pos2)
    {
        SwitchItemPositionsEvent evnt = SwitchItemPositionsEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.Pos1 = pos1;
        evnt.Pos2 = pos2;
        evnt.Send();
    }
    public override void OnEvent(SwitchItemPositionsEvent evnt)
    {
        _playerInventory.InventoryAlteration(2, evnt.Pos1, evnt.Pos2);
    }
    public void RaiseEquipItemEvent(int pos1, int pos2)
    {
        EquipItemEvent evnt = EquipItemEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.From = pos1;
        evnt.To = pos2;
        evnt.Send();
    }
    public override void OnEvent(EquipItemEvent evnt)
    {
        _playerInventory.InventoryAlteration(5, evnt.From, evnt.To);
    }
    public void RaiseUnequipItemEvent(int a)
    {
        UnequipItemEvent evnt = UnequipItemEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.From = a;
        evnt.Send();
    }
    public override void OnEvent(UnequipItemEvent evnt)
    {
        _playerInventory.InventoryAlteration(6, evnt.From, -1);
    }

    public void RaiseStore0BuyEvent(int a)
    {
        Store0BuyItemEvent evnt = Store0BuyItemEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.itemposition = a;
        evnt.secretitempos = -1;
        evnt.Send();
    }

    public override void OnEvent(Store0BuyItemEvent evnt)
    {
        if (BoltNetwork.IsServer)
        {
            if(transform.position.x >= -30 && transform.position.x <= 0 && transform.position.y >= -25 && transform.position.y <= -5 && transform.position.z >= -100 && transform.position.z <= -60)
            {
                if(_powerStats.GetMoney() >=ItemList.Items.store0Price[evnt.itemposition].GetPowerEssence() && _powerStats.GetPoweressence() >=ItemList.Items.store0Price[evnt.itemposition].GetDarkPowerEssence())
                {
                    _powerStats.SetEssence(-ItemList.Items.store0Price[evnt.itemposition].GetPowerEssence(), -ItemList.Items.store0Price[evnt.itemposition].GetDarkPowerEssence());
                    _playerInventory.InventoryAlteration(0, evnt.itemposition, -1);

                }
            }
        }
    }



    public void RaiseAddAbilityEvent(int a)
    {
        AddAbilityEvent evnt = AddAbilityEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.abilityId = a;
        evnt.Send();
    }
    public override void OnEvent(AddAbilityEvent evnt)
    {
        int pos = -1;
        for (int i = 0; i < state.spellbook.Length; i++)
        {
            if (state.spellbook[i].abilityID == -1 && pos == -1)
                pos = i;
        }

        if (pos > -1)
        {
            state.spellbook[pos].abilityID= evnt.abilityId;
        }

        _playerSpellbook.UpdateSpellbookViaServer();
    }


    public void RaiseDeleteAbilityEvent(int a)
    {
        DeleteAbilityEvent evnt = DeleteAbilityEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.pos = a;
        evnt.Send();
    }
    public override void OnEvent(DeleteAbilityEvent evnt)
    {
        state.spellbook[evnt.pos].abilityID = -1;

        _playerSpellbook.UpdateSpellbookViaServer();
    }
    public void RaiseActivateAbilityEvent(int a, int b)
    {
        ActivateAbilityEvent evnt = ActivateAbilityEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.from = a;
        evnt.to = b;
        evnt.Send();
    }
    public override void OnEvent(ActivateAbilityEvent evnt)
    {
        int spellbookfrom = evnt.from;
        int activePos = evnt.to;
        if (state.spellbook[spellbookfrom].abilityID != -1 && state.activeSpells[activePos].abilityID == -1)
        {
           // itemCategory category = ItemList.Items.itemInfo[state.Items[invfrom].itemID].getCategory();
           // if (category == itemCategory.Weapon)
            //{
                state.activeSpells[activePos].abilityID = state.spellbook[spellbookfrom].abilityID;

                state.spellbook[spellbookfrom].abilityID = -1;
           // }
        }
        if (state.spellbook[spellbookfrom].abilityID != -1 && state.activeSpells[activePos].abilityID > -1) // an exei idi item
        {
            //itemCategory category = ItemList.Items.itemInfo[state.Items[invfrom].itemID].getCategory();
            //if (category == itemCategory.Weapon)
            //{
                int tempabilityid;

                tempabilityid = state.activeSpells[activePos].abilityID;

                state.activeSpells[activePos].abilityID = state.spellbook[spellbookfrom].abilityID;

                state.spellbook[spellbookfrom].abilityID = tempabilityid;
            //}
        }

        _playerSpellbook.UpdateActiveSpellsViaServer();
        _playerSpellbook.UpdateSpellbookViaServer();
    }
    public void RaiseDeactivateAbilityEvent(int a)
    {
        DeactivateAbilityEvent evnt= DeactivateAbilityEvent.Create(entity, EntityTargets.OnlyOwner);
        evnt.from = a;
        evnt.Send();
    }

    public override void OnEvent(DeactivateAbilityEvent evnt)
    {
        int activefrom = evnt.from;

        int pos = -1;
        for (int i = 0; i < state.spellbook.Length; i++)
        {
            if (state.spellbook[i].abilityID == -1 && pos == -1)
                pos = i;
        }

        if (pos > -1)
        {
            if (state.activeSpells[activefrom].abilityID != -1)
            {
                state.spellbook[pos].abilityID = state.activeSpells[activefrom].abilityID;

                state.activeSpells[activefrom].abilityID = -1;
            }
        }

        _playerSpellbook.UpdateActiveSpellsViaServer();
        _playerSpellbook.UpdateSpellbookViaServer();
    }
}
