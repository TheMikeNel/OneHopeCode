using TMPro;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    [SerializeField] public GameObject playerHands;
    [SerializeField] private float woodValue = 0f;
    [SerializeField] private TextMeshProUGUI woodIndicator;

    [SerializeField] private float[] oreValue = new float[] { 0, 0, 0, 0, 0, 0, 0 };
    [SerializeField] private TextMeshProUGUI[] oreIndicator = new TextMeshProUGUI[7];

    [SerializeField] private float[] ingotValue = new float[] { 0, 0, 0, 0 };
    [SerializeField] private TextMeshProUGUI[] ingotIndicator = new TextMeshProUGUI[4];

    [SerializeField] private float[] productValue = new float[] { 0, 0, 0, 0 };
    [SerializeField] private TextMeshProUGUI[] productIndicator = new TextMeshProUGUI[4];

    public float coins = 0;
    [SerializeField] private TextMeshProUGUI coinsIndicator;

    private void Start()
    {
        SetIndications();
    }

    private void SetIndications()
    {
        woodIndicator.text = woodValue.ToString("#");

        for (int i = 0; i < oreIndicator.Length; i++)
        {
            oreIndicator[i].text = oreValue[i].ToString("#");
        }

        for (int i = 0; i < ingotIndicator.Length; i++)
        {
            ingotIndicator[i].text = ingotValue[i].ToString("#");
        }

        for (int i = 0; i < productIndicator.Length; i++)
        {
            productIndicator[i].text = productValue[i].ToString("#");
        }

        coinsIndicator.text = coins.ToString("#");
    }

    public void AddWood(float value) { woodValue += value; woodIndicator.text = woodValue.ToString("#"); }

    public void AddCoins(float value) { coins += value; ; coinsIndicator.text = coins.ToString("#"); }

    //Добавление ресурсов по их индексу
    public void AddOre(int index, float value) { oreValue[index] += value; oreIndicator[index].text = oreValue[index].ToString("#"); }

    public void AddIngot(int index, float value) { ingotValue[index] += value; ingotIndicator[index].text = ingotValue[index].ToString("#"); }

    public void AddProduct(int index, float value) { productValue[index] += value; productIndicator[index].text = productValue[index].ToString("#"); }


    public float GetWood() { return woodValue;}

    public float GetOre(int index) { return oreValue[index]; }

    public float GetIngot(int index) { return ingotValue[index]; }

    public float GetProduct(int index) { return productValue[index]; }

    public float GetCoins() { return coins; }

    public void AddResourceByType(string tag, int index, float value)
    {
        switch (tag)
        {
            case "Ore":
                oreValue[index] += value;
                oreIndicator[index].text = oreValue[index].ToString("#");
                break;

            case "Ingot":
                ingotValue[index] += value;
                ingotIndicator[index].text = ingotValue[index].ToString("#");
                break;

            case "Product":
                productValue[index] += value;
                productIndicator[index].text = productValue[index].ToString("#");
                break;
        }
    }

    public float GetResourceByType(string tag, int index)
    {
        switch (tag)
        {
            case "Ore":
                return oreValue[index];
            case "Ingot":
                return ingotValue[index];
            case "Product":
                return productValue[index];
            default:
                return 0;
        }
    }
}
