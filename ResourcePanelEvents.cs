using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelEvents : MonoBehaviour
{
    [SerializeField] private StationsScript sortStation;

    [SerializeField, Tooltip("������ ����� �� ������ �������� ���������� (������ ��������������� ���������� � ������� ��������!).")] 
    private GameObject[] oreButton = new GameObject[7];

    [SerializeField, Tooltip("������ ������� �� ������ �������� ���������� (������ ��������������� ���������� � ������� ��������!).")] 
    private GameObject[] ingotButton = new GameObject[4];

    [SerializeField, Tooltip("������ ������� �� ������ �������� ���������� (������ ��������������� ���������� � ������� ��������!).")] 
    private GameObject[] productButton = new GameObject[4];

    private string _activeResType;
    private Image _activeResImage;
    private int _activeResIndex;
    private GameObject _currentResButton = null;

    public void SelectActiveResource(GameObject resourceImageObject)
    {
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

    public Sprite GetResourceSprite(string resourceType, int resourceIndex)
    {
        switch (resourceType)
        {
            case "Ore":
                return oreButton[resourceIndex].GetComponent<Image>().sprite;

            case "Ingot":
                return ingotButton[resourceIndex].GetComponent<Image>().sprite;

            case "Product":
                return productButton[resourceIndex].GetComponent<Image>().sprite;

            default: return null;
        }
    }
}
