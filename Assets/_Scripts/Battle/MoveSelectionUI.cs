using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] private Text[] movementTexts;
    [SerializeField] private Color selectedColor;
    private int _currentSelectedMovement = 0;

    public void SetMovements(List<MoveBase> pokemonMoves, MoveBase newMove){

        _currentSelectedMovement = 0;

        for (int i = 0; i < pokemonMoves.Count; i++)
        {
            movementTexts[i].text = pokemonMoves[i].Name;
        }

        movementTexts[pokemonMoves.Count].text = newMove.Name;
    }

    public void HandleForgetMoveSelection(Action<int> onSelected){
        if(Input.GetAxisRaw("Vertical") != 0){
            int direction = Mathf.FloorToInt(Input.GetAxisRaw("Vertical"));
            _currentSelectedMovement -= direction;
            //Pasarle un -1 es decir que cambió de acción
            onSelected?.Invoke(-1);
        }

        _currentSelectedMovement = Mathf.Clamp(_currentSelectedMovement, 0, PokemonBase.NUMBER_OF_LEARNABLE_MOVES);
        UpdateColorForgetMoveSelection(_currentSelectedMovement);

        if(Input.GetAxisRaw("Submit") != 0){
            onSelected?.Invoke(_currentSelectedMovement);
        }
    }

    public void UpdateColorForgetMoveSelection(int selectedMove){
        for (int i = 0; i <= PokemonBase.NUMBER_OF_LEARNABLE_MOVES; i++)
        {
            movementTexts[i].color = (i == selectedMove ? selectedColor : Color.black);
        }
    }
}
