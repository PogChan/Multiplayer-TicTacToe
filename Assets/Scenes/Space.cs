using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Space : NetworkBehaviour
{
    public Button button;
    public Text buttonText;

    [SyncVar(hook = nameof(SetSpace))] private string buttText;
    private GameController gameController;

    public void SetControllerReference(GameController control)
    {
        gameController = control;
    }

   
    //gameController.EndTurn();
    [Command]
    public void CmdOnCellClick()
    {
        Debug.Log("Just clicked on Cell " + button.name);
        this.SetSpace(this.buttonText.text, gameController.GetSide());

        gameController.EndTurn();
    }
    public void SetSpace(string oldValue, string newValue)
    {
        buttonText.text = newValue;
        button.interactable = false;
    }
}
