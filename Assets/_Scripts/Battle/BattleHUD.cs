using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text pokemonName, pokemonLevel;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject expBar;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        pokemonName.text = pokemon.Base.Name;
        SetLevelText();
        healthBar.setHP(_pokemon);
        SetExp();
        StartCoroutine(UpdatePokemonData());
    }

    public void SetExp(){
        if(expBar == null){
            return;
        }

        expBar.transform.localScale = new Vector3(NormalizedExp(), 1f, 1f);
    }

    public IEnumerator UpdatePokemonData(){
        if(_pokemon.HasHpChanged){
            yield return healthBar.SetSmoothHP(_pokemon);
            _pokemon.HasHpChanged = false;
        }
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
