using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ResourceObject : MonoBehaviour
{
    [SerializeField, Tooltip("true если необходимо поместить ресурс в руки игрока, false если нужно добавить его в инвентарь")]
    public bool addToPlayerHandsOrStorage = false;

    [SerializeField, Tooltip("Индекс ресурса в массиве системы хранения ресурсов:\n" +
    "Камень - 0, Уголь - 1, Железо - 2, Медь - 3, Олово - 4, Боксит - 5, Золото - 6.\n" +
    "Слитки: Сталь - 0, Бронза - 1, Алюминий - 2, Золото - 3.\n" +
    "Товары: Болты - 0, Трубы - 1, Каркасы - 2, Украшения - 3.\n" +
    "Тип ресурса определяется его тегом (Ore, Ingot, Product, Coin).")]
    public int index = 0;

    [SerializeField, Tooltip("Тип ресурса (задается тегом).")]
    public string typeOfResource;

    [SerializeField, Tooltip("Количество ресурса в одном экзкмпляре объекта.")]
    public float value = 1;

    private Sprite _resourceSprite;
    private ResourceStorage _storage;

    private void OnEnable()
    {
        typeOfResource = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _storage = other.GetComponent<ResourceStorage>();

            if (!addToPlayerHandsOrStorage)
            {
                switch (gameObject.tag)
                {
                    case "Wood":
                        _storage.AddWood(value); Destroy(gameObject);
                        break;

                    case "Ore":
                        _storage.AddOre(index, value); Destroy(gameObject);
                        break;

                    case "Ingot":
                        _storage.AddIngot(index, value); Destroy(gameObject);
                        break;

                    case "Product":
                        _storage.AddProduct(index, value); Destroy(gameObject);
                        break;

                    case "Coins":
                        _storage.AddCoins(value); Destroy(gameObject);
                        break;
                }
            }
            else
            {
                Vector3.MoveTowards(transform.position, _storage.playerHands.transform.position, 0.1f);
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                transform.SetParent(_storage.playerHands.transform);
                gameObject.transform.localRotation = _storage.playerHands.transform.localRotation;
                gameObject.transform.localPosition = _storage.playerHands.transform.localPosition;

            }
        }
    }

    #region Перегрузки метода установки настроек ресурса
    /// <summary>
    /// Установка настроек ресурса.
    /// </summary>
    /// <param name="tag">Тип ресурса.</param>
    /// <param name="resourceSprite">2D спрайт ресурса.</param>
    /// <param name="resourceindex">Индекс ресурса в массиве системы хранения ресурсов.</param>
    /// <param name="value">Количество ресурса в одном экзкмпляре объекта.</param>
    public void SetResourceValues(string tag, int resourceindex, float value, Sprite resourceSprite)
    {
        _resourceSprite = resourceSprite;
        typeOfResource = tag;
        index = resourceindex;
        this.value = value;
    }

    /// <summary>
    /// Установка настроек ресурса.
    /// </summary>
    /// <param name="tag">Тип ресурса.</param>
    /// <param name="resourceindex">Индекс ресурса в массиве системы хранения ресурсов.</param>
    /// <param name="value">Количество ресурса в одном экзкмпляре объекта.</param>
    public void SetResourceValues(string tag, int resourceindex, float value)
    {
        typeOfResource = tag;
        index = resourceindex;
        this.value = value;
    }

    /// <summary>
    /// Установка настроек ресурса.
    /// </summary>
    /// <param name="resourceIndex">Индекс ресурса в массиве системы хранения ресурсов.</param>
    /// <param name="value">Количество ресурса в одном экзкмпляре объекта.</param>
    public void SetResourceValues(int resourceIndex, float value) 
    {
        index = resourceIndex;
        this.value = value; 
    }

    /// <summary>
    /// Установка настроек ресурса.
    /// </summary>
    /// <param name="value">Количество ресурса в одном экзкмпляре объекта.</param>
    public void SetResourceValues(float value) 
    { 
        this.value = value; 
    }
    #endregion

    /// <summary>
    /// Применить направленную силу к объекту
    /// </summary>
    /// <param name="forceVector">Вектор силы.</param>
    public void AddForceToResourceOut(Vector3 forceVector)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }

    /// <summary>
    /// Возвращает 2D спрайт текущего ресурса.
    /// </summary>
    /// <returns></returns>
    public Sprite GetSprite()
    {
        if (_resourceSprite != null)
        {
            return _resourceSprite;
        }

        else
        {
            return FindFirstObjectByType<ResourcePanelEvents>().GetResourceSprite(typeOfResource, index);
        }
    }
}
