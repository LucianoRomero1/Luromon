using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    private const int NUM_MAX_POKEMON_IN_PARTY = 6;

    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemons => pokemons;

    private void Start() {
        foreach(var pokemon in pokemons){
            pokemon.InitPokemon();
        }
    }

    public Pokemon GetFirstHealthyPokemon(){
        //Devolveme de todos los pokemones el primero con HP mayor a cero, es un filter pero de c#
        return pokemons.Where(p => p.HP > 0).FirstOrDefault();
    }

    public int GetPositionFromPokemon(Pokemon pokemon){
        for (int i = 0; i < Pokemons.Count; i++)
        {
            if(pokemon == Pokemons[i]){
                return i;
            }
        }

        //Como a esto NUNCA va a entrar, es un valor típico a devolver cuando sabes que NO va a funcionar
        return -1;
    }

    public bool AddPokemonToParty(Pokemon pokemon){
        if(pokemons.Count < NUM_MAX_POKEMON_IN_PARTY){
            pokemons.Add(pokemon);
            return true;
        }else{
            //TODO: Enviar al PC de Bill
            return false;
        }
    }
}
