using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    Travel,
    Battle
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;

    [SerializeField] private Camera worldMainCamera;

    private GameState _gameState;


    private void Awake() {
        _gameState = GameState.Travel;
    }

    private void Start() {
        //con el += vinculo los eventos
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += FinishPokemonBattle;
    }

    private void Update() {
        if(_gameState == GameState.Travel){

            playerController.HandleUpdate();

        }else if(_gameState == GameState.Battle){

            battleManager.HandleUpdate();

        }
    }

    private void StartPokemonBattle(){
        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();

        battleManager.HandleStartBattle(playerParty, wildPokemon);
    }

    private void FinishPokemonBattle(bool playerWon){
        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);

        if(!playerWon){
            //TODO: Diferenciar entre victoria y derrota
        }
    }
}
