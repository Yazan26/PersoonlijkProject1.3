using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;
    public Object2DApiClient objectApiClient;
    
    // Lijst met objecten die geplaatst zijn in de wereld
    private List<GameObject> placedObjects;

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        UISideMenu.SetActive(false);

        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);

        DraggingObject2D objects2D = instanceOfPrefab.GetComponent<DraggingObject2D>();

        objects2D.objectManager = this;
        objects2D.apiClient = objectApiClient; // ✅ hier geef je de API client mee

        objects2D.isDragging = true;
    }
    
    
    
    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

    // Methode om de huidige sc�ne te resetten
    public void Reset()
    {
        // Laad de huidige sc�ne opnieuw
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
