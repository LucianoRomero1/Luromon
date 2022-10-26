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
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox battleDialogBox;

    [SerializeField] private BattleState state;

    private void Start()
    {
        StartCoroutine(SetupBattle());  
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        playerUnit.SetupPokemon();
        playerHUD.SetPokemonData(playerUnit.Pokemon);

        enemyUnit.SetupPokemon();
        enemyHUD.SetPokemonData(enemyUnit.Pokemon);

        //No arranco otra corrutina sino que espero que se ejecute esa corrutina para hacer otra cosa
        yield return battleDialogBox.SetDialog($"A wild {enemyUnit.Pokemon.Base.Name} has appeared!");
        yield return new WaitForSeconds(1.0f);

        //Defino quien ataca primero
        if(enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            StartCoroutine(battleDialogBox.SetDialog($"The enemy attack first"));
            EnemyAction();
        }
        else
        {
            PlayerAction();
        }

    }

    public void PlayerAction()
    {
        state = BattleState.PlayerSelectAction;
        StartCoroutine(battleDialogBox.SetDialog($"Select an action..."));
        battleDialogBox.ToggleActions(true);
    }

    public void EnemyAction()
    {

    }

    
}
