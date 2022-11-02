using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    //Getter
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
}
