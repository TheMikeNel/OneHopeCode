using UnityEngine;

public class NPCSpawn : MonoBehaviour
{
    [SerializeField] private PlayerController playerToolLevel;
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform spawnPosition;

    private NPCController _controller;

    public void Spawn(StationsScript addToStation)
    {
        _controller = Instantiate(npcPrefab, spawnPosition.position, Quaternion.identity).GetComponent<NPCController>();
        _controller.toolLevel = playerToolLevel.toolLevel;
        _controller.SetStationTarget(addToStation);
    }
}
