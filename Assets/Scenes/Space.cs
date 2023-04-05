using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Space : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetSpace))] public Button button;
    [SyncVar(hook = nameof(SetSpace))] public Text buttonText;
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
        this.SetSpace();

        gameController.EndTurn();
    }
    public void SetSpace()
    {
        buttonText.text = gameController.GetSide();
        button.interactable = false;
    }
}
