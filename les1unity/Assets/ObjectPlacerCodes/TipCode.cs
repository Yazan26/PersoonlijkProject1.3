using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TipCode : MonoBehaviour
{
public Button TipButton;
public GameObject TipPanel;

    public void Start()
    {
        TipButton.onClick.AddListener(OnTipButtonClick);
    }
    
    private void OnTipButtonClick()
    {
        TipPanel.SetActive(!TipPanel.activeSelf);
    }
    
}
