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
        healthBar.setHP((float)_pokemon.HP/_pokemon.MaxHP); //1 para empezar con el 100% de la vida
        UpdatePokemonData(_pokemon.MaxHP);
    }

    public void UpdatePokemonData(int oldHPVal){
        StartCoroutine(healthBar.SetSmoothHP((float)_pokemon.HP/_pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
    }

    private IEnumerator DecreaseHealthPoints(int oldHPVal){

        while(oldHPVal > _pokemon.HP){
            oldHPVal--;
            pokemonHealth.text = $"{oldHPVal}/{_pokemon.MaxHP}";
            yield return new WaitForSeconds(0.1f);
        }

        pokemonHealth.text = $"{_pokemon.HP}/{_pokemon.MaxHP}";
        
    }

    
}
