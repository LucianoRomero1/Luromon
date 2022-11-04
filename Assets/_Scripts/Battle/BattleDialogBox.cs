using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private Text dialogText;

    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject movementSelect;
    [SerializeField] private GameObject movementDescription;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> movementTexts;
    
    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;

    [SerializeField] private float characterPerSecond = 10.0f;
    [SerializeField] private float timeToWaitAfterText = 1.0f;

    [SerializeField] Color selectedColor = Color.blue;

    [SerializeField] private bool isWriting = false;

    public bool IsWriting => isWriting;

    public IEnumerator SetDialog(string message)
    {
        isWriting = true;

        dialogText.text = "";
        //Con este foreach escribo el texto letra a letra de manera lenta
        foreach(var character in message)
        {
            dialogText.text += character;
            yield return new WaitForSeconds(1 / characterPerSecond);
        }

        yield return new WaitForSeconds(timeToWaitAfterText);
        isWriting = false;
    }

    public void ToggleDialogText(bool activated)
    {
        dialogText.enabled = activated;
    }
    
    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }

    public void ToggleMovements(bool activated)
    {
        movementSelect.SetActive(activated);
        movementDescription.SetActive(activated);
    }

    public void SelectAction(int selectedAction){
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = (i == selectedAction ? selectedColor : Color.black);
        }
    }

    public void SetPokemonMovements(List<Move> moves){
        for (int i = 0; i < movementTexts.Count; i++)
        {   
            if(i < moves.Count){
                movementTexts[i].text = moves[i].Base.Name;
            }else{
                movementTexts[i].text = "--";
            }
        }
    }

    public void SelectMovement(int selectedMovement, Move move){
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].color = (i == selectedMovement ? selectedColor : Color.black);
        }

        ppText.text = $"PP {move.Pp}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString().ToUpper();

        ppText.color = (move.Pp <= 0 ? Color.red : Color.black);
    }

    
}
