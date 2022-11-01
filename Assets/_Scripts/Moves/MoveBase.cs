using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;
    public string Name => name;

    [TextArea] [SerializeField] private string description;
    public string Description => description;

    //Stats
    [SerializeField] private PokemonType type; //Tipo de ataque del pokemon
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;

    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;

    public bool isSpecialMove{
        get{
            if(type == PokemonType.Fire     || type == PokemonType.Water    || type == PokemonType.Grass   || 
               type == PokemonType.Ice      || type == PokemonType.Electric || type == PokemonType.Psychic || 
               type == PokemonType.Dragon   || type == PokemonType.Dark){
                //Si es de alguno de estos tipos es ataque especial
                return true;
            }else{
                return false;
            }
        }
    }
}
