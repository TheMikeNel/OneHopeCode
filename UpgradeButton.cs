using TMPro;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI thisCostText;
    [SerializeField] private StationsScript workshop;
    [Space]

    [SerializeField] private bool isTool = false;
    [SerializeField] private float[] costOnToolLevels = { 50f, 150f, 300f };
    [Space]

    [SerializeField] private bool isStation = false;
    [SerializeField] private StationsScript upgradeStation;
    [SerializeField] private float costOfUpgrade;
    [SerializeField, Tooltip("Множитель стоимости улучшения (при каждом улучшении, стоимость увеличивается на этот фактор")] 

    public bool isActiveUpgrade = false;

    private void OnEnable()
    {
        if (isTool)
        {
            costOfUpgrade = costOnToolLevels[workshop.workLevel - 1];
        }
        thisCostText.text = costOfUpgrade.ToString("#.#");

    }

    public void ClickUpgradeButton()
    {
        if (!isActiveUpgrade)
        {
            SelectButton();
        }

        else
        {
            DeselectButton();
        }
    }

    public void SelectButton()
    {
        if (!isActiveUpgrade)
        {
            if (isTool)
            {
                workshop.SetUpgradeToolLevel(costOfUpgrade, workshop.workLevel + 1);
            }

            else if (isStation)
            {
                workshop.SetUpgradeStation(costOfUpgrade, upgradeStation);
            }
            isActiveUpgrade = true;
        }
    }

    public void DeselectButton()
    {
        if(isActiveUpgrade)
        {
            if (isTool)
            {
                workshop.SetUpgradeToolLevel(-costOfUpgrade, -1);
            }

            else if (isStation)
            {
                workshop.SetUpgradeStation(-costOfUpgrade, upgradeStation);
            }
            isActiveUpgrade = false;
        }
    }
}
