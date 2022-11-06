using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    [SerializeField] private Text nameText, levelText, typeText, hpText;
    [SerializeField] private HealthBar healtBar;
    [SerializeField] private Image pokemonImage;
    [SerializeField] Color selectedColor = Color.blue;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon){
        _pokemon = pokemon;

        nameText.text   = pokemon.Base.Name;
        levelText.text  = $"Lvl {pokemon.Level}";
        
        if(pokemon.Base.Type2 == PokemonType.None){
            typeText.text = pokemon.Base.Type1.ToString().ToUpper();
        }else{
            typeText.text   = $"{pokemon.Base.Type1.ToString().ToUpper()} - {pokemon.Base.Type2.ToString().ToUpper()}";
        }

        hpText.text     = $"{pokemon.HP} / {pokemon.MaxHP}";
        healtBar.setHP((float) pokemon.HP/pokemon.MaxHP);
        pokemonImage.sprite = pokemon.Base.FrontSprite;

        GetComponent<Image>().color = TypeColor.GetColorFromType(pokemon.Base.Type1);
    }

    public void SetSelectedPokemon(bool selected){
        nameText.color = (selected ? selectedColor : Color.black);
    }
}
