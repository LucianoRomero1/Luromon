using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    StartBattle,
    PlayerSelectAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private BattleUnit playerUnit;

    [SerializeField]
    private BattleHUD playerHUD;

    [SerializeField]
    private BattleUnit enemyUnit;

    [SerializeField]
    private BattleHUD enemyHUD;

    [SerializeField]
    private BattleDialogBox battleDialogBox;

    [SerializeField]
    private BattleState state;

    private int currentSelectedAction;
    private int currentSelectedMovement;

    private float timeSinceLastClick;

    private float timeBetweenClicks = 0.3f;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        playerUnit.SetupPokemon();
        playerHUD.SetPokemonData(playerUnit.Pokemon);

        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);

        enemyUnit.SetupPokemon();
        enemyHUD.SetPokemonData(enemyUnit.Pokemon);

        //No arranco otra corrutina sino que espero que se ejecute esa corrutina para hacer otra cosa
        yield return battleDialogBox
                .SetDialog($"A wild {enemyUnit.Pokemon.Base.Name} has appeared!");
        yield return new WaitForSeconds(1.0f);

        //Defino quien ataca primero
        if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            StartCoroutine(battleDialogBox
                .SetDialog($"The enemy attack first"));
            EnemyAction();
        }
        else
        {
            PlayerAction();
        }
    }

    private void PlayerAction()
    {
        state = BattleState.PlayerSelectAction;
        StartCoroutine(battleDialogBox.SetDialog($"Select an action..."));
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleMovements(false);
        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    private void PlayerMovement(){
        state = BattleState.PlayerMove;
        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement);
    }

    public void EnemyAction()
    {
    }

    private void Update()
    {
        timeSinceLastClick += Time.deltaTime;

        if (state == BattleState.PlayerSelectAction){
            HandlePlayerSelection();
        }else if(state == BattleState.PlayerMove){
            HandlePlayerMovementSelection();
        }
    }

    private void HandlePlayerSelection()
    {
        //Verifico con una variable de tiempo para que no me saltee tan rapido de opciones
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }
        else
        {
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                timeSinceLastClick = 0;
                
                //Esto va a dar siempre 0 o 1 por el calculo matematico del modulo
                currentSelectedAction = (currentSelectedAction+1) % 2;

                battleDialogBox.SelectAction(currentSelectedAction);
            }

            if(Input.GetAxisRaw("Submit") != 0){
                timeSinceLastClick = 0;

                if(currentSelectedAction == 0){
                    PlayerMovement();
                }else if(currentSelectedAction == 1){
                    //TODO: ESCAPE
                }
            }
        }
    }

    private void HandlePlayerMovementSelection(){
        if(timeSinceLastClick < timeBetweenClicks){
            return;
        }

        if(Input.GetAxisRaw("Vertical") != 0){
            timeSinceLastClick = 0;
            currentSelectedMovement = (currentSelectedMovement + 2) % 4;
            battleDialogBox.SelectMovement(currentSelectedMovement);
        }else if(Input.GetAxisRaw("Horizontal") != 0){
            timeSinceLastClick = 0;
            if(currentSelectedMovement <= 1){
                currentSelectedMovement = (currentSelectedMovement + 1) % 2;
            }else{
                currentSelectedMovement = (currentSelectedMovement + 1) % 2 + 2;
            }
            battleDialogBox.SelectMovement(currentSelectedMovement);
        } 
        

    }
}
