using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] public int toolLevel = 1;
    [SerializeField] private GameObject moveTargetCursor;
    [SerializeField, Range(0f, 10f)] private float selectedOutlineWidth = 2f;
    [SerializeField] private LayerMask RaycastLayers;

    private Animator _anim;
    private NavMeshAgent _agent;
    private Outline _currentOutline;
    private StationsScript _currentStation;

    private bool _justRun =false;
    private bool _outlineActive = false; // Переменная для активации / деактивации внешней обводки объектов при наведении / снятии курсора.
    private bool _runToStation = false;  // Переменная, необходимая для отслеживания достижения игроком рабочего места какой-либо станции.

    void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        OnPathCompleted();
        RaycastOutline();

        if (Input.GetMouseButtonDown(1))
        {
            MouseSelectPosition();
        }
    }
    /// <summary>
    /// Выделение объектов, на которые наведен курсор, внешними линиями.
    /// </summary>
    private void RaycastOutline()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 150, RaycastLayers))
        {

            if (hit.collider.GetComponent<Outline>() != null)
            {
                if (!_outlineActive)
                {
                    _currentOutline = hit.collider.GetComponent<Outline>();
                    SetOutline(true);
                }
            }

            else
            {
                if (_currentOutline != null)
                    SetOutline(false);
            }
        }
    }

    /// <summary>
    /// Выбор, с помощью нажатия ПКМ, позиции следования игрока
    /// </summary>
    private void MouseSelectPosition()
    {
        Ray movingRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit movingRayHit;

        if (Physics.Raycast(movingRay, out movingRayHit, 150, RaycastLayers))
        {
            switch (movingRayHit.transform.gameObject.tag) // Проверка позиции, к которой должен пойти игрок
            {
                case "Ground": // Если игрок выбрал перемещение по земле, происходит его перемещение без каких-либо дополнительных функций
                    _justRun = true;
                    JustMoving(movingRayHit.point);
                    break;
                case "Station": // Если игрок выбрал перемещение к какой-либо постройке, выполняется метод перемещения к позиции рабочего места данной постройки
                    _justRun = true;
                    MovingToStations(movingRayHit.collider.gameObject);
                    break; 
            }
        }
    }

    /// <summary>
    /// Движение игрока по земле (выключается работа игрока с какой-либо постройкой)
    /// </summary>
    /// <param name="target"></param>
    private void JustMoving(Vector3 target)
    {
        _justRun = true;
        _runToStation = false;

        if (_currentStation != null) _currentStation.StationWork(false, toolLevel);

        _agent.destination = target;
        _anim.SetBool("IsWork", false);
        _anim.SetBool("IsRun", true);
    }

    private void MovingToStations(GameObject station)
    {
        if (_currentStation != null) _currentStation.StationWork(false, toolLevel);

        _currentStation = station.GetComponent<StationsScript>();

        if (_currentStation != null)
        {
            _agent.destination = _currentStation.GetWorkPosition();
            _runToStation = true;
            _anim.SetBool("IsWork", false);
            _anim.SetBool("IsRun", true);
        }
    }

    private void OnPathCompleted()
    {
        if ((_justRun || _runToStation) && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (_runToStation)
            {
                _currentStation.StationWork(true, toolLevel);
                _runToStation = false;
                _anim.SetBool("IsRun", false);
                _anim.SetBool("IsWork", true);
            }

            if (_justRun)
            {
                _justRun = false;
                _anim.SetBool("IsRun", false);
            }
        }
    }

    private void SetOutline(bool setting)
    {
        if (setting)
        {
            _outlineActive = true;
            _currentOutline.OutlineWidth = selectedOutlineWidth;
        }

        else
        {
            _outlineActive = false;
            _currentOutline.OutlineWidth = 0;
            _currentOutline = null;
        }
    }
}
