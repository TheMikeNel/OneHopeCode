using UnityEngine;

public class NPCSpawn : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform spawnPosition;

    private GameObject _currentNPC;
    private NPCController _controller;

    public void Spawn(StationsScript addToStation)
    {
        _currentNPC = Instantiate(npcPrefab, spawnPosition.position, Quaternion.identity);
        _controller =  _currentNPC.GetComponent<NPCController>();
        _controller.SetStationTarget(addToStation);
    }
}
