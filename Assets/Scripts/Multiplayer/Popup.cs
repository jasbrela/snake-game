using System;
using System.Collections;
using Enums;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI popupMessage;
    [SerializeField] private TextMeshProUGUI headerTitle;
    
    [Header("Messages")]
    [TextArea(2, 10)] [SerializeField] private string addNewPlayerMessage;
    [TextArea(2, 10)] [SerializeField] private string duplicateMessage;
    [TextArea(2, 10)] [SerializeField] private string notEnoughPlayers;

    [Header("Buttons")]
    [SerializeField] private GameObject cancelMessage;
    [SerializeField] private GameObject okButton;

    /// <summary>
    /// Show the duplicate's message then go back to the add new player's message.
    /// </summary>
    public void ShowDuplicateMessage(string path)
    {
        headerTitle.text = PopupType.Warning.ToString();

        string key = path.Replace("<Keyboard>/", String.Empty).ToUpper();
        popupMessage.text = duplicateMessage.Replace("{key}", $"[{key}]");
        if (gameObject.activeInHierarchy) StartCoroutine(ShowNewPlayerMessageAgain());
    }

    /// <summary>
    /// Show the add new player's message after 5 seconds
    /// </summary>
    private IEnumerator ShowNewPlayerMessageAgain()
    {
        yield return new WaitForSeconds(5);
        OnAddNewPlayerShowMessage();
    }

    /// <summary>
    /// Show a message to notify the player that there's not enough players to start the game.
    /// </summary>
    public void ShowNotEnoughPlayersMessage(int minimum)
    {
        headerTitle.text = PopupType.Warning.ToString();

        okButton.SetActive(true);
        cancelMessage.SetActive(false);
        popupMessage.text = notEnoughPlayers.Replace("{minimum}", minimum.ToString());
        
        ShowPopup();
    }

    /// <summary>
    /// Hide popup and reset the buttons to default.
    /// </summary>
    public void OnClickOk()
    {
        HidePopup();
        
        okButton.SetActive(false);
        cancelMessage.SetActive(true);
    }

    /// <summary>
    /// Show the add new player's message.
    /// </summary>
    public void OnAddNewPlayerShowMessage()
    {
        headerTitle.text = PopupType.Information.ToString();
        popupMessage.text = addNewPlayerMessage;
        
        if (!panel.activeSelf) ShowPopup();
    }

    /// <summary>
    /// Show the popup
    /// </summary>
    private void ShowPopup()
    {
        panel.SetActive(true);
    }

    /// <summary>
    /// Hide the popup
    /// </summary>
    public void HidePopup()
    {
        panel.SetActive(false);
    }

}
