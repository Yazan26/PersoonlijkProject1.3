using UnityEngine;
using UnityEngine.SceneManagement;

public class scene : MonoBehaviour
{
    public void LaadScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
