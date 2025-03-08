using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyCalculator : MonoBehaviour
{
    public string keuzespeler;
    public string keuzecpu;
    public TextMeshProUGUI resultaat;
    public TextMeshProUGUI cpukeuze;
    public GameObject textGameObject;

    public void ShowTotal()
    {

    }

    private void Resultaat()
    {
        switch (keuzespeler)
        {
            case "steen":
                if (keuzecpu == "steen")
                {
                    resultaat.text = "Gelijkspel";
                }
                else if (keuzecpu == "papier")
                {
                    resultaat.text = "Je hebt verloren";
                }
                else if (keuzecpu == "schaar")
                {
                    resultaat.text = "Je hebt gewonnen";
                }
                break;
            case "papier":
                if (keuzecpu == "steen")
                {
                    resultaat.text = "Je hebt gewonnen";
                }
                else if (keuzecpu == "papier")
                {
                    resultaat.text = "Gelijkspel";
                }
                else if (keuzecpu == "schaar")
                {
                    resultaat.text = "Je hebt verloren";
                }
                break;
            case "schaar":
                if (keuzecpu == "steen")
                {
                    resultaat.text = "Je hebt verloren";
                }
                else if (keuzecpu == "papier")
                {
                    resultaat.text = "Je hebt gewonnen";
                }
                else if (keuzecpu == "schaar")
                {
                    resultaat.text = "Gelijkspel";
                }
                break;
            case "":
                resultaat.text = "is thinking";
                break;
        }
    }

    public string randomkeuzecpu()
    {
        int random = UnityEngine.Random.Range(1, 4);

        switch (random)
        {
            case 1:
                return "steen";
            case 2:
                return "papier";
            case 3:
                return "schaar";
            default:
                return "";
        }
    }

    public void steenknop()
    {
        keuzespeler = "steen";
        keuzecpu = randomkeuzecpu();
        cpukeuze.text = keuzecpu;
        Debug.Log(keuzespeler);
        Debug.Log(keuzecpu);
        resultaat.text = keuzecpu;
        Resultaat();
    }

    public void papierknop()
    {
        keuzespeler = "papier";
        keuzecpu = randomkeuzecpu();
        cpukeuze.text = keuzecpu;
        Debug.Log(keuzespeler);
        Debug.Log(keuzecpu);
        resultaat.text = keuzecpu;
        Resultaat();
    }

    public void schaarknop()
    {
        keuzespeler = "schaar";

        keuzecpu = randomkeuzecpu();
        cpukeuze.text = keuzecpu;

        Debug.Log(keuzespeler);
        Debug.Log(keuzecpu);
        resultaat.text = keuzecpu;
        Resultaat();
    }

    public void ResetKnop()
    {
        Debug.Log("reseted");
        keuzespeler = "";
        keuzecpu = "";
        resultaat.text = "is thinking";
    }
}