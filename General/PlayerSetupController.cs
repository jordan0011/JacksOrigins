using UnityEngine;
using Photon.Bolt;
using UnityEngine.UI;

public class PlayerSetupController : Photon.Bolt.GlobalEventListener
{
    [SerializeField]
    private Camera _SceneCamera;

    [SerializeField]
    private GameObject _teamSelector = null;
    [SerializeField]
    private GameObject _classSelector = null;

    private int _TTCount = 0;
    private int _ATCount = 0;

    [SerializeField]
    private Text _TTCountText = null;
    [SerializeField]
    private Text _ATCountText = null;

    [SerializeField]
    private Button _TTButton = null;
    [SerializeField]
    private Button _ATButton = null;

    public Camera SceneCamera { get => _SceneCamera; }

    private Team _selectedTeam = Team.AT;
    private PlayerSquadID _playerSquad;
    private System.Guid _eventID = System.Guid.Empty;

    [SerializeField]
    private Text _soldierCount = null;
    [SerializeField]
    private Text _medicCount = null;
    [SerializeField]
    private Text _heavyCount = null;

    [SerializeField]
    private Transform _TTBase = null;
    [SerializeField]
    private Transform _ATBase = null;

    private bool[] playerIds = new bool[200];

    private bool hasplayer = false;
    private bool hasplayer2 = false;

    
    public void Awake()
    {
        for (int i = 0; i < playerIds.Length; i++)
        {
            playerIds[i] = false;
        }
        // CursorMode mode = CursorMode.ForceSoftware;
        //   tex.width = tex.width / 2;
        //  tex.height = tex.height / 2;
        //tex.Resize(tex.width / 2, tex.height / 2);
        // Graphics.CopyTexture(CursorArrow, tex);
        // tex.Apply();
        // CursorArrow.height = CursorArrow.height / 2;
        //Debug.Log("Cursor Heaight: " + CursorArrow.height);
        //Vector2 hotspot = new Vector2(20, 4);

        // colors used to tint the first 3 mip levels
        /* Color[] colors = new Color[3];
         colors[0] = Color.red;
         colors[1] = Color.green;
         colors[2] = Color.blue;
         int mipCount = Mathf.Min(3, CursorArrow.mipmapCount);

         // tint each mip level
         for (int mip = 0; mip < mipCount; ++mip)
         {
             Color[] cols = CursorArrow.GetPixels(mip);
             for (int i = 0; i < cols.Length; ++i)
             {
                 cols[i] = Color.Lerp(cols[i], colors[mip], 0.33f);
             }
             CursorArrow.SetPixels(cols, mip);
         }
         // actually apply all SetPixels, don't recalculate mip levels
         CursorArrow.Apply(false);*/

        //Vector2 hotspot = new Vector2(CursorArrow.width / 2f, CursorArrow.height / 2f);
        //Vector2 hotSpot = new Vector2(xspot, yspot);
        // Cursor.SetCursor(CursorArrow, hotspot, mode);
        // Cursor.SetCursor(CursorArrow, hotspot, CursorMode.Auto);

       
    }

   // public override void 

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        if (!BoltNetwork.IsServer)
        {
            // _teamSelector.SetActive(true);
            SelectTeamEvent();
        }
    }
    public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
    {
        if (BoltNetwork.IsServer)
        {
            UpdateTeamCountEvent evnt = UpdateTeamCountEvent.Create(connection, ReliabilityModes.ReliableOrdered);
            evnt.AT = _ATCount;
            evnt.TT = _TTCount;
            evnt.Send();
        }
    }

    public override void OnEvent(UpdateTeamCountEvent evnt)
    {
        /* _ATCount = evnt.AT;
         _TTCount = evnt.TT;

         _TTCountText.text = _TTCount.ToString();
         _ATCountText.text = _ATCount.ToString();

         UpdateTeamButtons();*/
        if (!hasplayer)
        {
            SetTeam(1);
            hasplayer = true;
        }
    }
    private void UpdateTeamButtons()
    {
        if (_TTCount == _ATCount)
        {
            _ATButton.interactable = true;
            _TTButton.interactable = true;
        }
        else
        {
            _ATButton.interactable = true;
            _TTButton.interactable = true;
        }
    }

    public void SetTeam(int t)
    {
        _ATButton.interactable = false;
        _TTButton.interactable = false;
        _selectedTeam = (Team)t;
        ChooseTeamEvent evnt = ChooseTeamEvent.Create(ReliabilityModes.ReliableOrdered);
        evnt.Team = t;
        _eventID = System.Guid.NewGuid();
        evnt.ID = _eventID;
        evnt.Send();
    }

    public override void OnEvent(ChooseTeamEvent evnt)
    {
        if (BoltNetwork.IsServer)
        {
            bool accepted = true;

            /*if ((Team)evnt.Team == Team.AT)
            {
                if (_ATCount > _TTCount)
                    accepted = false;
            }
            else
            {
                if (_ATCount < _TTCount)
                    accepted = false;
            }*/

            ConfirmTeamEvent evntT = ConfirmTeamEvent.Create(ReliabilityModes.ReliableOrdered);
            evntT.Team = evnt.Team;
            evntT.Accepted = accepted;
            evntT.ID = evnt.ID;

            if ((Team)evnt.Team == Team.AT)
                evntT.SquadID = _ATCount;
            else
                evntT.SquadID = _TTCount;

            evntT.Send();

            if (accepted)
                AddTeamCount((Team)evnt.Team);
        }
    }
    public void AddTeamCount(Team t)
    {
        if (t == Team.TT)
            _TTCount++;
        else
            _ATCount++;

        _TTCountText.text = _TTCount.ToString();
        _ATCountText.text = _ATCount.ToString();

        UpdateTeamButtons();
    }
    public override void OnEvent(ConfirmTeamEvent evnt)
    {
        
        if (evnt.Accepted)
        {
            if (_eventID == evnt.ID)
            {
                //_teamSelector.SetActive(false);
                SelectClassEvent();
                _playerSquad = (PlayerSquadID)evnt.SquadID;
            }

            if (BoltNetwork.IsClient)
                AddTeamCount((Team)evnt.Team);
        }
        else
        {
            UpdateTeamButtons();
        }

        UpdateClassView();
        if (!hasplayer2)
        {
                RaiseSpawnPlayerEvent(2);
            hasplayer2 = true;
        }
    }

    public void SelectClassEvent()
    {
        Debug.Log("Select Class Event");
    }
    public void SelectTeamEvent()
    {
        Debug.Log("Select Team Event (Client)");
    }
    public void UpdateClassView()
    {
        int s = 0;
        int m = 0;
        int h = 0;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            PlayerToken token = (PlayerToken)p.GetComponent<PlayerMotor>().entity.AttachToken;
            if (token.team == _selectedTeam)
            {
                if (token.characterClass == CharacterClass.Soldier)
                    s++;
                else if (token.characterClass == CharacterClass.Heavy)
                    h++;
                else if (token.characterClass == CharacterClass.Medic)
                    m++;
            }
        }

        _heavyCount.text = h.ToString();
        _medicCount.text = m.ToString();
        _soldierCount.text = s.ToString();
    }

    public void RaiseSpawnPlayerEvent(int index)
    {
        SpawnPlayerEvent spawn = SpawnPlayerEvent.Create(Photon.Bolt.GlobalTargets.OnlyServer);
        spawn.PlayerName = AppManager.Current.Username;
        spawn.Team = (short)_selectedTeam;
        spawn.Class = index;
        spawn.SquadID = (short)_playerSquad;
        spawn.Send();
    }
    public override void OnEvent(SpawnPlayerEvent evnt)
    {
        var token = new PlayerToken();
        token.team = (Team)evnt.Team;
        token.name = evnt.PlayerName;
        token.playerSquadID = (PlayerSquadID)evnt.SquadID;
        token.characterClass = (CharacterClass)evnt.Class;

        Vector3 v = Vector3.zero;

        v.y += 9;

        if (token.team == Team.TT)
        {
            v = _TTBase.transform.position;
            v.x += Random.Range(-_TTBase.localScale.x / 2f, _TTBase.localScale.x / 2f);
            v.z += Random.Range(-_TTBase.localScale.z / 2f, _TTBase.localScale.z / 2f);
        }
        else
        {
            v = _ATBase.transform.position;
            v.x += Random.Range(-_ATBase.localScale.x / 2f, _ATBase.localScale.x / 2f);
            v.z += Random.Range(-_ATBase.localScale.z / 2f, _ATBase.localScale.z / 2f);
        }

        BoltEntity entity;
        switch ((CharacterClass)evnt.Class)
        {
            case CharacterClass.Soldier:
                entity = BoltNetwork.Instantiate(BoltPrefabs.Soldier, token, v, Quaternion.identity);
                break;
            case CharacterClass.Medic:
                entity = BoltNetwork.Instantiate(BoltPrefabs.Medic, token, v, Quaternion.identity);
                break;
            default:
                entity = BoltNetwork.Instantiate(BoltPrefabs.Heavy, token, v, Quaternion.identity);
                break;
        }
        entity.AssignControl(evnt.RaisedBy);
    }

    public Vector3 GetSpawnPoint(Team teamSide)
    {
        Vector3 v = Vector3.zero;

        v.y += 9;

        if (teamSide == Team.TT)
        {
            v = _TTBase.transform.position;
            v.x += Random.Range(-_TTBase.localScale.x / 2f, _TTBase.localScale.x / 2f);
            v.z += Random.Range(-_TTBase.localScale.z / 2f, _TTBase.localScale.z / 2f);
        }
        else
        {
            v = _ATBase.transform.position;
            v.x += Random.Range(-_ATBase.localScale.x / 2f, _ATBase.localScale.x / 2f);
            v.z += Random.Range(-_ATBase.localScale.z / 2f, _ATBase.localScale.z / 2f);
        }
        return v;
    }
}