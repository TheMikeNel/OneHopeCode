using UnityEngine;

[RequireComponent (typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ResourceObject : MonoBehaviour
{
    [Tooltip("true ���� ���������� ��������� ������ � ���� ������, false ���� ����� �������� ��� � ���������")]
    public bool addToPlayerHandsOrStorage = false;

    [Tooltip("������ ������� � ������� ������� �������� ��������:\n" +
    "������ - 0, ����� - 1, ������ - 2, ���� - 3, ����� - 4, ������ - 5, ������ - 6.\n" +
    "������: ����� - 0, ������ - 1, �������� - 2, ������ - 3.\n" +
    "������: ����� - 0, ����� - 1, ������� - 2, ��������� - 3.\n" +
    "��� ������� ������������ ��� ����� (Ore, Ingot, Product, Coin).")]
    public int index = 0;

    [Tooltip("��� ������� (�������� �����).")]
    public string typeOfResource;

    [Tooltip("���������� ������� � ����� ���������� �������.")]
    public float value = 1;

    private ResourceStorage _storage;

    private void OnEnable()
    {
        typeOfResource = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("NPC"))
        {
            if (other.gameObject.CompareTag("NPC"))
                _storage = GameObject.FindWithTag("Player").GetComponent<ResourceStorage>();

            else _storage = other.GetComponent<ResourceStorage>();

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
                GetComponent<Rigidbody>().isKinematic = true;
                transform.SetParent(_storage.playerHands.transform);
                transform.SetLocalPositionAndRotation(_storage.playerHands.transform.localPosition, _storage.playerHands.transform.localRotation);

            }
        }
    }

    #region ���������� ������ ��������� �������� �������

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
}
