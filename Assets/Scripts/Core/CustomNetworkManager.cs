using UnityEngine;
using Mirror;
using UnityEngine.Assertions.Must;

public class CustomNetworkManager : NetworkManager
{
    public GameObject HunterPrefab;
    public Transform HunterSpawnPoint;
    public GameObject[] HiderPrefabList;
    public Transform[] HiderSpawnPointList;

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("Client connected to server.");
        NetworkClient.AddPlayer();
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player;
        if (numPlayers == 0) {
            player = Instantiate(HunterPrefab, HunterSpawnPoint.position, HunterSpawnPoint.rotation);
        } else {
            int randval1 = Random.Range(0, HiderPrefabList.Length);
            GameObject HiderPrefab = HiderPrefabList[randval1];
            // HiderPrefab.GetComponent<CharacterController>().enabled = false;
            // HiderPrefab.GetComponent<Collider>().enabled = false;
            int randval2 = Random.Range(0, HiderSpawnPointList.Length);
            Transform SpawnPoint = HiderSpawnPointList[randval2];
            player = Instantiate(HiderPrefab, SpawnPoint.position, SpawnPoint.rotation);
        }
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
