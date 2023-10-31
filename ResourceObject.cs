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
    int index = 0;

    [SerializeField, Tooltip("Тип ресурса (задается тегом")]
    public string typeOfResource;

    [SerializeField, Tooltip("Количество ресурса в одном экзкмпляре объекта.")]
    float value = 1;

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
                Vector3.MoveTowards(transform.position, _storage.playerHandsPosition.transform.position, 0.1f);
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                transform.SetParent(_storage.playerHandsPosition.transform);
                gameObject.transform.localRotation = _storage.playerHandsPosition.transform.localRotation;
                gameObject.transform.localPosition = _storage.playerHandsPosition.transform.localPosition;

            }
        }
    }

    /// <summary>
    /// Установка настроек ресурса для его передачи в систему хранения.
    /// </summary>
    /// <param name="tag">Тип ресурса.</param>
    /// <param name="resourceindex">Индекс ресурса в массиве системы хранения ресурсов.</param>
    /// <param name="value">Количество ресурса в одном экзкмпляре объекта.</param>
    public void SetResourceValue(string tag, int resourceindex, float value)
    {
        typeOfResource = tag;
        index = resourceindex;
        this.value = value;
    }

    /// <summary>
    /// Установка настроек ресурса для его передачи в систему хранения.
    /// </summary>
    /// <param name="resourceIndex">Индекс ресурса в массиве системы хранения ресурсов.</param>
    /// <param name="value">Количество ресурса в одном экзкмпляре объекта.</param>
    public void SetResourceValue(int resourceIndex, float value) 
    {
        index = resourceIndex;
        this.value = value; 
    }

    /// <summary>
    /// Установка настроек ресурса для его передачи в систему хранения.
    /// </summary>
    /// <param name="value">Количество ресурса в одном экзкмпляре объекта.</param>
    public void SetResourceValue(float value) 
    { 
        this.value = value; 
    }

    /// <summary>
    /// Применить направленную силу к объекту
    /// </summary>
    /// <param name="forceVector">Вектор силы.</param>
    public void AddForceToResourceOut(Vector3 forceVector)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }
}
