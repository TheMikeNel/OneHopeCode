using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ResourceObject : MonoBehaviour
{
    [SerializeField, Tooltip("true ���� ���������� ��������� ������ � ���� ������, false ���� ����� �������� ��� � ���������")]
    public bool addToPlayerHandsOrStorage = false;

    [SerializeField, Tooltip("������ ������� � ������� ������� �������� ��������:\n" +
    "������ - 0, ����� - 1, ������ - 2, ���� - 3, ����� - 4, ������ - 5, ������ - 6.\n" +
    "������: ����� - 0, ������ - 1, �������� - 2, ������ - 3.\n" +
    "������: ����� - 0, ����� - 1, ������� - 2, ��������� - 3.\n" +
    "��� ������� ������������ ��� ����� (Ore, Ingot, Product, Coin).")]
    public int index = 0;

    [SerializeField, Tooltip("��� ������� (�������� �����).")]
    public string typeOfResource;

    [SerializeField, Tooltip("���������� ������� � ����� ���������� �������.")]
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

    #region ���������� ������ ��������� �������� �������
    /// <summary>
    /// ��������� �������� �������.
    /// </summary>
    /// <param name="tag">��� �������.</param>
    /// <param name="resourceSprite">2D ������ �������.</param>
    /// <param name="resourceindex">������ ������� � ������� ������� �������� ��������.</param>
    /// <param name="value">���������� ������� � ����� ���������� �������.</param>
    public void SetResourceValues(string tag, int resourceindex, float value, Sprite resourceSprite)
    {
        _resourceSprite = resourceSprite;
        typeOfResource = tag;
        index = resourceindex;
        this.value = value;
    }

    /// <summary>
    /// ��������� �������� �������.
    /// </summary>
    /// <param name="tag">��� �������.</param>
    /// <param name="resourceindex">������ ������� � ������� ������� �������� ��������.</param>
    /// <param name="value">���������� ������� � ����� ���������� �������.</param>
    public void SetResourceValues(string tag, int resourceindex, float value)
    {
        typeOfResource = tag;
        index = resourceindex;
        this.value = value;
    }

    /// <summary>
    /// ��������� �������� �������.
    /// </summary>
    /// <param name="resourceIndex">������ ������� � ������� ������� �������� ��������.</param>
    /// <param name="value">���������� ������� � ����� ���������� �������.</param>
    public void SetResourceValues(int resourceIndex, float value) 
    {
        index = resourceIndex;
        this.value = value; 
    }

    /// <summary>
    /// ��������� �������� �������.
    /// </summary>
    /// <param name="value">���������� ������� � ����� ���������� �������.</param>
    public void SetResourceValues(float value) 
    { 
        this.value = value; 
    }
    #endregion

    /// <summary>
    /// ��������� ������������ ���� � �������
    /// </summary>
    /// <param name="forceVector">������ ����.</param>
    public void AddForceToResourceOut(Vector3 forceVector)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }

    /// <summary>
    /// ���������� 2D ������ �������� �������.
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
