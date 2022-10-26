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

    public IEnumerator SetDialog(string message)
    {
        dialogText.text = "";
        //Con este foreach escribo el texto letra a letra de manera lenta
        foreach(var character in message)
        {
            dialogText.text += character;
            yield return new WaitForSeconds(1 / characterPerSecond);
        }
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
}
