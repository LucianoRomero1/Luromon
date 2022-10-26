using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text pokemonName, pokemonLevel, pokemonHealth;
    [SerializeField] private HealthBar healthBar;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        pokemonName.text = pokemon.Base.Name;
        //Esto se llama string literals
        pokemonLevel.text = $"Lvl: {pokemon.Level}";
        UpdatePokemonData();
    }

    public void UpdatePokemonData(){
        StartCoroutine(healthBar.SetSmoothHP((float)_pokemon.HP/_pokemon.MaxHP));
        pokemonHealth.text = $"{_pokemon.HP}/{_pokemon.MaxHP}";
    }

    
}
