using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int _level;
    [SerializeField] private bool isPlayer;

    //Getter y Setter por defecto
    [SerializeField] private Pokemon pokemon;

    public Pokemon Pokemon
    {
        get => pokemon;
        set => pokemon = value;
    }

    public void SetupPokemon()
    {
        pokemon = new Pokemon(_base, _level);
        GetComponent<Image>().sprite = (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
    }
}
