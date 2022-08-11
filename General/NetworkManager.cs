using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit;
using System;

public class NetworkManager : Photon.Bolt.GlobalEventListener
{
    [SerializeField]
    private UnityEngine.UI.Text feedback;
    [SerializeField]
    private UnityEngine.UI.InputField username;

    private void Awake()
    {
        username.text = AppManager.Current.Username;
    }
    public void FeedbackUser(string text)
    {
        feedback.text = text;
    }

    public void Connect()
    {
        if (username.text != "")
        {
            AppManager.Current.Username = username.text;
            BoltLauncher.StartClient();
            FeedbackUser("Connecting ...");
        }
        else
            FeedbackUser("Enter a valid name");
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        FeedbackUser("Searching ...");
        BoltMatchmaking.JoinSession(HeadlessServerManager.RoomID());
    }

    public override void Connected(BoltConnection connection)
    {
        FeedbackUser("Connected !");
    }
}
