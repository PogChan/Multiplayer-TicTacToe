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

    [SyncVar(hook = nameof(SetSpace))] private string buttText;


    public void SetControllerReference(GameController control)
    {
        gameController = control;
    }

    //gameController.EndTurn();
    [Command(requiresAuthority = false)]
    public void CmdOnCellClick()
    {
        Debug.Log("Just clicked on Cell " + button.name);
       
        buttText = gameController.GetSide();
        gameController.EndTurn();
    }
    public void SetSpace(string oldValue, string newValue)
    {
        buttText = newValue;
        buttonText.text = newValue;
        button.interactable = false;
    }
}