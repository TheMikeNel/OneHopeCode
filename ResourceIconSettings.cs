using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс панели ресурса, отправляемого на продажу. Отображает количество, стоимость и спрайт ресурса в UI станции доставки
/// </summary>
public class ResourceIconSettings : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public TextMeshProUGUI costText;

    public void SetIcon(int value, float cost, Sprite sprite)
    {
        if (value != 0)
        {
            costText.text = cost.ToString("#.#");
        }
        else costText.gameObject.SetActive(false);

        countText.text = value.ToString();
        gameObject.GetComponent<Image>().sprite = sprite;
    }
}
