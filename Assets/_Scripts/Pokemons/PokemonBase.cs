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

//No va a contener todos los tipos ya que no se programó el juego completo, solo unos modelos
public class TypeMatrix{
    float[][] matrix = {
        //                     NOR  FIR   WAT   ELE  GRA  ICE  FIG  POI  GRO  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE   FAI
        /* NOR */ new float[] {1f,  1f  , 1f  , 1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,1f  ,0.5f,0f  ,1f  ,1f  ,0.5f, 1f},
        /* FIR */ new float[] {1f,  0.5f, 0.5f, 1f  ,2f  ,2f  ,1f  ,1f  ,1f  ,1f  ,1f  ,2f  ,0.5f,1f  ,0.5f,1f  ,1f  , 1f},
        /* WAT */ new float[] {1f,  2f  , 0.5f, 1f  ,0.5f,1f  ,1f  ,1f  ,2f  ,2f  ,1f  ,1f  ,2f  ,1f  ,0.5f,1f  ,1f  , 1f},
        /* ELE */ new float[] {1f,  1f  , 2f  , 0.5f,0.5f,1f  ,1f  ,1f  ,0f  ,0f  ,1f  ,1f  ,0f  ,1f  ,0.5f,1f  ,1f  , 1f},
        /* GRA */ new float[] {1f,  0.5f, 1f  , 1f  ,0.5f,1f  ,1f  ,0.5f,2f  ,0.5f,1f  ,0.5f,2f  ,1f  ,0.5f,1f  ,1f  , 1f},
    };
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase move;
    [SerializeField] private int level;

    public MoveBase Move => move;
    public int Level => level;
}
