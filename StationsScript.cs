using Costs;
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
    [SerializeField] private bool isFurnace = false;
    [SerializeField] private bool isFactory = false;

    // Настройки вывода панели взаимодействия со станцией
    [Space]
    [Header("UI Settings")]
    [Space]
    [Tooltip("UI панель взаимодействия со станцией.")]
    public GameObject mainPanel;

    [SerializeField, Tooltip("Префаб иконки ресурса (для отображения ресурсов в панелях взаимодействия)")]
    private GameObject resourceIconPrefab;

    [SerializeField, Tooltip("Окно ошибки, если игрок пытается применить взаимодействие со станцией при нехватке требуемых ресурсов")]
    private GameObject errorPanel;

    // Настройки, действующие на ВСЕ типы производственных станций
    [Space]
    [Header("Main Settings")]
    [Space]
    [Tooltip("Базовое значение количества выдаваемых ресурсов в одном префабе ресурса.")]
    public float normalResourceDrop = 1f;

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

    [SerializeField, Tooltip("Применить рандомную направленную силу к ресурсу, при его выходе.")] 
    private bool addRandomForceToResourceInOut = true;

    [SerializeField, Tooltip("Минимальная сила.")] 
    private Vector3 minForce = Vector3.zero;

    [SerializeField, Tooltip("Максимальная сила.")] 
    private Vector3 maxForce = Vector3.up;

    [HideInInspector] public int workLevel = 1; // Уровень добычи (Зависит от уровня инструмента игрока).


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

    [SerializeField, Tooltip("Отображение выбранного ресурса в панели сортировки")]
    private Image selectedResImage;


    // Настройки, действующие на станцию ДОСТАВКИ
    [Space]
    [Header("Delivery Settings")]
    [Space]
    [SerializeField, Tooltip("Панель отправляемых ресурсов с настроенной Grid")] 
    private GameObject deliveryResourcesPanel;

    [SerializeField, Tooltip("Индикатор итоговой выручки")]
    private TextMeshProUGUI deliveryCostText;

    [SerializeField, Tooltip("Курьер")]
    private GameObject duck;

    private ResourceObject[] _sortPacksInPlayerHands;
    private List<GameObject> _iconsOnDeliveryPanel = new();
    private float _deliveryCost;


    // Настройки, действующие на ВЕРСТАК УЛУЧШЕНИЙ
    [Space]
    [Header("Workshop Settings")]
    [Space]
    [SerializeField] private float npcWorkFactor = 1.2f;
    [SerializeField] private UpgradeButton rockUpButton;
    [SerializeField] private UpgradeButton sawmillUpButton;
    [SerializeField] private UpgradeButton toolUpButton;

    [Tooltip("Счетчик стоимости улучшения")]
    public TextMeshProUGUI upgradeCostText;

    [Tooltip("Скрипт спавна ботов")]
    public NPCSpawn npcSpawner;

    [HideInInspector] public float _currentUpgradeCost = 0; // Текущее значение стоимости улучшения
    [HideInInspector] public int _currentUpgradeToolLevel; // Текущий предлагаемый уровень улучшения инструмента
    [HideInInspector] public List<StationsScript> _currentUpgradeStations; // Контейнер станций, которые улучшаются (К ним будет присвоен рабочий NPC)


    // Настройки, действующие на ПЛАВИЛЬНЮ и ЗАВОД
    [Space]
    [Header("Furnace And Factory Settings")]
    [Space]
    [SerializeField, Tooltip("Префабы изготавливаемых ресурсов")] 
    private GameObject[] outResourcesPrefabs;

    [SerializeField, Tooltip("Панель требуемых для изготовления ресурсов")] 
    private GameObject InResourcesPanel;

    [SerializeField, Tooltip("Панель выходного ресурса")] 
    private GameObject outResourcePanel;
    
    private ResourceIconSettings[] _currentInResIcon = new ResourceIconSettings[2]; // Контейнер иконок, требуемых для изготовления (Для изготовления всегда нужно 2 ресурса. Иконки создаются из шаблона-префаба иконки (задается в настройках UI))
    private ResourceIconSettings _currentOutResIcon; // Иконка выходного ресурса.
    private Vector4 _requireFurnaceResources = Vector4.zero; // Параметры требуемых ресурсов. Для изготавливаемого ресурса необходимо 4 параметра: индекс первого и второго ресурса, их количество (Принимается из контейнера CostOfResources)


    private string _resType; // Хранение типа выбранного для взаимодействия ресурса
    private int _resInd; // Хранение индекса выбранного для взаимодействия ресурса
    private int _resValue; // Хранение количества ресурсов, подготавливаемых для взаимодействия


    private bool _isNPCWorking = false;
    private float _NPCWorkersCountFactor = 1; // Параметр множителя работы. Зависит от количества работаемых на станции людей. (Прибавляется при вызове метода StationWork)
    private ResourcePanelEvents resourcesButtons; // контейнер кнопок и изображений ресурсов в инвентаре
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
        resourcesButtons = FindFirstObjectByType<ResourcePanelEvents>();
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
            RockMining();
        }

        else if (isSawmill)
        {
            ResourceOut(woodProductionFactorOnLevel[workLevel - 1] * normalResourceDrop * _NPCWorkersCountFactor);
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

        else if (isFurnace || isFactory)
        {
            ProductionEnded();
            workTimerImage.fillAmount = 0f;
            workTimerImage.gameObject.SetActive(false);
            _working = false;
        }
        else ResourceOut();
    }

    /// <summary>
    /// Включение / выключение работы станции
    /// </summary>
    /// <param name="isWork">Включение / выключение</param>
    /// <param name="level">Уровень работы (Уровень инструмента игрока</param>
    /// <param name="workerTag">Тег рабочего (Если в станции работают NPC, станция не выключается)</param>
    public void StationWork(bool isWork, int level, string workerTag)
    {
        workLevel = level;

        // Если игрок подошел к станции и работает с ней:
        if (isWork)
        {
            if (mainPanel != null) mainPanel.SetActive(true);

            if (isSorter)
            {
                _resValue = 0;
                countSortText.text = _resValue.ToString();
            }

            else if (isDelivery)
            {

                if (_storage.playerHands.GetComponentInChildren<ResourceObject>() != null)
                {
                    _sortPacksInPlayerHands = _storage.playerHands.GetComponentsInChildren<ResourceObject>();

                    SetDeliveryStats();
                }
            }

            else if (isFurnace || isFactory)
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
            {
                _NPCWorkersCountFactor *= npcWorkFactor;
                normalResourceDrop *= _NPCWorkersCountFactor; // Количество руды увеличивается при работе NPC
                _isNPCWorking = true;
            }
        }

        // Если игрок ушел от станции:
        else
        {
            if (!isDelivery && !isFactory && !isFurnace) // Станция доставки, плавильня и завод работают независимо от нахождения в них игрока
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
                    ReturnUpgradeSettings();
                }

                _timer = 0;

                if (!_isNPCWorking) // Если игрок ушел от станции и на ней не работают NPC
                    _working = false;
            }

            if (mainPanel != null) mainPanel.SetActive(false);
        }
    }


    // Получить разрешение на перемещение (Если при работе со станцией открывается меню взаимодействия, перемещаться нельзя).
    public bool GetPermissionToMove()
    {
        if (mainPanel != null)
        {
            return !mainPanel.activeSelf;
        }
        else return true;
    }


    /// <summary>
    /// Получение выбранного в инвентаре ресурса, с которым будет происходить взаимодействие (Вызывается в ResourcePanelEvents на EventSystem, при нажатии на кнопки ресурсов).
    /// </summary>
    /// <param name="image">Изображение ресурса, для вывода в окно отображения активного ресурса в станции</param>
    /// <param name="type">Тип ресурса (Ore, Ingot, Product). Задается тегом объекта.</param>
    /// <param name="index">Индекс ресурса конкретного типа в системе хранения ресурсов.</param>
    public void SetSettingsOfSelectedResource(Image image, string type, int index)
    {
        _resType = type;
        _resInd = index;

        if (isSorter)
        {
            selectedResImage.sprite = image.sprite;
        }
        else if (isFurnace || isFactory)
        {
            _resValue = 1;
            if (_currentOutResIcon == null)
            {
                _currentOutResIcon = Instantiate(resourceIconPrefab, outResourcePanel.transform).GetComponent<ResourceIconSettings>();
                _currentInResIcon[0] = Instantiate(resourceIconPrefab, InResourcesPanel.transform).GetComponent<ResourceIconSettings>();
                _currentInResIcon[1] = Instantiate(resourceIconPrefab, InResourcesPanel.transform).GetComponent<ResourceIconSettings>();
                CalculateStationRequireResources(_resInd);
            }

            else
            {
                CalculateStationRequireResources(_resInd);
            }
        }
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
            if (_currentUpgradeToolLevel > _playerController.toolLevel) // Если есть улучшение инструмента, инструмент игрока улучшается
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

            if (_currentUpgradeToolLevel > _playerController.toolsOnLevelPrefabs.Length - 1) Destroy(toolUpButton.gameObject); // Если уровень инструмента максимальный - улучшение невозможно.

            ReturnUpgradeSettings();
            mainPanel.SetActive(false);
            _working = false;
        }

        else // Иначе выводится окно с ошибкой о нехватке денег
        {
            errorPanel.SetActive(true);
        }
    }

    public void ReturnUpgradeSettings()
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
                resource = Instantiate(outResourcePrefab, transform);
            }

            valueWithRare *= Random.Range(0, normalResourceDrop * miningFactorOnLevel[workLevel - 1]);

            if (resource.TryGetComponent(out ResourceObject resSc))
            {
                resSc.SetResourceValues(index, valueWithRare);

                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
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
                resource = Instantiate(outResourcePrefab, transform);
            }

            if (resource.TryGetComponent(out ResourceObject resSc))
            {
                resSc.SetResourceValues(value);

                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
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

            else resource = Instantiate(outResourcePrefab, transform);

            if (resource.TryGetComponent(out ResourceObject resSc))
            {
                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
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
        if (_resValue > 0)
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
            else resource = Instantiate(outResourcePrefab, transform);

            if (resource.TryGetComponent(out ResourceObject resSc))
            {
                resSc.SetResourceValues(_resType, _resInd, _resValue);

                _storage.AddResourceByType(_resType, _resInd, _resValue * -1);

                if (addRandomForceToResourceInOut)
                {
                    Vector3 forceVec = new(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
                    resSc.AddForceToResourceOut(forceVec);
                }
            }
        }
        _resValue = 0;
    }

    /// <summary>
    /// Прибавление количества ресурса, подготавливаемого для сортировки, если ресурса в инвентаре достаточно. (Отображается на UI панели сортировочной станции).
    /// </summary>
    /// <param name="value">Прибавляемое значение</param>
    public void AddSortResource(int value)
    {
        if (value < 0 && _resValue + value >= 0)
            _resValue += value;

        if (value > 0 && _storage.GetResourceByType(_resType, _resInd) - ((float)value + (float)_resValue) >= 0)
        {
            _resValue += value;
        }

        countSortText.text = _resValue.ToString();
    }


    /// <summary>
    /// Возврат значений, заданных при сортировке, в стандартное состояние
    /// </summary>
    public void ReturnDefaultSortValues()
    {
        _resValue = 0;
        countSortText.text = "0";
    }
    #endregion



    #region Delivery Methods
    private void SetDeliveryStats()
    {
        for (int i = 0; i < _sortPacksInPlayerHands.Length; i++)
        {
            GameObject icon = Instantiate(resourceIconPrefab, deliveryResourcesPanel.transform);

            _iconsOnDeliveryPanel.Add(icon);

            ResourceObject resObj = _sortPacksInPlayerHands[i];
            float cost = CostOfResources.GetCostOfResource(resObj.typeOfResource, resObj.index) * (int)resObj.value;

            icon.GetComponent<ResourceIconSettings>().SetIcon((int)resObj.value, cost, resourcesButtons.GetResourceSprite(resObj.typeOfResource, resObj.index));
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



    #region Furnace And Factory Methods
    private void FurnaceAndFactoryOut() 
    {
        GameObject resource;

        resource = Instantiate(outResourcesPrefabs[_resInd], resourceOutPoint.transform);

        if (resource.TryGetComponent(out ResourceObject resSc))
        {
            resSc.SetResourceValues(_resValue);

            if (addRandomForceToResourceInOut)
            {
                Vector3 forceVec = new(Random.Range(minForce.x, maxForce.x), Random.Range(minForce.x, maxForce.y), Random.Range(minForce.x, maxForce.z));
                resSc.AddForceToResourceOut(forceVec);
            }
        }
    }

    private void CalculateStationRequireResources(int resourceIndex)
    {
        _currentOutResIcon.GetComponent<ResourceIconSettings>().SetIcon(_resValue, 0, resourcesButtons.GetResourceSprite(_resType, _resInd));
        _requireFurnaceResources = CostOfResources.GetCostToProduction(_resType, resourceIndex);

        if (_resType == "Ingot") // Для производства слитков необходимо 2 вида сырой руды
        {
            _currentInResIcon[0].SetIcon((int)_requireFurnaceResources.y * _resValue, 0, resourcesButtons.GetResourceSprite("Ore", (int)_requireFurnaceResources.x));
            _currentInResIcon[1].SetIcon((int)_requireFurnaceResources.w * _resValue, 0, resourcesButtons.GetResourceSprite("Ore", (int)_requireFurnaceResources.z));
        }
        else if (_resType == "Product") // Для производства товаров необходим 1 вид сырой руды и 1 вид слитка
        {
            _currentInResIcon[0].SetIcon((int)_requireFurnaceResources.y * _resValue, 0, resourcesButtons.GetResourceSprite("Ore", (int)_requireFurnaceResources.x));
            _currentInResIcon[1].SetIcon((int)_requireFurnaceResources.w * _resValue, 0, resourcesButtons.GetResourceSprite("Ingot", (int)_requireFurnaceResources.z));
        }
    }

    public void ProductionEnded()
    {
        FurnaceAndFactoryOut();
        ReturnDefaultProductionSettings();
    }

    public void ApplyProduction()
    {
        // Если хватает ресурсов для ПЕРЕПЛАВКИ, работа запускается
        if (isFurnace &&
            _requireFurnaceResources.y * _resValue <= _storage.GetOre((int)_requireFurnaceResources.x) &&
            _requireFurnaceResources.w * _resValue <= _storage.GetOre((int)_requireFurnaceResources.z))
        {
            _storage.AddOre((int)_requireFurnaceResources.x, -_requireFurnaceResources.y * _resValue);
            _storage.AddOre((int)_requireFurnaceResources.z, -_requireFurnaceResources.w * _resValue);

            workTimerImage.gameObject.SetActive(true);
            _working = true;
            mainPanel.SetActive(false);
        }
        // Если хватает ресурсов для ИЗГОТОВЛЕНИЯ ТОВАРОВ, работа запускается
        else if (isFactory &&
            _requireFurnaceResources.y * _resValue <= _storage.GetOre((int)_requireFurnaceResources.x) &&
            _requireFurnaceResources.w * _resValue <= _storage.GetIngot((int)_requireFurnaceResources.z))
        {
            _storage.AddOre((int)_requireFurnaceResources.x, -_requireFurnaceResources.y * _resValue);
            _storage.AddIngot((int)_requireFurnaceResources.z, -_requireFurnaceResources.w * _resValue);

            workTimerImage.gameObject.SetActive(true);
            _working = true;
            mainPanel.SetActive(false);
        }
        // Иначе, выводится окно с ошибкой о нехватке ресурсов
        else errorPanel.SetActive(true);
    }

    public void AddProductionResource(int value)
    {
        if (value < 0 && _resValue + value >= 1)
            _resValue += value;

        else if (value > 0)
            _resValue += value;

        CalculateStationRequireResources(_resInd);
    }

    /// <summary>
    /// Обнулить работу с производственной станцией
    /// </summary>
    public void ReturnDefaultProductionSettings()
    {
        if (_currentOutResIcon != null)
        {
            Destroy(_currentOutResIcon.gameObject);
            Destroy(_currentInResIcon[0].gameObject);
            Destroy(_currentInResIcon[1].gameObject);
            _requireFurnaceResources = Vector4.zero;
        }
    }
    #endregion


    public Transform GetWorkTransform()
    {
        return workPosition.transform;
    }

    public Vector3 GetWorkPosition()
    {
        return workPosition.transform.position;
    }
}
