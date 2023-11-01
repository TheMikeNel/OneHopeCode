using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� ������ �������, ������������� �� �������. ���������� ����������, ��������� � ������ ������� � UI ������� ��������
/// </summary>
public class DeliveryResourceIconSettings : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public TextMeshProUGUI costText;

    public void SetIcon(int value, float cost, Sprite sprite)
    {
        countText.text = value.ToString();
        costText.text = cost.ToString("#.#");
        gameObject.GetComponent<Image>().sprite = sprite;
    }
}
