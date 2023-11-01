using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public int toolLevel = 1;

    private Animator _anim;
    private NavMeshAgent _agent;
    private StationsScript _station;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        OnPathCompleted();
    }

    private void OnPathCompleted()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _anim.SetBool("IsRun", false);
            _anim.SetBool("IsWork", true);

            transform.rotation = _station.GetWorkTransform().rotation;           
        }
    }

    public void SetStationTarget(StationsScript station)
    {
        _anim.SetBool("IsRun", true);
        _station = station;
        _agent.destination = _station.GetWorkPosition();
    }

}
