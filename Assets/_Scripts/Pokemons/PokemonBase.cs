using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/New Pokemon")]

//No es un comportamiento por lo tanto no tiene ni start ni update ni nada
public class PokemonBase : ScriptableObject
{
    //Properties
    [SerializeField] private int _id;
    [SerializeField] private string name;

    //Esto es un getter de manera reducida, la palabra Name con Mayus es el sinonimo de GetName();
    public string Name => name;

    //Le agrego un textarea para que en el editor se vea mas grande el recuadro
    [TextArea][SerializeField] private string description;

    public string Description => description;

    [SerializeField] private Sprite frontSprite, backSprite;

    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;

    [SerializeField] private PokemonType type1, type2;
    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;

    //Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;

    public int MaxHp => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;

    [SerializeField] private List<LearnableMove> learnableMoves;

    public List<LearnableMove> LearnableMoves => learnableMoves;

}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Fight,
    Ice,
    Poison,
    Ground,
    Fly,
    Psychic,
    Rock,
    Bug,
    Ghost,
    Dragon,
    Dark,
    Fairy,
    Steel,
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase move;
    [SerializeField] private int level;

    public MoveBase Move => move;
    public int Level => level;
}
