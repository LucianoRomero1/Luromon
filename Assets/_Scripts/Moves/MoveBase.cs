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
    [SerializeField] private MoveType moveType;
    [SerializeField] private MoveStatEffect effects;
    [SerializeField] private MoveTarget target;

    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;
    public MoveType MoveType => moveType;
    public MoveStatEffect Effects => effects;
    public MoveTarget Target => target;

    public bool isSpecialMove => moveType == MoveType.Special;
    //{
        //get{
            // if(type == PokemonType.Fire     || type == PokemonType.Water    || type == PokemonType.Grass   || 
            //    type == PokemonType.Ice      || type == PokemonType.Electric || type == PokemonType.Psychic || 
            //    type == PokemonType.Dragon   || type == PokemonType.Dark){
            //     //Si es de alguno de estos tipos es ataque especial
            //     return true;
            // }else{
            //     return false;
            // }
            
        //}
    //}
}

public enum MoveType{
    Physical,
    Special,
    Stats
}

[System.Serializable]
public class MoveStatEffect{
    [SerializeField] private List<StatBoosting> boostings;
    [SerializeField] private StatusConditionID status;
    public List<StatBoosting> Boostings => boostings;
    public StatusConditionID Status => status;
}

[System.Serializable]
public class StatBoosting{
    public Stat stat;
    public int boost;
    public MoveTarget target;
}

public enum MoveTarget{
    Self,
    Other
}
