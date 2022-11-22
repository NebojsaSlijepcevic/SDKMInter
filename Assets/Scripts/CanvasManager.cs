using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject mainMenuPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
}
