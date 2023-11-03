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

    // ��������� ������ ������ �������������� �� ��������
    [Space]
    [Header("UI Settings")]
    [Space]
    [Tooltip("UI ������ �������������� �� ��������.")]
    public GameObject mainPanel;

    [SerializeField, Tooltip("������ ������ ������� (��� ����������� �������� � ������� ��������������)")]
    private GameObject resourceIconPrefab;

    [SerializeField, Tooltip("���� ������, ���� ����� �������� ��������� �������������� �� �������� ��� �������� ��������� ��������")]
    private GameObject errorPanel;

    // ���������, ����������� �� ��� ���� ���������������� �������
    [Space]
    [Header("Main Settings")]
    [Space]
    [Tooltip("������� �������� ���������� ���������� �������� � ����� ������� �������.")]
    public float normalResourceDrop = 1f;

    [SerializeField, Tooltip("����� ������, ����������� ��� ����� �������� ������������.")] 
    private float workCycleTime = 4f;

    [SerializeField, Tooltip("������ �������.")] 
    private Image workTimerImage;

    [SerializeField, Tooltip("�������, � ������� ������ ����� ��� ������ �� ��������.")] 
    private GameObject workPosition;

    [SerializeField, Tooltip("�������, �� ������� ����� ������������� �������.")] 
    private GameObject resourceOutPoint;

    [SerializeField, Tooltip("������ ��������� �������.")] 
    private GameObject outResourcePrefab;

    [SerializeField, Tooltip("��������� ��������� ������������ ���� � �������, ��� ��� ������.")] 
    private bool addRandomForceToResourceInOut = true;

    [SerializeField, Tooltip("����������� ����.")] 
    private Vector3 minForce = Vector3.zero;

    [SerializeField, Tooltip("������������ ����.")] 
    private Vector3 maxForce = Vector3.up;

    [HideInInspector] public int workLevel = 1; // ������� ������ (������� �� ������ ����������� ������).


    // ���������, ����������� �� ������� ������ ����
    [Space] 
    [Header("Mining Rock Settings")]
    [Space]
    [SerializeField, Tooltip("������ ���������� ��� �� �����")] 
    private GameObject[] outOrePrefab = new GameObject[7];

    [SerializeField, Tooltip("������ ���������� ������ ������ ����, ��� ��������� ������")] 
    private float[] miningFactorOnLevel = new float[] { 1, 2, 4, 7 };

    [SerializeField, Tooltip("������ �������� ��������. ���������� � ������ ���������� ��������. ��������� �������� = 1")] 
    private float oreRareFactor = 0.85f;



    // ���������, ����������� �� ������� ������ ������
    [Space]
    [Header("Wood Settings")]
    [Space]
    [SerializeField, Tooltip("������ ���������� ������ ������ ������, ��� ��������� ������")] 
    private float[] woodProductionFactorOnLevel = new float[] {1, 2, 4, 7};


    // ���������, ����������� �� ������� ����������
    [Space]
    [Header("Sort Settings")]
    [Space]
    [SerializeField, Tooltip("������� ���������� ����������� ��������")]
    private TextMeshProUGUI countSortText;

    [SerializeField, Tooltip("����������� ���������� ������� � ������ ����������")]
    private Image selectedResImage;


    // ���������, ����������� �� ������� ��������
    [Space]
    [Header("Delivery Settings")]
    [Space]
    [SerializeField, Tooltip("������ ������������ �������� � ����������� Grid")] 
    private GameObject deliveryResourcesPanel;

    [SerializeField, Tooltip("��������� �������� �������")]
    private TextMeshProUGUI deliveryCostText;

    [SerializeField, Tooltip("������")]
    private GameObject duck;

    private ResourceObject[] _sortPacksInPlayerHands;
    private List<GameObject> _iconsOnDeliveryPanel = new();
    private float _deliveryCost;


    // ���������, ����������� �� ������� ���������
    [Space]
    [Header("Workshop Settings")]
    [Space]
    [SerializeField] private float npcWorkFactor = 1.2f;
    [SerializeField] private UpgradeButton rockUpButton;
    [SerializeField] private UpgradeButton sawmillUpButton;
    [SerializeField] private UpgradeButton toolUpButton;

    [Tooltip("������� ��������� ���������")]
    public TextMeshProUGUI upgradeCostText;

    [Tooltip("������ ������ �����")]
    public NPCSpawn npcSpawner;

    [HideInInspector] public float _currentUpgradeCost = 0; // ������� �������� ��������� ���������
    [HideInInspector] public int _currentUpgradeToolLevel; // ������� ������������ ������� ��������� �����������
    [HideInInspector] public List<StationsScript> _currentUpgradeStations; // ��������� �������, ������� ���������� (� ��� ����� �������� ������� NPC)


    // ���������, ����������� �� ��������� � �����
    [Space]
    [Header("Furnace And Factory Settings")]
    [Space]
    [SerializeField, Tooltip("������� ��������������� ��������")] 
    private GameObject[] outResourcesPrefabs;

    [SerializeField, Tooltip("������ ��������� ��� ������������ ��������")] 
    private GameObject InResourcesPanel;

    [SerializeField, Tooltip("������ ��������� �������")] 
    private GameObject outResourcePanel;
    
    private ResourceIconSettings[] _currentInResIcon = new ResourceIconSettings[2]; // ��������� ������, ��������� ��� ������������ (��� ������������ ������ ����� 2 �������. ������ ��������� �� �������-������� ������ (�������� � ���������� UI))
    private ResourceIconSettings _currentOutResIcon; // ������ ��������� �������.
    private Vector4 _requireFurnaceResources = Vector4.zero; // ��������� ��������� ��������. ��� ���������������� ������� ���������� 4 ���������: ������ ������� � ������� �������, �� ���������� (����������� �� ���������� CostOfResources)


    private string _resType; // �������� ���� ���������� ��� �������������� �������
    private int _resInd; // �������� ������� ���������� ��� �������������� �������
    private int _resValue; // �������� ���������� ��������, ���������������� ��� ��������������


    private bool _isNPCWorking = false;
    private float _NPCWorkersCountFactor = 1; // �������� ��������� ������. ������� �� ���������� ���������� �� ������� �����. (������������ ��� ������ ������ StationWork)
    private ResourcePanelEvents resourcesButtons; // ��������� ������ � ����������� �������� � ���������
    private ResourceStorage _storage; // ��������� ������
    private GameObject _player;
    private PlayerController _playerController;
    private bool _working = false; // ������ �������. ���� ������ - �� �������� ������ CycleTimer.
    private float _timer = 0; // ����� �������� ����� ������.

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


    // ����� ������� ��������� ������ (����������� CycleTimer), ��� ������ �������� ������, ���� ��������� �������� ��������.
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
    /// ��������� / ���������� ������ �������
    /// </summary>
    /// <param name="isWork">��������� / ����������</param>
    /// <param name="level">������� ������ (������� ����������� ������</param>
    /// <param name="workerTag">��� �������� (���� � ������� �������� NPC, ������� �� �����������)</param>
    public void StationWork(bool isWork, int level, string workerTag)
    {
        workLevel = level;

        // ���� ����� ������� � ������� � �������� � ���:
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
                normalResourceDrop *= _NPCWorkersCountFactor; // ���������� ���� ������������� ��� ������ NPC
                _isNPCWorking = true;
            }
        }

        // ���� ����� ���� �� �������:
        else
        {
            if (!isDelivery && !isFactory && !isFurnace) // ������� ��������, ��������� � ����� �������� ���������� �� ���������� � ��� ������
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

                if (!_isNPCWorking) // ���� ����� ���� �� ������� � �� ��� �� �������� NPC
                    _working = false;
            }

            if (mainPanel != null) mainPanel.SetActive(false);
        }
    }


    // �������� ���������� �� ����������� (���� ��� ������ �� �������� ����������� ���� ��������������, ������������ ������).
    public bool GetPermissionToMove()
    {
        if (mainPanel != null)
        {
            return !mainPanel.activeSelf;
        }
        else return true;
    }


    /// <summary>
    /// ��������� ���������� � ��������� �������, � ������� ����� ����������� �������������� (���������� � ResourcePanelEvents �� EventSystem, ��� ������� �� ������ ��������).
    /// </summary>
    /// <param name="image">����������� �������, ��� ������ � ���� ����������� ��������� ������� � �������</param>
    /// <param name="type">��� ������� (Ore, Ingot, Product). �������� ����� �������.</param>
    /// <param name="index">������ ������� ����������� ���� � ������� �������� ��������.</param>
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
        if ((_storage.GetCoins() - _currentUpgradeCost) >= 0) // ���� ������� �����, ���������� ���������
        {
            if (_currentUpgradeToolLevel > _playerController.toolLevel) // ���� ���� ��������� �����������, ���������� ������ ����������
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

            if (_currentUpgradeToolLevel > _playerController.toolsOnLevelPrefabs.Length - 1) Destroy(toolUpButton.gameObject); // ���� ������� ����������� ������������ - ��������� ����������.

            ReturnUpgradeSettings();
            mainPanel.SetActive(false);
            _working = false;
        }

        else // ����� ��������� ���� � ������� � �������� �����
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
        /// ����� ���������� �������� ������ �� UI ������ ������������� �������. ��� ����� ����������� ������ ���������� � ����������� UI ������.
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
    /// ����� ������� �� ������ ������������� ������� ������� � ����������� ��������, ���������� � ���� ���������� � ���� � ���������� ������������ ������� (��� ��� ������� � ������� ��������).
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
    /// ����������� ���������� �������, ����������������� ��� ����������, ���� ������� � ��������� ����������. (������������ �� UI ������ ������������� �������).
    /// </summary>
    /// <param name="value">������������ ��������</param>
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
    /// ������� ��������, �������� ��� ����������, � ����������� ���������
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
    /// �������� �������� �� �������. ���������� ������� "���������" � UI ������ ��������.
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

        if (_resType == "Ingot") // ��� ������������ ������� ���������� 2 ���� ����� ����
        {
            _currentInResIcon[0].SetIcon((int)_requireFurnaceResources.y * _resValue, 0, resourcesButtons.GetResourceSprite("Ore", (int)_requireFurnaceResources.x));
            _currentInResIcon[1].SetIcon((int)_requireFurnaceResources.w * _resValue, 0, resourcesButtons.GetResourceSprite("Ore", (int)_requireFurnaceResources.z));
        }
        else if (_resType == "Product") // ��� ������������ ������� ��������� 1 ��� ����� ���� � 1 ��� ������
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
        // ���� ������� �������� ��� ����������, ������ �����������
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
        // ���� ������� �������� ��� ������������ �������, ������ �����������
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
        // �����, ��������� ���� � ������� � �������� ��������
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
    /// �������� ������ � ���������������� ��������
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
