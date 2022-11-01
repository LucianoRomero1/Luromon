using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    [SerializeField] private Text nameText, levelText, typeText, hpText;
    [SerializeField] private HealthBar healtBar;
    [SerializeField] private Image pokemonImage;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon){
        _pokemon = pokemon;

        nameText.text   = pokemon.Base.Name;
        typeText.text   = pokemon.Base.Type1.ToString();
        levelText.text  = $"Lvl {pokemon.Level}";
        hpText.text     = $"{pokemon.HP} / {pokemon.MaxHP}";

        healtBar.setHP((float) pokemon.HP/pokemon.MaxHP);
        pokemonImage.sprite = pokemon.Base.FrontSprite;
    }
}
