using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text pokemonName, pokemonLevel, pokemonHealth;
    [SerializeField] private HealthBar healthBar;

    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonName.text = pokemon.Base.Name;
        //Esto se llama string literals
        pokemonLevel.text = $"Lvl: {pokemon.Level}";
        healthBar.setHP(pokemon.HP/pokemon.MaxHP);
        pokemonHealth.text = $"{pokemon.HP}/{pokemon.MaxHP}";
    }
}
