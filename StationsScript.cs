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

    [SerializeField, Tooltip("UI ������ �������������� �� ��������.")]
    public GameObject mainPanel;

    [SerializeField, Tooltip("����������� ����������� ��� �������������� �������.")]
    private Image selectedResourceImage;


    // ���������, ����������� �� ��� ���� ���������������� �������
    [Space]
    [Header("Main Settings")]
    [Space]
    [SerializeField, Tooltip("������� �������� ���������� ���������� �������� � ����� ������� �������.")]
    public float normalResourceDrop = 2f;

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
    private string _resSortType; // �������� ���� ���������� ��� ���������� �������
    private int _resSortInd; // �������� ������� ���������� ��� ���������� �������
    private int _resSortValue; // �������� ���������� ��������, ���������������� � ����������


    // ���������, ����������� �� ������� ��������
    [Space]
    [Header("Delivery Settings")]
    [Space]
    [SerializeField, Tooltip("������ ������������ �������� � ����������� Grid")] 
    private GameObject deliveryResourcesPanel;

    [SerializeField, Tooltip("��������� �������� �������")]
    private TextMeshProUGUI deliveryCostText;

    [SerializeField, Tooltip("������ ������ ������������� ������� (��� ����������� ������������ ��������)")]
    private GameObject deliveryResourceIconPrefab;

    [SerializeField, Tooltip("������")]
    private GameObject duck;

    private ResourceObject[] _sortPacksInPlayerHands;
    private float _deliveryCost;



    private ResourceStorage _storage;
    private GameObject _player;
    private int _workLevel = 1; // ������� ������.
    private bool _working = false; // ������ �������. ���� ������ - �� �������� ������ CycleTimer.
    private float _timer = 0; // ����� �������� ����� ������.

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _storage = _player.GetComponent<ResourceStorage>();
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

    private void WorkEnded()
    {
        if (isRock)
        {
            RockMining();
        }

        else if (isSawmill)
        {
            ResourceOut(woodProductionFactorOnLevel[_workLevel] * normalResourceDrop);
        }

        else if (isSorter)
        {
            Debug.Log("SorterOk");
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

    private void RockMining()
    {
        float rare = 1;

        if (_workLevel == 1)
        {
            ResourceOut(0, rare);
            ResourceOut(1, rare);
            rare *= oreRareFactor;
            ResourceOut(2, rare);
        }

        else if (_workLevel == 2)
        {
            ResourceOut(0, rare);
            ResourceOut(1, rare);
            rare *= oreRareFactor;
            ResourceOut(2, rare);
            ResourceOut(3, rare);
            ResourceOut(4, rare);
        }

        else if (_workLevel >= 3)
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

    public Vector3 GetWorkPosition()
    {
        return workPosition.transform.position;
    }

    public void StationWork(bool isWork, int level)
    {
        _workLevel = level;

        // ���� ����� ������� � ������� � �������� � ���:
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

            else
            {
                if (workTimerImage != null)
                    workTimerImage.gameObject.SetActive(true);
                _working = true;
            }
        }
        // ���� ����� ���� �� �������:
        else
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

            _timer = 0;
            _working = false;
        }
    }


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

            valueWithRare *= Random.Range(0, normalResourceDrop * miningFactorOnLevel[_workLevel - 1]);
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



    #region Sort Station Methods
    /// <summary>
    /// ����� ���������� �������� ������ �� UI ������ ������������� �������. ��� ����� ����������� ������ ���������� � ����������� UI ������.
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
    /// ����� ������� �� ������ ������������� ������� ������� � ����������� ��������, ���������� � ���� ���������� � ���� � ���������� ������������ ������� (��� ��� ������� � ������� ��������).
    /// </summary>
    private void SortResourceOut()
    {
        Debug.Log("OutResource");
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
    /// ����������� ���������� �������, ����������������� ��� ����������, ���� ������� � ��������� ����������. (������������ �� UI ������ ������������� �������).
    /// </summary>
    /// <param name="value">������������ ��������</param>
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
    /// ��������� ���������� � ��������� �������, � ������� ����� ����������� �������������� (���������� � ResourcePanelEvents �� EventSystem, ��� ������� �� ������ ��������).
    /// </summary>
    /// <param name="image">����������� �������, ��� ������ � ���� ����������� ��������� ������� � �������</param>
    /// <param name="type">��� ������� (Ore, Ingot, Product). �������� ����� �������.</param>
    /// <param name="index">������ ������� ����������� ���� � ������� �������� ��������.</param>
    public void SetSettingsOfSelectedResource(Image image, string type, int index)
    {
        selectedResourceImage.sprite = image.sprite;
        _resSortType = type;
        _resSortInd = index;
    }

    /// <summary>
    /// ������� ��������, �������� ��� ����������, � ����������� ���������
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
        Debug.Log("Length packs: " + _sortPacksInPlayerHands.Length);

        for (int i = 0; i < _sortPacksInPlayerHands.Length; i++)
        {
            GameObject icon = Instantiate(deliveryResourceIconPrefab, deliveryResourcesPanel.transform);

            ResourceObject resObj = _sortPacksInPlayerHands[i];
            float cost = Costs.CostOfResources.GetCostOfResource(resObj.typeOfResource, resObj.index);

            icon.GetComponent<DeliveryResourcePrefabSettings>().SetIcon((int)resObj.value, cost * (int)resObj.value, resObj.GetSprite());

            _deliveryCost += cost * resObj.value;
            deliveryCostText.text = _deliveryCost.ToString();
        }
    }

    public void ResetDeliveryStats()
    {
        var icons = deliveryResourcesPanel.GetComponentsInChildren<Transform>();

        foreach (var item in icons)
        {
            Destroy(item.gameObject);
        }
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

        mainPanel.SetActive(false);
        workTimerImage.gameObject.SetActive(true);
        _working = true;
    }
    #endregion
}
