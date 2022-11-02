using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    FinishBattle
}

public class BattleManager : MonoBehaviour
{
    
    [SerializeField] private BattleUnit playerUnit;

    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox battleDialogBox;

    [SerializeField] private PartyHUD partyHUD;

    [SerializeField] private BattleState state;

    private int currentSelectedAction;
    private int currentSelectedMovement;

    private int currentSelectedPokemon;

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

        if (state == BattleState.ActionSelection){
            HandlePlayerSelection();
        }else if(state == BattleState.MovementSelection){
            HandlePlayerMovementSelection();
        }else if(state == BattleState.PartySelectScreen){
            HandlePlayerPartySelection();
        }
    }

    private void PlayerActionSelection()
    {
        state = BattleState.ActionSelection;

        StartCoroutine(battleDialogBox.SetDialog($"Select an action..."));
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleMovements(false);

        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    private void PlayerMovementSelection(){
        state = BattleState.MovementSelection;

        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);

        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
    }
    
    private void OpenPartySelectionScreen(){
        state = BattleState.PartySelectScreen;
    
        partyHUD.SetPartyData(playerParty.Pokemons);
        partyHUD.gameObject.SetActive(true);

        currentSelectedPokemon = playerParty.GetPositionFromPokemon(playerUnit.Pokemon);
        partyHUD.UpdateSelectedPokemon(currentSelectedAction);
    }

    private void OpenInventoryScreen(){
        print("Open bag");
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
                    PlayerMovementSelection();
                }else if(currentSelectedAction == 1){
                    OpenInventoryScreen();
                }
                else if(currentSelectedAction == 2){
                    OpenPartySelectionScreen();
                }
                else if(currentSelectedAction == 3){
                    BattleFinish(false);
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
            PlayerActionSelection();
        }

    }

    private void HandlePlayerPartySelection(){
         if(timeSinceLastClick < timeBetweenClicks){
            return;
        }

        if(Input.GetAxisRaw("Vertical") != 0){
            timeSinceLastClick = 0;
            currentSelectedPokemon -= (int)Input.GetAxisRaw("Vertical") * 2;      
        }else if(Input.GetAxisRaw("Horizontal") != 0){
            timeSinceLastClick = 0;
            currentSelectedPokemon += (int)Input.GetAxisRaw("Horizontal") ; 
        } 

        currentSelectedPokemon = Mathf.Clamp(currentSelectedPokemon, 0 , playerParty.Pokemons.Count - 1);
        partyHUD.UpdateSelectedPokemon(currentSelectedPokemon);

        if(Input.GetAxisRaw("Submit") != 0){
            timeSinceLastClick = 0;
            var selectedPokemon = playerParty.Pokemons[currentSelectedPokemon];
            if(selectedPokemon.HP <= 0){
                partyHUD.SetMessage("You can't send a weakened pokemon!");
                return;
            }else if(selectedPokemon == playerUnit.Pokemon){
                partyHUD.SetMessage("This pokemon is already in battle!");
                return;
            }

            partyHUD.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedPokemon));
        }

        if(Input.GetAxis("Cancel") != 0){
            partyHUD.gameObject.SetActive(false);
            PlayerActionSelection();
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

        partyHUD.InitPartyHUD();

        //No arranco otra corrutina sino que espero que se ejecute esa corrutina para hacer otra cosa
        yield return battleDialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name} has appeared!");

        //Defino quien ataca primero
        if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            StartCoroutine(battleDialogBox.SetDialog($"The enemy attack first"));

            yield return new WaitForSeconds(1.5f);
            StartCoroutine(PerformEnemyMovement());
        }
        else
        {
            PlayerActionSelection();
        }
    }

    private void BattleFinish(bool playerHasWon){
        state = BattleState.FinishBattle;
        OnBattleFinish(playerHasWon);
    }

    IEnumerator PerformPlayerMovement(){

        state = BattleState.PerformMovement;

        Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];

        yield return StartCoroutine(RunMovement(playerUnit, enemyUnit, move));

        if(state == BattleState.PerformMovement){
            StartCoroutine(PerformEnemyMovement());
        }

        
    }

    IEnumerator PerformEnemyMovement(){
        state = BattleState.PerformMovement;
        
        Move move = enemyUnit.Pokemon.RandomMove();

        yield return StartCoroutine(RunMovement(enemyUnit, playerUnit, move));

        if(state == BattleState.PerformMovement){
            PlayerActionSelection();
        }
    }

    IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move){

        move.Pp--;
        yield return battleDialogBox.SetDialog($"{attacker.Pokemon.Base.Name} used {move.Base.Name}!");

        var oldHPVal = target.Pokemon.HP;

        //Arranco la anim de ataque y espero 1segundo para restarle la vida al pokemon
        attacker.AttackAnimationBattle();
        yield return new WaitForSeconds(1.0f);
        target.ReceiveAttackAnimation();

        var damageDescription = target.Pokemon.ReceiveDamage(attacker.Pokemon, move); 
        //enemyHUD.UpdatePokemonData(oldHPVal);
        yield return ShowDamageDescription(damageDescription);

        if(damageDescription.Fainted){
            yield return battleDialogBox.SetDialog($"{target.Pokemon.Base.Name} fainted!");
            target.DieAnimationBattle();

            //Espero para que haga la animacion y el texto y dsp cierro la batalla
            yield return new WaitForSeconds(2f);
            CheckForBattleFinish(target); 
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

    IEnumerator SwitchPokemon(Pokemon newPokemon){

        if(playerUnit.Pokemon.HP > 0){
            yield return battleDialogBox.SetDialog($"Go back {playerUnit.Pokemon.Base.Name}!");
            playerUnit.DieAnimationBattle();
            yield return new WaitForSeconds(1.5f);
        }
       
        playerUnit.SetupPokemon(newPokemon);
        playerHUD.SetPokemonData(newPokemon);
        battleDialogBox.SetPokemonMovements(newPokemon.Moves);

        yield return battleDialogBox.SetDialog($"Go ahead {newPokemon.Base.Name}!");
        StartCoroutine(PerformEnemyMovement());
    }


    private void CheckForBattleFinish(BattleUnit faintedUnit){
        if(faintedUnit.IsPlayer){
            var nextPokemon = playerParty.GetFirstHealthyPokemon();
            if(nextPokemon != null){
                OpenPartySelectionScreen();
            }else{
                BattleFinish(false);
            }
        }else{
            BattleFinish(true);
        }
    }
    
}
