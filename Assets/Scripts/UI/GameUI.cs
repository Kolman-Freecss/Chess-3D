using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; set; }

    [SerializeField] private Animator menuAnimator;

    private void Awake() 
    {
        Instance = this;
    }

    public void OnLocalGameButton()
    {
        menuAnimator.SetTrigger("InGameMenu");
    }

    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }
    
    public void OnOnlineHostButton()
    {
        menuAnimator.SetTrigger("HostMenu");
    }

    public void OnOnlineConnectButton()
    {
        Debug.Log("Online Connect Button Clicked"); // $$
    }

    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }

    public void OnHostBackButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }
}
