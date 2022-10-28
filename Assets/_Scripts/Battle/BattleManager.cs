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

    private void Update()
    {
        timeSinceLastClick += Time.deltaTime;

        if(battleDialogBox.IsWriting){
            //Hago esto para que no se me superpongan escrituras apretando enter, solo aparece otra si NO está escribiendo
            return;
        }

        if (state == BattleState.PlayerSelectAction){
            HandlePlayerSelection();
        }else if(state == BattleState.PlayerMove){
            HandlePlayerMovementSelection();
        }
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
        yield return battleDialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name} has appeared!");

        //Defino quien ataca primero
        if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            StartCoroutine(battleDialogBox.SetDialog($"The enemy attack first"));
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
        battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
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
            var oldSelectedMovement = currentSelectedMovement;
            currentSelectedMovement = (currentSelectedMovement + 2) % 4;

            if(currentSelectedMovement >= playerUnit.Pokemon.Moves.Count){
                currentSelectedMovement = oldSelectedMovement;
            }

            battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);

        }else if(Input.GetAxisRaw("Horizontal") != 0){
            timeSinceLastClick = 0;

            var oldSelectedMovement = currentSelectedMovement;
            if(currentSelectedMovement <= 1){
                currentSelectedMovement = (currentSelectedMovement + 1) % 2;
            }else{
                currentSelectedMovement = (currentSelectedMovement + 1) % 2 + 2;
            }

            if(currentSelectedMovement >= playerUnit.Pokemon.Moves.Count){
                currentSelectedMovement = oldSelectedMovement;
            }

            battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
        } 

        if(Input.GetAxisRaw("Submit") != 0){

            timeSinceLastClick = 0;

            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            StartCoroutine(PerformPlayerMovement());
        }

    }

    IEnumerator PerformPlayerMovement(){
        Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
        yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}!");

        var oldHPVal = enemyUnit.Pokemon.HP;

        //Arranco la anim de ataque y espero 1segundo para restarle la vida al pokemon
        playerUnit.AttackAnimationBattle();
        yield return new WaitForSeconds(1.0f);

        enemyUnit.ReceiveAttackAnimation();
        bool pokemonFainted = enemyUnit.Pokemon.ReceiveDamage(playerUnit.Pokemon, move); 
        enemyHUD.UpdatePokemonData(oldHPVal);

        if(pokemonFainted){
            yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} fainted!");
            enemyUnit.DieAnimationBattle();
        }else{
            StartCoroutine(EnemyAction());
        }
    }

    IEnumerator EnemyAction(){
        state = BattleState.EnemyMove;
        
        Move move = enemyUnit.Pokemon.RandomMove();
        yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}!");

        var oldHPVal = playerUnit.Pokemon.HP;

        enemyUnit.AttackAnimationBattle();
        yield return new WaitForSeconds(1.0f);

        playerUnit.ReceiveAttackAnimation();
        bool pokemonFainted = playerUnit.Pokemon.ReceiveDamage(enemyUnit.Pokemon, move);
        playerHUD.UpdatePokemonData(oldHPVal);

        if(pokemonFainted){
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} fainted!");
            playerUnit.DieAnimationBattle();
        }else{
            PlayerAction();
        }
    }
}
