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
    int index = 0;

    [SerializeField, Tooltip("��� ������� (�������� �����")]
    public string typeOfResource;

    [SerializeField, Tooltip("���������� ������� � ����� ���������� �������.")]
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
    /// ��������� �������� ������� ��� ��� �������� � ������� ��������.
    /// </summary>
    /// <param name="tag">��� �������.</param>
    /// <param name="resourceindex">������ ������� � ������� ������� �������� ��������.</param>
    /// <param name="value">���������� ������� � ����� ���������� �������.</param>
    public void SetResourceValue(string tag, int resourceindex, float value)
    {
        typeOfResource = tag;
        index = resourceindex;
        this.value = value;
    }

    /// <summary>
    /// ��������� �������� ������� ��� ��� �������� � ������� ��������.
    /// </summary>
    /// <param name="resourceIndex">������ ������� � ������� ������� �������� ��������.</param>
    /// <param name="value">���������� ������� � ����� ���������� �������.</param>
    public void SetResourceValue(int resourceIndex, float value) 
    {
        index = resourceIndex;
        this.value = value; 
    }

    /// <summary>
    /// ��������� �������� ������� ��� ��� �������� � ������� ��������.
    /// </summary>
    /// <param name="value">���������� ������� � ����� ���������� �������.</param>
    public void SetResourceValue(float value) 
    { 
        this.value = value; 
    }

    /// <summary>
    /// ��������� ������������ ���� � �������
    /// </summary>
    /// <param name="forceVector">������ ����.</param>
    public void AddForceToResourceOut(Vector3 forceVector)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Impulse);
    }
}
