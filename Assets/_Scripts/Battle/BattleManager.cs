using System;
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

    public event Action<bool> OnBattleFinish;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;
    
    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {   
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        StartCoroutine(SetupBattle());
    }

    //No puede tener Update sino va a estar corriendo en simultáneo con el playeController 
    public void HandleUpdate()
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

        playerUnit.SetupPokemon(playerParty.GetFirstHealthyPokemon());
        playerHUD.SetPokemonData(playerUnit.Pokemon);

        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);

        enemyUnit.SetupPokemon(wildPokemon);
        enemyHUD.SetPokemonData(enemyUnit.Pokemon);

        //No arranco otra corrutina sino que espero que se ejecute esa corrutina para hacer otra cosa
        yield return battleDialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name} has appeared!");

        //Defino quien ataca primero
        if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            StartCoroutine(battleDialogBox.SetDialog($"The enemy attack first"));

            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyAction());
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
                currentSelectedAction = (currentSelectedAction + 2) % 4;

                battleDialogBox.SelectAction(currentSelectedAction);
            }else if(Input.GetAxisRaw("Horizontal") != 0){
                timeSinceLastClick = 0;
                currentSelectedAction = (currentSelectedAction + 1) % 2 + 2 * (Mathf.FloorToInt(currentSelectedAction / 2));

                battleDialogBox.SelectAction(currentSelectedAction);
            }

            if(Input.GetAxisRaw("Submit") != 0){
                timeSinceLastClick = 0;

                if(currentSelectedAction == 0){
                    //Fight
                    PlayerMovement();
                }else if(currentSelectedAction == 1){
                    //Bag
                    OpenInventoryScreen();
                }
                else if(currentSelectedAction == 2){
                    //Pokemon
                    OpenPartySelectionScreen();
                }
                else if(currentSelectedAction == 3){
                    //Run
                    OnBattleFinish(false);
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

            var oldSelectedMovement = (currentSelectedMovement + 1) % 2 + 2 * (Mathf.FloorToInt(currentSelectedMovement / 2));

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

        if(Input.GetAxis("Cancel") != 0){
            PlayerAction();
        }

    }

    IEnumerator PerformPlayerMovement(){
        Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
        move.Pp--;
        yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}!");

        var oldHPVal = enemyUnit.Pokemon.HP;

        //Arranco la anim de ataque y espero 1segundo para restarle la vida al pokemon
        playerUnit.AttackAnimationBattle();
        yield return new WaitForSeconds(1.0f);
        enemyUnit.ReceiveAttackAnimation();

        var damageDescription = enemyUnit.Pokemon.ReceiveDamage(playerUnit.Pokemon, move); 
        enemyHUD.UpdatePokemonData(oldHPVal);
        yield return ShowDamageDescription(damageDescription);

        if(damageDescription.Fainted){
            yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} fainted!");
            enemyUnit.DieAnimationBattle();

            //Espero para que haga la animacion y el texto y dsp cierro la batalla
            yield return new WaitForSeconds(2f);
            OnBattleFinish(true);
        }else{
            StartCoroutine(EnemyAction());
        }
    }

    IEnumerator EnemyAction(){
        state = BattleState.EnemyMove;
        
        Move move = enemyUnit.Pokemon.RandomMove();
        move.Pp--;
        yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}!");

        var oldHPVal = playerUnit.Pokemon.HP;

        enemyUnit.AttackAnimationBattle();
        yield return new WaitForSeconds(1.0f);
        playerUnit.ReceiveAttackAnimation();

        var damageDescription = playerUnit.Pokemon.ReceiveDamage(enemyUnit.Pokemon, move);
        playerHUD.UpdatePokemonData(oldHPVal);
        yield return ShowDamageDescription(damageDescription);

        if(damageDescription.Fainted){
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} fainted!");
            playerUnit.DieAnimationBattle();

            yield return new WaitForSeconds(1.5f);

            var nextPokemon =  playerParty.GetFirstHealthyPokemon();
            if(nextPokemon == null){
                OnBattleFinish(false);
            }else{
                playerUnit.SetupPokemon(nextPokemon);
                playerHUD.SetPokemonData(nextPokemon);

                battleDialogBox.SetPokemonMovements(nextPokemon.Moves);

                yield return battleDialogBox.SetDialog($"Go ahead {nextPokemon.Base.Name}!");

                PlayerAction();
            }

        }else{
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDescription(DamageDescription description){
        if(description.Critical > 1f){
            yield return battleDialogBox.SetDialog($"A critical hit!");
        }
        if(description.Type > 1f){
            yield return battleDialogBox.SetDialog($"It's very effective!");
        }else if(description.Type < 1f){
            yield return battleDialogBox.SetDialog($"Isn't very effective...");
        }
    }

    private void OpenPartySelectionScreen(){
        print("Open windows with all pokemon");
        if(Input.GetAxisRaw("Cancel") != 0){
            PlayerAction();
        }
    }

    private void OpenInventoryScreen(){
        print("Open windows with all inventory");
        if(Input.GetAxisRaw("Cancel") != 0){
            PlayerAction();
        }
    }
}
