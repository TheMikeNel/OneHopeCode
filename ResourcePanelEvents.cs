using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelEvents : MonoBehaviour
{
    [SerializeField] private StationsScript sortStation;

    [SerializeField] private GameObject[] oreButton = new GameObject[7];
    [SerializeField] private GameObject[] ingotButton = new GameObject[4];
    [SerializeField] private GameObject[] productButton = new GameObject[4];

    private string _activeResType; 
    private Image _activeResImage;
    private int _activeResIndex;
    private GameObject _currentResButton = null;

    public void SelectActiveResource(GameObject resourceImageObject)
    {
        Debug.Log(resourceImageObject.name + " ResourceSelected");

        if (resourceImageObject != _currentResButton)
        {
            for (int j = 0; j < oreButton.Length; j++)
            {
                if (resourceImageObject == oreButton[j])
                {
                    _activeResIndex = j;
                    _activeResType = "Ore";
                    _currentResButton = resourceImageObject;
                    break;
                }
            }
        }

        if (resourceImageObject != _currentResButton)
        {
            for (int j = 0; j < ingotButton.Length; j++)
            {
                if (resourceImageObject == ingotButton[j])
                {
                    _activeResIndex = j;
                    _activeResType = "Ingot";
                    _currentResButton = resourceImageObject;
                    break;
                }
            }
        }

        if (resourceImageObject != _currentResButton)
        {
            for (int j = 0; j < productButton.Length; j++)
            {
                if (resourceImageObject == productButton[j])
                {
                    _activeResIndex = j;
                    _activeResType = "Product";
                    _currentResButton = resourceImageObject;
                    break;
                }
            }
        }
        
        _activeResImage = resourceImageObject.GetComponent<Image>();

        if (sortStation.mainPanel.activeSelf == true)
        {
            sortStation.ReturnDefaultSortValues();
            sortStation.SetSettingsOfSelectedResource(_activeResImage, _activeResType, _activeResIndex);
        }
        Debug.Log($"Resource Type: {_activeResType};  Index: {_activeResIndex}.");
    }

    public int GetActiveResourceIndexInStorage()
    {
        return _activeResIndex;
    }

    public string GetActiveResourceType()
    {
        return _activeResType;
    }

    public Image GetActiveResourceImage()
    {
        return _activeResImage;
    }
}
