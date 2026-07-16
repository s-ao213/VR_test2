using UnityEngine;
using System;
public class UI_Button : MonoBehaviour
{
    public static event Action ClickStartButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStartButton()
    {
        ClickStartButton?.Invoke();
        Debug.Log("Start Button Clicked");
    }
}
