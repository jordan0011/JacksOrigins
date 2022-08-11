[Photon.Bolt.BoltGlobalBehaviour]
public class NetworkCallbacks : Photon.Bolt.GlobalEventListener
{
    public override void BoltStartBegin()
    {
        Photon.Bolt.BoltNetwork.RegisterTokenClass<Photon.Bolt.PhotonRoomProperties>();
        Photon.Bolt.BoltNetwork.RegisterTokenClass<WeaponDropToken>();
        Photon.Bolt.BoltNetwork.RegisterTokenClass<PlayerToken>();
    }
    public override void SceneLoadLocalDone(string scene, Photon.Bolt.IProtocolToken token)
    {
        if (Photon.Bolt.BoltNetwork.IsServer)
        {
            if (scene == HeadlessServerManager.Map())
            {
                if (!GameController.Current)
                    Photon.Bolt.BoltNetwork.Instantiate(Photon.Bolt.BoltPrefabs.GameController);
            }
        }
    }
}
