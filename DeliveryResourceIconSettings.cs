using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс панели ресурса, отправляемого на продажу. Отображает количество, стоимость и спрайт ресурса в UI станции доставки
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
