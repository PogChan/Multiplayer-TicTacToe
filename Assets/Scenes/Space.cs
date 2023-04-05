using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class Space : NetworkBehaviour
{
    public Button button;
    public Text buttonText;
    private GameController gameController;

    [SyncVar(hook = nameof(SetSpaceInteractable))] private bool buttInteractable;
    [SyncVar(hook = nameof(SetSpaceText))] private string buttText;


    public void SetControllerReference(GameController control)
    {
        gameController = control;
    }

    //gameController.EndTurn();
    [Command(requiresAuthority = false)]
    public void CmdOnCellClick()
    {
        Debug.Log("Just clicked on Cell " + button.name);
        buttInteractable = false;
        buttText = gameController.GetSide();
        gameController.EndTurn();
    }

    public void SetSpaceText(string oldValue, string newValue)
    {
        buttText = newValue;
        buttonText.text = newValue;
    }

    public void SetSpaceInteractable(bool oldValue, bool newValue)
    {
        buttInteractable = newValue;
        button.interactable = newValue;
    }
}