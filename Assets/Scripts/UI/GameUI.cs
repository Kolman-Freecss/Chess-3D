using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; set; }

    private void Awake() 
    {
        Instance = this;
    }

    public void OnLocalGameButton()
    {
        Debug.Log("Local Game Button Clicked");
    }

    public void OnOnlineGameButton()
    {
        Debug.Log("Online Game Button Clicked");
    }
    
    public void OnOnlineHostButton()
    {
        Debug.Log("Online Host Button Clicked");
    }

    public void OnOnlineConnectButton()
    {
        Debug.Log("Online Connect Button Clicked");
    }

    public void OnOnlineBackButton()
    {
        Debug.Log("Online Back Button Clicked");
    }

    public void OnHostBackButton()
    {
        Debug.Log("Host Back Button Clicked");
    }
}
