using UnityEngine;
using Photon.Bolt;

public class GameController : EntityEventListener<IGameModeState>
{
    #region Singleton
    private static GameController _instance = null;

    public static GameController Current
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameController>();

            return _instance;
        }
    }
    #endregion

    int _playerCountTarget = 4;

    private GameObject _localPlayer = null;
    public GameObject localPlayer
    {
        get => _localPlayer;
        set => _localPlayer = value;
    }

    public override void Attached()
    {
        state.AddCallback("AlivePlayers", UpdatePlayersAlive);
    }
    public void UpdatePlayersAlive()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject lp = GameObject.FindGameObjectWithTag("LocalPlayer");
       // GUI_Controller.Current.UpdatePlayersPlate(players, lp);
    }
}
