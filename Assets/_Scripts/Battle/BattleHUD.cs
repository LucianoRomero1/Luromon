using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text pokemonName, pokemonLevel, pokemonHealth;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject expBar;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        pokemonName.text = pokemon.Base.Name;
        SetLevelText();
        healthBar.setHP((float)_pokemon.HP/_pokemon.MaxHP);
        SetExp();
        StartCoroutine(UpdatePokemonData(_pokemon.MaxHP));
    }

    public void SetExp(){
        if(expBar == null){
            return;
        }

        expBar.transform.localScale = new Vector3(NormalizedExp(), 1f, 1f);
    }

    public IEnumerator UpdatePokemonData(int oldHPVal){
        StartCoroutine(healthBar.SetSmoothHP((float)_pokemon.HP/_pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
        
        yield return null;
    }

    private IEnumerator DecreaseHealthPoints(int oldHPVal){

        while(oldHPVal > _pokemon.HP){
            oldHPVal--;
            pokemonHealth.text = $"{oldHPVal}/{_pokemon.MaxHP}";
            yield return new WaitForSeconds(0.1f);
        }

        pokemonHealth.text = $"{_pokemon.HP}/{_pokemon.MaxHP}";   
    }

    public IEnumerator SetExpSmooth(bool needsToResetBar = false){
        if(expBar == null){
            yield break;
        }

        if(needsToResetBar){
            expBar.transform.localScale = new Vector3(0f, 1f, 1f);
        }


        yield return expBar.transform.DOScaleX(NormalizedExp(), 2f).WaitForCompletion();
    }

    private float NormalizedExp(){
        float currentLevelExp = _pokemon.Base.GetNecessaryExpForLevel(_pokemon.Level);
        float nextLevelExp = _pokemon.Base.GetNecessaryExpForLevel(_pokemon.Level + 1);
        float normalizedExp = (_pokemon.Experience - currentLevelExp) / (nextLevelExp - currentLevelExp);

        //Da un valor no superior a 1, si pasa del 1 queda en 1
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevelText(){
        pokemonLevel.text = $"Lvl: {_pokemon.Level}";
    }
    
}
