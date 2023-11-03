using TMPro;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    public string indicationFormat = "#";
    public GameObject playerHands;

    [SerializeField] private float woodValue = 0f;
    [SerializeField] private TextMeshProUGUI woodIndicator;

    public float coins = 0;
    [SerializeField] private TextMeshProUGUI coinsIndicator;

    [SerializeField] private float[] oreValue = new float[] { 0, 0, 0, 0, 0, 0, 0 };
    [SerializeField] private TextMeshProUGUI[] oreIndicator = new TextMeshProUGUI[7];

    [SerializeField] private float[] ingotValue = new float[] { 0, 0, 0, 0 };
    [SerializeField] private TextMeshProUGUI[] ingotIndicator = new TextMeshProUGUI[4];

    [SerializeField] private float[] productValue = new float[] { 0, 0, 0, 0 };
    [SerializeField] private TextMeshProUGUI[] productIndicator = new TextMeshProUGUI[4];

    private void LoadSavedResources()
    {
        if (PlayerPrefs.HasKey("Wood")) woodValue = PlayerPrefs.GetFloat("Wood");
        if (PlayerPrefs.HasKey("Coins")) coins = PlayerPrefs.GetFloat("Coins");

        for (int i = 0; i < oreValue.Length; i++)
        {
            if (PlayerPrefs.HasKey("Ore" + i)) oreValue[i] = PlayerPrefs.GetFloat("Ore" + i);
        }

        for (int i = 0; i < ingotValue.Length; i++)
        {
            if (PlayerPrefs.HasKey("Ingot" + i)) ingotValue[i] = PlayerPrefs.GetFloat("Ingot" + i);
        }

        for (int i = 0; i < productValue.Length; i++)
        {
            if (PlayerPrefs.HasKey("Product" + i)) ingotValue[i] = PlayerPrefs.GetFloat("Product" + i);
        }
    }

    private void Start()
    {
        LoadSavedResources();
        SetIndications();
    }

    private void SetIndications()
    {
        woodIndicator.text = woodValue.ToString(indicationFormat);

        for (int i = 0; i < oreIndicator.Length; i++)
        {
            oreIndicator[i].text = oreValue[i].ToString(indicationFormat);
        }

        for (int i = 0; i < ingotIndicator.Length; i++)
        {
            ingotIndicator[i].text = ingotValue[i].ToString(indicationFormat);
        }

        for (int i = 0; i < productIndicator.Length; i++)
        {
            productIndicator[i].text = productValue[i].ToString(indicationFormat);
        }

        coinsIndicator.text = coins.ToString(indicationFormat);
    }

    public void AddWood(float value) { woodValue += value; woodIndicator.text = woodValue.ToString(indicationFormat); }

    public void AddCoins(float value) { coins += value; ; coinsIndicator.text = coins.ToString(indicationFormat); }

    //Добавление ресурсов по их индексу
    public void AddOre(int index, float value) { oreValue[index] += value; oreIndicator[index].text = oreValue[index].ToString(indicationFormat); }

    public void AddIngot(int index, float value) { ingotValue[index] += value; ingotIndicator[index].text = ingotValue[index].ToString(indicationFormat); }

    public void AddProduct(int index, float value) { productValue[index] += value; productIndicator[index].text = productValue[index].ToString(indicationFormat); }


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
                oreIndicator[index].text = oreValue[index].ToString(indicationFormat);
                break;

            case "Ingot":
                ingotValue[index] += value;
                ingotIndicator[index].text = ingotValue[index].ToString(indicationFormat);
                break;

            case "Product":
                productValue[index] += value;
                productIndicator[index].text = productValue[index].ToString(indicationFormat);
                break;
        }
    }

    public float GetResourceByType(string tag, int index)
    {
        return tag switch
        {
            "Ore" => oreValue[index],
            "Ingot" => ingotValue[index],
            "Product" => productValue[index],
            _ => 0,
        };
    }

    public void SaveResourcesOnExitGame()
    {
        PlayerPrefs.SetFloat("Wood", woodValue);
        PlayerPrefs.SetFloat("Coins", coins);

        for (int i = 0; i < oreValue.Length; i++)
        {
            PlayerPrefs.SetFloat("Ore" + i, oreValue[i]);
        }

        for (int i = 0; i < ingotValue.Length; i++)
        {
            PlayerPrefs.SetFloat("Ingot" + i, ingotValue[i]);
        }

        for(int i = 0; i < productValue.Length; i++)
        {
            PlayerPrefs.SetFloat("Product" + i, productValue[i]);
        }
    }
}
