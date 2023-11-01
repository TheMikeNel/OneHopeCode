using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public int toolLevel = 1;
    private Animator _anim;
    private NavMeshAgent _agent;
    private StationsScript _station;
    private bool _isMove = false;

    private void Update()
    {
        if (_isMove) OnPathCompleted();
    }

    private void OnPathCompleted()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _anim.SetBool("IsRun", false);
            _anim.SetBool("IsWork", true);

            _isMove = false;
            transform.rotation = _station.GetWorkTransform().rotation;
            _station.StationWork(true, toolLevel);
        }
    }

    public void SetStationTarget(StationsScript station)
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _anim.SetBool("IsRun", true);
        _station = station;
        _agent.destination = _station.GetWorkPosition();
        _isMove = true;
    }

}
