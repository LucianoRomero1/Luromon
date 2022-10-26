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
}
