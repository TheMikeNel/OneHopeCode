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

    [SerializeField, Tooltip("UI панель взаимодействия со станцией.")]
    public GameObject mainPanel;

    [SerializeField, Tooltip("Изображение выбираемого для взаимодействия ресурса.")]
    private Image selectedResourceImage;


    // Настройки, действующие на все типы производственных станций
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

    [SerializeField, Tooltip("Применить рандомную направленную силу к ресурсу, при его выходе.")] 
    private bool addRandomForceToResourceInOut = true;

    [SerializeField, Tooltip("Минимальная сила.")] 
    private Vector3 minForce = Vector3.zero;

    [SerializeField, Tooltip("Максимальная сила.")] 
    private Vector3 maxForce = Vector3.up;


    // Настройки, действующие на станцию добычи руды
    [Space] 
    [Header("Mining Rock Settings")]
    [Space]
    [SerializeField, Tooltip("Массив выдаваемых руд из камня")] 
    private GameObject[] outOrePrefab = new GameObject[7];

    [SerializeField, Tooltip("Фактор увеличения объема добычи руды, при повышении уровня")] 
    private float[] miningFactorOnLevel = new float[] { 1, 2, 4, 7 };

    [SerializeField, Tooltip("Фактор редкости ресурсов. Умножается с каждым повышением редкости. Начальная редкость = 1")] 
    private float oreRareFactor = 0.85f;


    // Настройки, действующие на станцию добычи дерева
    [Space]
    [Header("Wood Settings")]
    [Space]
    [SerializeField, Tooltip("Фактор увеличения объема добычи дерева, при повышении уровня")] 
    private float[] woodProductionFactorOnLevel = new float[] {1, 2, 4, 7};


    // Настройки, действующие на станцию сортировки
    [Space]
    [Header("Sort Settings")]
    [Space]
    private string _resSortType;
    private int _resSortInd;
    private int _resSortValue;

    [SerializeField, Tooltip("Счетчик количества сортируемых ресурсов")]
    private TextMeshProUGUI countSortText;


    private ResourceStorage _storage;
    private ResourcePanelEvents _resourcePanelEvents;
    private GameObject _player;
    private int _workLevel = 1;
    private bool _working = false;
    private float _timer = 0;

    private void Start()
    {
        _resourcePanelEvents = FindFirstObjectByType<ResourcePanelEvents>();
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

        if (isSawmill)
        {
            ResourceOut(woodProductionFactorOnLevel[_workLevel] * normalResourceDrop);
        }

        if (isSorter)
        {
            SortResourceOut();

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

        if (isWork)
        {
            if (isSorter)
            {
                _resSortValue = 0;
                countSortText.text = _resSortValue.ToString();
                mainPanel.SetActive(true);
            }
            if (isDelivery)
            {

            }
            else
            {
                if (workTimerImage != null)
                    workTimerImage.gameObject.SetActive(true);
                _working = true;
            }
        }
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
                resSc.SetResourceValue(index, valueWithRare);

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
                resSc.SetResourceValue(value);

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
    /// Метод вызывается нажатием кнопки на UI панели сортировочной станции. Тем самым запускается работа сортировки и закрывается UI панель.
    /// </summary>
    public void DoSort()
    {
        if (workTimerImage != null)
            workTimerImage.gameObject.SetActive(true);

        if (_resSortValue >= 10)
        {
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
                resSc.SetResourceValue(_resSortType, _resSortInd, _resSortValue);

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
    /// Получение выбранного в инвентаре ресурса, с которым будет происходить взаимодействие
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
    /// Возврат значений в стандартное состояние
    /// </summary>
    public void ReturnDefaultSortValues()
    {
        _resSortValue = 0;
        countSortText.text = "0";
    }
    #endregion
}
