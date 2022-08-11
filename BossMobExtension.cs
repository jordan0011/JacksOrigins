using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class BossMobExtension : MonoBehaviour
{
    private MobCombat _mobCombat;
    private PowerStats _powerStats;
    // Start is called before the first frame update
    void Start()
    {
        _mobCombat = GetComponent<MobCombat>();
        _powerStats = GetComponent<PowerStats>();
    }

    public void ExtraFunctions()
    {
        if (BoltNetwork.IsServer)
        {
            RaiseKillNotificationEvent();
        }
    }
    public void RaiseKillNotificationEvent()
    {
        //string name = _powerStats.GetPlayersWhoTouched()[0].username;
        string name = _powerStats.GetKiller().username;


        var evnt = BossKilledEvent.Create(GlobalTargets.AllClients);

        if(name.Length >= 140)
        {
            char[] name1 = "".ToCharArray();
            for(int i=0; i< 139; i++)
            {
                name1[i] = name[i];
            }
            char[] name2 = "".ToCharArray();
            for (int i = 140; i< 289; i++)
            {
                name2[i] = name[i];
            }
            string newname1 = new string(name1);
            evnt.player = newname1;
            string newname2 = new string(name2);
            evnt.player2 = newname2;
        }
        else
        {
            evnt.player = name;
        }

        evnt.Send();
    }
}
