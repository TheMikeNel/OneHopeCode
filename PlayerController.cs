using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    public int toolLevel = 1;
    public GameObject toolAnchor;
    public GameObject[] toolsOnLevelPrefabs = new GameObject[4];

    [SerializeField, Tooltip("3D курсор направления движения (при надобности)")] private GameObject moveTargetCursor;
    [SerializeField, Range(0f, 10f)] private float selectedOutlineWidth = 2f;
    [SerializeField] private LayerMask RaycastLayers;

    [Space]
    [Header("UI Tooltips Settings")]
    [SerializeField] private RectTransform tooltipOnUI;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private GameObject[] stations;
    [SerializeField] private string[] selectedStationTooltipText;

    private bool _canMove = true;
    private string _selectedStation;
    private Animator _anim;
    private NavMeshAgent _agent;
    private Outline _currentOutline;
    private StationsScript _currentStation;

    private GameObject _currentTool;
    private bool _justRun =false;
    private bool _outlineActive = false; // Переменная для активации / деактивации внешней обводки объектов при наведении / снятии курсора.
    private bool _runToStation = false;  // Переменная, необходимая для отслеживания достижения игроком рабочего места какой-либо станции.

    void Start()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _currentTool = Instantiate(toolsOnLevelPrefabs[toolLevel - 1], toolAnchor.transform);
    }

    void Update()
    {
        if (_canMove)
        {
            OnPathCompleted();
            RaycastOutline();
            TooltipWork();

            if (Input.GetMouseButtonDown(1))
            {
                MouseSelectPosition();
            }
        }
        else _canMove = _currentStation.GetPermissionToMove();
    }


    /// <summary>
    /// Выделение объектов, на которые наведен курсор, внешними линиями.
    /// </summary>
    private void RaycastOutline()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 150, RaycastLayers))
        {

            if (hit.collider.GetComponent<Outline>() != null)
            {
                _selectedStation = hit.collider.gameObject.name; // Получение названия станции для отображения соответствующей UI подсказки

                if (!_outlineActive)
                {
                    _currentOutline = hit.collider.GetComponent<Outline>();
                    SetOutline(true);
                }
            }

            else
            {
                if (_currentOutline != null)
                {
                    SetOutline(false);
                }
            }
        }
    }

    /// <summary>
    /// Выбор, с помощью нажатия ПКМ, позиции следования игрока
    /// </summary>
    private void MouseSelectPosition()
    {
        Ray movingRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(movingRay, out RaycastHit movingRayHit, 150, RaycastLayers))
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

    private void TooltipWork()
    {
        if (_outlineActive && _canMove) // Подсказка включается при наведении курсора на станцию (станции подсвечиваются при наведении курсорв) если игрок не взаимодействует со станциями
        {
            for (int i = 0; i < selectedStationTooltipText.Length; i++)
            {
                if (_selectedStation == stations[i].name)
                {
                    tooltipText.text = selectedStationTooltipText[i];
                    break;
                }
            }
            tooltipOnUI.gameObject.SetActive(true);
            tooltipOnUI.position = Input.mousePosition;
        }
        else // Если станция не выделена подсветкой, подсказка выключается
        {
            tooltipOnUI.gameObject.SetActive(false);
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

        if (_currentStation != null) _currentStation.StationWork(false, toolLevel, tag);

        if (moveTargetCursor != null)
        {
            moveTargetCursor.SetActive(true);
            moveTargetCursor.transform.position = target;
        }

        _agent.destination = target;

        _anim.SetBool("IsWork", false);
        _anim.SetBool("IsRun", true);
    }


    /// <summary>
    /// Движение игрока к станции.
    /// </summary>
    /// <param name="station">Объект станции, к которой движется игроу</param>
    private void MovingToStations(GameObject station)
    {
        if (_currentStation != null) _currentStation.StationWork(false, toolLevel, tag); // Отключение станции, на которой работал игрок, если он переключился сразу на другую.

        
        if (station.TryGetComponent(out _currentStation))
        {
            _agent.destination = _currentStation.GetWorkPosition();
            _runToStation = true;
            _anim.SetBool("IsWork", false);
            _anim.SetBool("IsRun", true);
        }

        if (moveTargetCursor != null && moveTargetCursor.activeSelf == true) moveTargetCursor.SetActive(false);
    }

    // Метод, вызываемый когда игрок достиг цели
    private void OnPathCompleted()
    {
        if ((_justRun || _runToStation) && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (_runToStation)
            {
                _currentStation.StationWork(true, toolLevel, tag);
                _canMove = _currentStation.GetPermissionToMove();
                _runToStation = false;
                transform.rotation = _currentStation.GetWorkTransform().rotation;

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

    public void UpgradeTool(int toLevel)
    {
        toolLevel = toLevel;
        Destroy(_currentTool);
        _currentTool = Instantiate(toolsOnLevelPrefabs[toolLevel - 1], toolAnchor.transform);

    }
}