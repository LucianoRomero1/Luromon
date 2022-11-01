using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemons{
        get => pokemons;
        set => pokemons = value;
    }

    private void Start() {
        foreach(var pokemon in pokemons){
            pokemon.InitPokemon();
        }
    }

    public Pokemon GetFirstHealthyPokemon(){
        //Devolveme de todos los pokemones el primero con HP mayor a cero, es un filter pero de c#
        return pokemons.Where(p => p.HP > 0).FirstOrDefault();
    }
}
