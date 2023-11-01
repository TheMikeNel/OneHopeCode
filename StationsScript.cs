using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StationsScript : MonoBehaviour
{
    [Header("Station Type")]
    [Space]
    [SerializeField] private bool isRock = false;
    [SerializeField] private bool isDelivery = false;
    [SerializeField] private bool isSawmill = false;
    [SerializeField] private bool isWorkshop = false;
    [SerializeField] private bool isSorter = false;
    //[SerializeField] private bool isFurnace = false;
    //[SerializeField] private bool isFactory = false;


    // Настройки вывода панели взаимодействия со станцией
    [Space]
    [Header("UI Settings")]
    [Space]

    [SerializeField, Tooltip("UI панель взаимодействия со станцией.")]
    public GameObject mainPanel;

    [SerializeField, Tooltip("Изображение выбираемого для взаимодействия ресурса.")]
    private Image selectedResourceImage;


    // Настройки, действующие на ВСЕ типы производственных станций
    [Space]
    [Header("Main Settings")]
    [Space]
    [SerializeField, Tooltip("Базовое значение количества выдаваемых ресурсов в одном префабе ресурса.")]
    public float normalResourceDrop = 2f;

    [SerializeField, Tooltip("Время работы, необходимое для одной итерации производства.")] 
    private float workCycleTime = 4f;

    [SerializeField, Tooltip("Спрайт таймера.")] 
    private Image workTimerImage;

    [SerializeField, Tooltip("Позиция, к которой пойдет игрок при работе со станцией.")] 
    private GameObject workPosition;

    [SerializeField, Tooltip("Позиция, из которой будут выбрасываться ресурсы.")] 
    private GameObject resourceOutPoint;

    [SerializeField, Tooltip("Префаб выходного ресурса.")] 
    private GameObject outResourcePrefab;

    [SerializeField, Tooltip("Контейнер кнопок ресурсов инвентаря")]
    private ResourcePanelEvents panelEvents;

    [SerializeField, Tooltip("Применить рандомную направленную силу к ресурсу, при его выходе.")] 
    private bool addRandomForceToResourceInOut = true;

    [SerializeField, Tooltip("Минимальная сила.")] 
    private Vector3 minForce = Vector3.zero;

    [SerializeField, Tooltip("Максимальная сила.")] 
    private Vector3 maxForce = Vector3.up;


    // Настройки, действующие на станцию ДОБЫЧИ РУДЫ
    [Space] 
    [Header("Mining Rock Settings")]
    [Space]
    [SerializeField, Tooltip("Массив выдаваемых руд из камня")] 
    private GameObject[] outOrePrefab = new GameObject[7];

    [SerializeField, Tooltip("Фактор увеличения объема добычи руды, при повышении уровня")] 
    private float[] miningFactorOnLevel = new float[] { 1, 2, 4, 7 };

    [SerializeField, Tooltip("Фактор редкости ресурсов. Умножается с каждым повышением редкости. Начальная редкость = 1")] 
    private float oreRareFactor = 0.85f;


    // Настройки, действующие на станцию ДОБЫЧИ ДЕРЕВА
    [Space]
    [Header("Wood Settings")]
    [Space]
    [SerializeField, Tooltip("Фактор увеличения объема добычи дерева, при повышении уровня")] 
    private float[] woodProductionFactorOnLevel = new float[] {1, 2, 4, 7};


    // Настройки, действующие на станцию СОРТИРОВКИ
    [Space]
    [Header("Sort Settings")]
    [Space]
    [SerializeField, Tooltip("Счетчик количества сортируемых ресурсов")]
    private TextMeshProUGUI countSortText;
    private string _resSortType; // Хранение типа выбранного для сортировки ресурса
    private int _resSortInd; // Хранение индекса выбранного для сортировки ресурса
    private int _resSortValue; // Хранение количества ресурсов, подготавливаемых к сортировке


    // Настройки, действующие на станцию ДОСТАВКИ
    [Space]
    [Header("Delivery Settings")]
    [Space]
    [SerializeField, Tooltip("Панель отправляемых ресурсов с настроенной Grid")] 
    private GameObject deliveryResourcesPanel;

    [SerializeField, Tooltip("Индикатор итоговой выручки")]
    private TextMeshProUGUI deliveryCostText;

    [SerializeField, Tooltip("Префаб иконки отправляемого ресурса (для отображения отправляемых ресурсов)")]
    private GameObject deliveryResourceIconPrefab;

    [SerializeField, Tooltip("Курьер")]
    private GameObject duck;

    private ResourceObject[] _sortPacksInPlayerHands;
    private List<GameObject> _iconsOnDeliveryPanel = new List<GameObject>();
    private float _deliveryCost;


    [Space]
    [Header("Workshop Settings")]
    [Space]
    [SerializeField] private UpgradeButton rockUpButton;
    [SerializeField] private UpgradeButton sawmillUpButton;
    [SerializeField] private UpgradeButton toolUpButton;

    [SerializeField, Tooltip("Счетчик стоимости улучшения")]
    public TextMeshProUGUI upgradeCostText;

    [SerializeField, Tooltip("Окно ошибки, если игрок пытается применить улучшение при недостаточном количестве денег")] 
    private GameObject errorPanel;

    [SerializeField, Tooltip("Скрипт спавна ботов")]
    public NPCSpawn npcSpawner;

    [HideInInspector] public float _currentUpgradeCost = 0; // Текущее значение стоимости улучшения
    [HideInInspector] public int _currentUpgradeToolLevel; // Текущий предлагаемый уровень улучшения инструмента
    [HideInInspector] public List<StationsScript> _currentUpgradeStations; // Текущие предлагаемые улучшения станций


    [HideInInspector] public int workLevel = 1; // Уровень добычи.

    private int _workFactor = 1; // Параметр множителя работы. Зависит от количества работаемых на станции людей. (Прибавляется или вычитается при вызове метода StationWork)
    private ResourceStorage _storage; // Инвентарь игрока
    private GameObject _player;
    private PlayerController _playerController;
    private bool _working = false; // Работа станции. Если ИСТИНА - то работает таймер CycleTimer.
    private float _timer = 0; // Время текущего цикла работы.

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _storage = _player.GetComponent<ResourceStorage>();
        _playerController = _player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (_working)
        {
            CycleTimer();
        }
    }

    private void CycleTimer()
    {
        _timer += Time.deltaTime;

        if (workTimerImage != null)
            workTimerImage.fillAmount = _timer / workCycleTime;

        if (_timer > workCycleTime)
        {
            if (workTimerImage != null)
                workTimerImage.fillAmount = 0;

            WorkEnded();
            _timer = 0;
        }
    }


    // Когда станция завершает работу (срабатывает CycleTimer), она выдает заданный ресурс, либо выполняет заданные действия.
    private void WorkEnded()
    {
        if (isRock)
        {
            for (int i = 0; i < _workFactor; i++)
            {
                RockMining();
            }
        }

        else if (isSawmill)
        {
            for (int i = 0; i < _workFactor; i++)
            {
                ResourceOut(woodProductionFactorOnLevel[workLevel] * normalResourceDrop);
            }
        }

        else if (isSorter)
        {
            SortResourceOut();
            _working = false;
        }

        else if (isDelivery)
        {
            workTimerImage.fillAmount = 0f;
            workTimerImage.gameObject.SetActive(false);
            _working = false;
        }
        else ResourceOut();
    }


    public Transform GetWorkTransform()
    {
        return workPosition.transform;
    }

    public Vector3 GetWorkPosition()
    {
        return workPosition.transform.position;
    }

    public void StationWork(bool isWork, int level, string workerTag)
    {
        workLevel = level;

        // Если игрок подошел к станции и работает с ней:
        if (isWork)
        {
            if (isSorter)
            {
                _resSortValue = 0;
                countSortText.text = _resSortValue.ToString();
                mainPanel.SetActive(true);
            }

            else if (isDelivery)
            {
                mainPanel.SetActive(true);

                if (_storage.playerHands.GetComponentInChildren<ResourceObject>() != null)
                {
                    _sortPacksInPlayerHands = _storage.playerHands.GetComponentsInChildren<ResourceObject>();

                    SetDeliveryStats();
                }
            }

            else if (isWorkshop)
            {
                mainPanel.SetActive(true);
            }

            else
            {
                if (workTimerImage != null)
                    workTimerImage.gameObject.SetActive(true);
                _working = true;
            }
            if (workerTag == "NPC")
                _workFactor++;
        }

        // Если игрок ушел от станции:
        else
        {
            if (!isDelivery) // Станция доставки работает независимо от нахождения в ней игрока
            {
                if (workTimerImage != null)
                {
                    workTimerImage.fillAmount = 0;
                    workTimerImage.gameObject.SetActive(false);
                }

                if (isSorter)
                {
                    ReturnDefaultSortValues();
                }

                if (isWorkshop)
                {
                    ReturnUpdateSettings();
                }

                _timer = 0;
                _working = false;
            }

            if (mainPanel != null) mainPanel.SetActive(false);
        }
    }


    // Получить разрешение на перемещение (Если при работе со станцией не открывается меню взаимодействия, возвращает true. Иначе - false).
    public bool GetPermissionToMove()
    {
        if (mainPanel != null)
        {
            return !mainPanel.activeSelf;
        }
        else return true;
    }



    #region Workshop Upgrade Methods
    public void SetUpgradeStation(float cost, StationsScript station)
    {
        _currentUpgradeCost += cost;

        if (cost < 0)
        {
            _currentUpgradeStations.Remove(station);
        }
        else _currentUpgradeStations.Add(station);

        upgradeCostText.text = _currentUpgradeCost.ToString("#.#");
    }

    public void SetUpgradeToolLevel(float cost, int inc)
    {
        _currentUpgradeCost += cost;
        _currentUpgradeToolLevel += inc;

        upgradeCostText.text = _currentUpgradeCost.ToString("#.#");
    }

    public void ApplyUpgrade()
    {
        if ((_storage.GetCoins() - _currentUpgradeCost) >= 0) // Если хватает денег, происходит улучшение
        {
            if (_currentUpgradeToolLevel > _playerController.toolLevel)
            {
                _playerController.UpgradeTool(_currentUpgradeToolLevel);
                workLevel = _currentUpgradeToolLevel;
            }

            if (_currentUpgradeStations != null)
            {
                if (_currentUpgradeStations.Count > 0)
                {
                    foreach (var item in _currentUpgradeStations)
                    {
                        npcSpawner.Spawn(item);
                    }
                }
            }
            _storage.AddCoins(-_currentUpgradeCost);

            if (_currentUpgradeToolLevel > _playerController.toolsOnLevelPrefabs.Length) Destroy(toolUpButton.gameObject); // Если уровень инструмента максимальный - улучшение невозможно.

            ReturnUpdateSettings();
            mainPanel.SetActive(false);
            _working = false;
        }

        else // Иначе выводится окно с ошибкой о нехватке денег
        {
            errorPanel.SetActive(true);
        }
    }

    public void ReturnUpdateSettings()
    {
        _currentUpgradeStations.Clear();
        _currentUpgradeCost = 0;
        _currentUpgradeToolLevel = 0;

        rockUpButton.isActiveUpgrade = false;
        sawmillUpButton.isActiveUpgrade = false;
        if (toolUpButton != null) toolUpButton.isActiveUpgrade = false;

        upgradeCostText.text = "0";
    }
    #endregion



    #region Resource Out Methods
    private void ResourceOut(int index, float valueWithRare)
    {
        if (isRock) outResourcePrefab = outOrePrefab[index];

        if (outResourcePrefab != null)
        {
            GameObject resource;

            if (resourceOutPoint != null)
            {
                resource = Instantiate(outResourcePrefab, resourceOutPoint.transform);
            }

            else
            {
                resource = Instantiate(outResourcePrefab, gameObject.transform);
            }

            valueWithRare *= Random.Range(0, normalResourceDrop * miningFactorOnLevel[workLevel - 1]);
            ResourceObject resSc = resource.GetComponent<ResourceObject>();

            if (resSc != null)
            {
                resSc.SetResourceValues(index, valueWithRare);

                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new Vector3(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
                    resSc.AddForceToResourceOut(forceVec);
                }
            }
        }
    }

    private void ResourceOut(float value)
    {
        if (outResourcePrefab != null)
        {
            GameObject resource;

            if (resourceOutPoint != null)
            {
                resource = Instantiate(outResourcePrefab, resourceOutPoint.transform);
            }

            else
            {
                resource = Instantiate(outResourcePrefab, gameObject.transform);
            }

            ResourceObject resSc = resource.GetComponent<ResourceObject>();

            if (resSc != null)
            {
                resSc.SetResourceValues(value);

                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new Vector3(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
                    resSc.AddForceToResourceOut(forceVec);
                }
            }            
        }

    }

    private void ResourceOut()
    {
        if (outResourcePrefab != null)
        {
            GameObject resource;

            if (resourceOutPoint != null) resource = Instantiate(outResourcePrefab, resourceOutPoint.transform);

            else resource = Instantiate(outResourcePrefab, gameObject.transform);

            ResourceObject resSc = resource.GetComponent<ResourceObject>();

            if (resSc != null)
            {
                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new Vector3(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
                    resSc.AddForceToResourceOut(forceVec);
                }
            }
        }
    }
    #endregion



    #region Rock Mining Method
    private void RockMining()
    {
        float rare = 1;

        if (workLevel == 1)
        {
            ResourceOut(0, rare);
            ResourceOut(1, rare);
            rare *= oreRareFactor;
            ResourceOut(2, rare);
        }

        else if (workLevel == 2)
        {
            ResourceOut(0, rare);
            ResourceOut(1, rare);
            rare *= oreRareFactor;
            ResourceOut(2, rare);
            ResourceOut(3, rare);
            ResourceOut(4, rare);
        }

        else if (workLevel >= 3)
        {
            ResourceOut(0, rare);
            ResourceOut(1, rare);
            rare *= oreRareFactor;
            ResourceOut(2, rare);
            ResourceOut(3, rare);
            ResourceOut(4, rare);
            rare *= oreRareFactor;
            ResourceOut(5, rare);
            rare *= oreRareFactor;
            ResourceOut(6, rare);
        }
    }
    #endregion



    #region Sort Station Methods
    /// <summary>
    /// Метод вызывается нажатием кнопки на UI панели сортировочной станции. Тем самым запускается работа сортировки и закрывается UI панель.
    /// </summary>
    public void DoSort()
    {
        if (_resSortValue > 0)
        {
            if (workTimerImage != null)
                workTimerImage.gameObject.SetActive(true);

            _working = true;
        }
    }

    /// <summary>
    /// Метод создает на выходе сортировочной станции коробку с упакованным ресурсом, содержащую в себе информацию о типе и количестве упакованного ресурса (Для его продаже в станции доставки).
    /// </summary>
    private void SortResourceOut()
    {
        if (outResourcePrefab != null)
        {
            GameObject resource;

            if (resourceOutPoint != null) resource = Instantiate(outResourcePrefab, resourceOutPoint.transform);
            else resource = Instantiate(outResourcePrefab, gameObject.transform);

            ResourceObject resSc = resource.GetComponent<ResourceObject>();

            if (resSc != null)
            {
                resSc.SetResourceValues(_resSortType, _resSortInd, _resSortValue);

                _storage.AddResourceByType(_resSortType, _resSortInd, _resSortValue * -1);

                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new Vector3(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
                    resSc.AddForceToResourceOut(forceVec);
                }
            }
        }
        _resSortValue = 0;
    }

    /// <summary>
    /// Прибавление количества ресурса, подготавливаемого для сортировки, если ресурса в инвентаре достаточно. (Отображается на UI панели сортировочной станции).
    /// </summary>
    /// <param name="value">Прибавляемое значение</param>
    public void AddSortResource(int value)
    {
        if (value < 0 && _resSortValue + value >= 0)
            _resSortValue += value;

        if (value > 0 && _storage.GetResourceByType(_resSortType, _resSortInd) - ((float)value + (float)_resSortValue) >= 0)
        {
            _resSortValue += value;
        }

        countSortText.text = _resSortValue.ToString();
    }

    /// <summary>
    /// Получение выбранного в инвентаре ресурса, с которым будет происходить взаимодействие (Вызывается в ResourcePanelEvents на EventSystem, при нажатии на кнопки ресурсов).
    /// </summary>
    /// <param name="image">Изображение ресурса, для вывода в окно отображения активного ресурса в станции</param>
    /// <param name="type">Тип ресурса (Ore, Ingot, Product). Задается тегом объекта.</param>
    /// <param name="index">Индекс ресурса конкретного типа в системе хранения ресурсов.</param>
    public void SetSettingsOfSelectedResource(Image image, string type, int index)
    {
        selectedResourceImage.sprite = image.sprite;
        _resSortType = type;
        _resSortInd = index;
    }

    /// <summary>
    /// Возврат значений, заданных при сортировке, в стандартное состояние
    /// </summary>
    public void ReturnDefaultSortValues()
    {
        _resSortValue = 0;
        countSortText.text = "0";
    }
    #endregion



    #region Delivery
    private void SetDeliveryStats()
    {
        for (int i = 0; i < _sortPacksInPlayerHands.Length; i++)
        {
            GameObject icon = Instantiate(deliveryResourceIconPrefab, deliveryResourcesPanel.transform);

            _iconsOnDeliveryPanel.Add(icon);

            ResourceObject resObj = _sortPacksInPlayerHands[i];
            float cost = Costs.CostOfResources.GetCostOfResource(resObj.typeOfResource, resObj.index) * (int)resObj.value;

            icon.GetComponent<DeliveryResourceIconSettings>().SetIcon((int)resObj.value, cost, panelEvents.GetResourceSprite(resObj.typeOfResource, resObj.index));
            _deliveryCost += cost;
        }

        deliveryCostText.text = _deliveryCost.ToString();
    }

    public void ResetDeliveryStats()
    {
        foreach (var item in _iconsOnDeliveryPanel)
        {
            Destroy(item);
        }
        _iconsOnDeliveryPanel.Clear();
        _deliveryCost = 0;
        deliveryCostText.text = _deliveryCost.ToString();
    }

    /// <summary>
    /// Отправка ресурсов на продажу. Вызывается кнопкой "Отправить" в UI панели доставки.
    /// </summary>
    public void SendDelivery()
    {
        duck.GetComponent<DuckCourier>().SendCourier(workCycleTime, _deliveryCost);

        foreach (ResourceObject item in _sortPacksInPlayerHands)
        {
            Destroy(item.gameObject);
        }
        ResetDeliveryStats();
        mainPanel.SetActive(false);
        workTimerImage.gameObject.SetActive(true);
        _working = true;
    }
    #endregion
}
