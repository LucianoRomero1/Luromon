using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Linq;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    ForgetMovement,
    FinishBattle,
    LoseTurn
}

public enum BattleType{
    WildPokemon,
    Trainer,
    Leader
}

public class BattleManager : MonoBehaviour
{
    
    [SerializeField] private BattleUnit playerUnit;

    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox battleDialogBox;

    [SerializeField] private PartyHUD partyHUD;

    [SerializeField] private BattleState state;
    [SerializeField] private MoveSelectionUI moveSelectionUI;

    [SerializeField] private GameObject pokeball;
    [SerializeField] AudioClip attackClip, damageClip, levelUpClip, pokeballClip, pkmEscapedPokeballClip, endBattleClip, pokemonFaintedClip;

    private BattleType battleType;

    private int currentSelectedAction;

    private int currentSelectedMovement;

    private int currentSelectedPokemon;

    private int escapeAttempts;

    private MoveBase moveToLearn;

    private float timeSinceLastClick;

    private float timeBetweenClicks = 0.3f;

    public event Action<bool> OnBattleFinish;

    private PokemonParty playerParty;

    private Pokemon wildPokemon;

    //No puede tener Update sino va a estar corriendo en simultáneo con el playeController 
    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        //Verifico con una variable de tiempo para que no me saltee tan rapido de opciones
        //Hago esto para que no se me superpongan escrituras apretando enter, solo aparece otra si NO está escribiendo
        if (timeSinceLastClick < timeBetweenClicks || battleDialogBox.IsWriting)
        {
            return;
        }

        if (state == BattleState.ActionSelection){
            HandlePlayerSelection();
        }else if(state == BattleState.MovementSelection){
            HandlePlayerMovementSelection();
        }else if(state == BattleState.PartySelectScreen){
            HandlePlayerPartySelection();
        }else if(state == BattleState.LoseTurn){
            StartCoroutine(PerformEnemyMovement());
        }else if(state == BattleState.ForgetMovement){
            moveSelectionUI.HandleForgetMoveSelection((moveIndex) => {
                if(moveIndex < 0){
                    timeSinceLastClick = 0;
                    return;
                }

                StartCoroutine(ForgetOldMove(moveIndex));
            });
        }
    }

    IEnumerator ForgetOldMove(int moveIndex){
        moveSelectionUI.gameObject.SetActive(false);
        if(moveIndex == PokemonBase.NUMBER_OF_LEARNABLE_MOVES){
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} has not learned {moveToLearn.Name}!");
        }else{
            var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.Name} and he learned {moveToLearn.Name}!");
            playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = BattleState.FinishBattle;
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
        //TODO Implementar inventario
        battleDialogBox.ToggleActions(false);
        StartCoroutine(ThrowPokeball());
    }

    private void HandlePlayerSelection()
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
                StartCoroutine(TryToEscapeFromBattle());
            }
        }
        
    }

    private void HandlePlayerMovementSelection(){

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
            currentSelectedMovement = (currentSelectedMovement + 1) % 2 + 2 * (Mathf.FloorToInt(currentSelectedMovement / 2));

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

    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        battleType = BattleType.WildPokemon;
        escapeAttempts = 0;

        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        StartCoroutine(SetupBattle());
    }

    public void HandleStartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, bool isLeader){
        battleType = (isLeader ? BattleType.Leader : BattleType.Trainer);
        //TODO: Batalla contra NPC
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        playerUnit.SetupPokemon(playerParty.GetFirstHealthyPokemon());

        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);

        enemyUnit.SetupPokemon(wildPokemon);

        partyHUD.InitPartyHUD();

        //No arranco otra corrutina sino que espero que se ejecute esa corrutina para hacer otra cosa
        yield return battleDialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name} has appeared!");

        //Defino quien ataca primero
        yield return ChooseFirstTurn(true);
    }

    private IEnumerator ChooseFirstTurn(bool showFirstMsg = false){
        if(enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed){
            battleDialogBox.ToggleDialogText(true);
            battleDialogBox.ToggleActions(false);
            battleDialogBox.ToggleMovements(false);
            if(showFirstMsg){
                yield return battleDialogBox.SetDialog($"The enemy attack first");
            }
            yield return PerformEnemyMovement();
        }else{
            PlayerActionSelection();
        }
    }

    private void BattleFinish(bool playerHasWon){
        SoundManager.SharedInstance.PlaySound(endBattleClip);
        state = BattleState.FinishBattle;
        playerParty.Pokemons.ForEach(pkmn => pkmn.OnBattleFinish());
        OnBattleFinish(playerHasWon);
    }

    IEnumerator ShowStatsMessages(Pokemon pokemon){
        while(pokemon.StatusChangeMessages.Count > 0){
            //Saco un msj de la cola
            var message = pokemon.StatusChangeMessages.Dequeue();
            yield return battleDialogBox.SetDialog(message);
        }
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

    private int TryToCatchPokemon(Pokemon pokemon){
        //Funcion con formula de captura de pokemon basada en la pag oficial

        float bonusPokeball = 1; 
        float bonusStat = 1;

        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * bonusPokeball * bonusStat / (3 * pokemon.MaxHP);

        if(a >= 255){
            //4 es porque es captura inmediata
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while(shakeCount < 4){
            if(Random.Range(0, 65535) >= b){
                break;
            }else{
                shakeCount++;
            }
        }

        return shakeCount;
    }

    IEnumerator PerformPlayerMovement(){

        state = BattleState.PerformMovement;

        Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
        if(move.Pp <= 0){
            PlayerMovementSelection();
            yield break;
        }

        yield return RunMovement(playerUnit, enemyUnit, move);

        if(state == BattleState.PerformMovement){

            StartCoroutine(PerformEnemyMovement());
        }

        
    }

    IEnumerator PerformEnemyMovement(){
        state = BattleState.PerformMovement;
        
        Move move = enemyUnit.Pokemon.RandomMove();

        yield return RunMovement(enemyUnit, playerUnit, move);

        if(state == BattleState.PerformMovement){
            PlayerActionSelection();
        }
    }

    IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move){
        move.Pp--;
        yield return battleDialogBox.SetDialog($"{attacker.Pokemon.Base.Name} used {move.Base.Name}!");
        
        yield return RunMoveAnims(attacker, attackClip);
        yield return RunMoveAnims(target, damageClip);

        if(move.Base.MoveType == MoveType.Stats){
            yield return RunMoveStats(attacker.Pokemon, target.Pokemon, move);
        }else{
            var oldHPVal = target.Pokemon.HP;
            var damageDescription = target.Pokemon.ReceiveDamage(attacker.Pokemon, move); 
            yield return target.Hud.UpdatePokemonData(oldHPVal);
            yield return ShowDamageDescription(damageDescription);  
        }

        if(target.Pokemon.HP <= 0){
            yield return HandlePokemonFainted(target);
        }
    }

    IEnumerator RunMoveStats(Pokemon attacker, Pokemon target, Move move){
        foreach (var effect in move.Base.Effects.Boostings)
            {
                if(effect.target == MoveTarget.Self){
                    attacker.ApplyBoost(effect);
                }else{
                    target.ApplyBoost(effect);
                }
            }

        yield return ShowStatsMessages(attacker);
        yield return ShowStatsMessages(target);
    }

    IEnumerator RunMoveAnims(BattleUnit actor, AudioClip sound){
        //Arranco la anim de ataque y espero 1segundo para restarle la vida al pokemon
        actor.AttackAnimationBattle();
        SoundManager.SharedInstance.PlaySound(sound);
        yield return new WaitForSeconds(1.0f);
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

        bool currentPokemonFainted = true;

        if(playerUnit.Pokemon.HP > 0){
            currentPokemonFainted = false;
            yield return battleDialogBox.SetDialog($"Go back {playerUnit.Pokemon.Base.Name}!");
            playerUnit.DieAnimationBattle();
            yield return new WaitForSeconds(1.5f);
        }
       
        playerUnit.SetupPokemon(newPokemon);
        battleDialogBox.SetPokemonMovements(newPokemon.Moves);

        yield return battleDialogBox.SetDialog($"Go ahead {newPokemon.Base.Name}!");
        if(currentPokemonFainted){
            yield return ChooseFirstTurn();
        }else{
            yield return PerformEnemyMovement();
        }
        
    }

    IEnumerator ThrowPokeball(){
        state = BattleState.Busy;

        if(battleType != BattleType.WildPokemon){
            yield return battleDialogBox.SetDialog($"You can't steal pokemon from other trainers!");
            state = BattleState.LoseTurn;
            yield break;
        }

        yield return battleDialogBox.SetDialog($"Luro used {pokeball.name}!");

        SoundManager.SharedInstance.PlaySound(pokeballClip);
        var pokeballInst = Instantiate(pokeball, playerUnit.transform.position + new Vector3(-1, -1), Quaternion.identity);

        var pokeballSpt = pokeballInst.GetComponent<SpriteRenderer>();
                                                                                                    //fuerza, salto, duration 
        yield return pokeballSpt.transform.DOLocalJump(enemyUnit.transform. position + new Vector3(0, 1.5f), 1.5f, 1, 1f).WaitForCompletion();

        yield return enemyUnit.PlayCapturedAnimation();
        yield return pokeballSpt.transform.DOLocalMoveY(enemyUnit.transform.position.y - 1.7f, 0.5f).WaitForCompletion();

        var numberOfShakes = TryToCatchPokemon(enemyUnit.Pokemon);
        //Mathf.min determina que cualquier nro mas alto que 3, siga en 3
        for (int i = 0; i < Mathf.Min(numberOfShakes, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeballSpt.transform.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.6f).WaitForCompletion();
        }

        if(numberOfShakes == 4){
            yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} Captured!");
            yield return pokeballSpt.DOFade(0, 1.5f).WaitForCompletion();

            if(playerParty.AddPokemonToParty(enemyUnit.Pokemon)){
                yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your team!");
            }else{
                //No está hecho lo de PC de Bill
                yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} has been sent to Bill's PC");
            }

            Destroy(pokeballInst);
            BattleFinish(true);
        }else{
            yield return new WaitForSeconds(0.5f);
            pokeballSpt.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if(numberOfShakes < 2){
                SoundManager.SharedInstance.PlaySound(pkmEscapedPokeballClip);
                yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} has escaped!");
            }else{
                yield return battleDialogBox.SetDialog($"you almost caught it!");
            }

            Destroy(pokeballInst);
            state = BattleState.LoseTurn;
        }
    }

    IEnumerator TryToEscapeFromBattle(){
        state = BattleState.Busy;

        if(battleType != BattleType.WildPokemon){
            yield return battleDialogBox.SetDialog($"You can't run away from battles against pokemon trainers!");
            state = BattleState.LoseTurn;
            yield break;
        }

        escapeAttempts++;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if(playerSpeed >= enemySpeed){
            yield return battleDialogBox.SetDialog($"You have escaped successfully!");
            yield return new WaitForSeconds(1f);
            OnBattleFinish(true);
        }else{
            int oddsScape = (Mathf.FloorToInt(playerSpeed * 128 / enemySpeed) + 30 * escapeAttempts) % 256;
            if(Random.Range(0, 256) < oddsScape){
                yield return battleDialogBox.SetDialog($"You have escaped successfully!");
                yield return new WaitForSeconds(1f);
                OnBattleFinish(true);
            }else{
                yield return battleDialogBox.SetDialog($"You can't escape!");
                state = BattleState.LoseTurn;
            }
        }
    }    

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit){
        yield return battleDialogBox.SetDialog($"{faintedUnit.Pokemon.Base.Name} fainted!");
        SoundManager.SharedInstance.PlaySound(pokemonFaintedClip);
        faintedUnit.DieAnimationBattle();

        //Espero para que haga la animacion y el texto y dsp cierro la batalla
        yield return new WaitForSeconds(2f);

        if(!faintedUnit.IsPlayer){
            int expBase = faintedUnit.Pokemon.Base.ExpBase;
            int level = faintedUnit.Pokemon.Level;
            float multiplier = (battleType == BattleType.WildPokemon ? 1 : 1.5f);
            int wonExp = Mathf.FloorToInt((expBase * level * multiplier / 7));

            playerUnit.Pokemon.Experience += wonExp;
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} has gained {wonExp} experience points!");
            yield return playerUnit.Hud.SetExpSmooth();

            yield return new WaitForSeconds(0.5f);

            //Check lvl up
            while(playerUnit.Pokemon.NeedToLevelUp()){
                SoundManager.SharedInstance.PlaySound(levelUpClip);
                playerUnit.Hud.SetLevelText();
                yield return playerUnit.Hud.UpdatePokemonData(playerUnit.Pokemon.HP);
                yield return new WaitForSeconds(1f);
                yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} level up!");
                
                //Learn new move
                var newLearnableMove = playerUnit.Pokemon.GetLearnableMoveAtCurrentLevel();
                if(newLearnableMove != null){
                    if(playerUnit.Pokemon.Moves.Count < PokemonBase.NUMBER_OF_LEARNABLE_MOVES){
                        playerUnit.Pokemon.LearnMove(newLearnableMove);
                        yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} learned {newLearnableMove.Move.Name}!");
                        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
                    }else{
                        yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.name} tries to learn {newLearnableMove.Move.Name}!");
                        yield return battleDialogBox.SetDialog($"But can't learn more than {PokemonBase.NUMBER_OF_LEARNABLE_MOVES} moves!");
                        yield return ChooseMovementToForget(playerUnit.Pokemon, newLearnableMove.Move);

                        //Booleano predicado, espero hasta que se cumpla otra condicion
                        yield return new WaitUntil( () => state != BattleState.ForgetMovement);
                    }
                }
                yield return playerUnit.Hud.SetExpSmooth(true);
            }
        }   
        
        CheckForBattleFinish(faintedUnit); 
    }

    private IEnumerator ChooseMovementToForget(Pokemon learner, MoveBase newMove){
        state = BattleState.Busy;
        yield return battleDialogBox.SetDialog("Select the move you want to forget");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMovements(learner.Moves.Select(mv => mv.Base).ToList(), newMove);

        moveToLearn = newMove;
        state = BattleState.ForgetMovement;
    }
}
