using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class ChangeInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    EventSystem system;
    void Start()
    {
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (next != null)
                {
                    next.Select();
                }
            }

        }
    }
}