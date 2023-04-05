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

    public void SetControllerReference(GameController control)
    {
        gameController = control;
    }

    //gameController.EndTurn();
    [Command(requiresAuthority = false)]
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