using System;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        if (gameOverPanel.activeInHierarchy) gameOverPanel.SetActive(false);
        GameManager.Instance.SendOnGameOverStatusChangedCallback(OnGameOverStatusChanged);
    }

    private void OnGameOverStatusChanged()
    {
        gameOverPanel.SetActive(GameManager.Instance.IsGameOver());
    }
}
