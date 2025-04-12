using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TipCode : MonoBehaviour
{
public Button TipButton;
public GameObject TipPanel;
[CanBeNull] public Button Backbutton;

    public void Start()
    {
        TipButton.onClick.AddListener(OnTipButtonClick);
        Backbutton.onClick.AddListener(back);
    }

    public void back()
    {
        SceneManager.LoadScene("WorldSelector");
    }
    private void OnTipButtonClick()
    {
        TipPanel.SetActive(!TipPanel.activeSelf);
    }
    
}
