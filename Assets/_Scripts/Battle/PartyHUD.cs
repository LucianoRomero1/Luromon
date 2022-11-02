using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyHUD : MonoBehaviour
{
    [SerializeField] private Text messageText;
    private PartyMemberHUD[] memberHuds;
    private List<Pokemon> pokemons;

    public void InitPartyHUD(){
        //Con esto se rellenan automaticamente dentro del array porque la party es hija del HUD padre
        memberHuds = GetComponentsInChildren<PartyMemberHUD>();
    }

    public void SetPartyData(List<Pokemon> pokemons){

        this.pokemons = pokemons;

        messageText.text = "Choose a Pokemon";
        
        for (int i = 0; i < memberHuds.Length; i++){
            if(i < pokemons.Count){
                memberHuds[i].SetPokemonData(pokemons[i]);
                memberHuds[i].gameObject.SetActive(true);
            }else{
                memberHuds[i].gameObject.SetActive(false);
            }
        }   
    }

    public void UpdateSelectedPokemon(int selectedPokemon){
        for (int i = 0; i < pokemons.Count; i++)
        {
            memberHuds[i].SetSelectedPokemon(i == selectedPokemon);
        }
    }

    public void SetMessage(string message){
        messageText.text = message;
    }
}
