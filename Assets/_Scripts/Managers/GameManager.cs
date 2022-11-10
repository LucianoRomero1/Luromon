using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
    Travel,
    Battle
}

[RequireComponent(typeof(ColorManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldMainCamera;
    [SerializeField] private AudioClip worldClip, battleClip;

    private GameState _gameState;


    private void Awake() {
        _gameState = GameState.Travel;
    }

    private void Start() {
        //con el += vinculo los eventos
        SoundManager.SharedInstance.PlayMusic(worldClip);
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
        SoundManager.SharedInstance.PlayMusic(battleClip);

        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleManager.HandleStartBattle(playerParty, wildPokemonCopy);
    }

    private void FinishPokemonBattle(bool playerWon){
        SoundManager.SharedInstance.PlayMusic(worldClip);

        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);

        if(!playerWon){
            //TODO: Diferenciar entre victoria y derrota
        }
    }
}
