using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

[Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    public PokemonBase Base => _base;

    [SerializeField] private int _level;

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    private List<Move> _moves;

    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }

    //Lo primero es la clave por el cual encuentro dentro el diccionario y lo segundo es lo que obtengo
    //cualquiera puede acceder a traves de get, pero solo esta clase puede setear
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatsBoosted { get; private set; }
    public StatusCondition StatusCondition { get; set; }
    public Queue<string> StatusChangeMessages { get; private set; } = new Queue<string>();
    public bool HasHpChanged { get; set; } = false;
    public int previousHPValue;

    //Vida actual del pokemon
    private int _hp;

    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            _hp = Mathf.FloorToInt(Mathf.Clamp(_hp, 0, MaxHP));
        }
    }

    private int _experience;

    public int Experience
    {
        get => _experience;
        set => _experience = value;
    }

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        _level = pLevel;
        InitPokemon();
    }

    public void InitPokemon()
    {
        _experience = Base.GetNecessaryExpForLevel(_level);
        _moves = new List<Move>();

        foreach (var lMove in _base.LearnableMoves)
        {
            if (lMove.Level <= _level)
            {
                _moves.Add(new Move(lMove.Move));
            }

            if (_moves.Count >= PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
            {
                break;
            }
        }

        CalculateStats();
        _hp = MaxHP;
        previousHPValue = MaxHP;
        HasHpChanged = true;

        ResetBoostings();
    }

    private void ResetBoostings(){

        StatusChangeMessages = new Queue<string>();

        StatsBoosted = new Dictionary<Stat, int>(){
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
        };
    }

    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((_base.Attack * _level) / 100.0f) + 2);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((_base.Defense * _level) / 100.0f) + 2);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((_base.SpAttack * _level) / 100.0f) + 2);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((_base.SpDefense * _level) / 100.0f) + 2);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((_base.Speed * _level) / 100.0f) + 2);

        MaxHP = Mathf.FloorToInt((_base.MaxHp * _level) / 20.0f) + 10;
    }

    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        int boost = StatsBoosted[stat];
        float multiplier = 1.0f + Mathf.Abs(boost) / 2.0f;

        if(boost >= 0){
            statValue = Mathf.FloorToInt(statValue * multiplier);
        }else{
            statValue = Mathf.FloorToInt(statValue / multiplier);
        }

        return statValue;
    }

    public void ApplyBoost(StatBoosting boost){

        var stat = boost.stat;
        var value = boost.boost;

        StatsBoosted[stat] = Mathf.Clamp(StatsBoosted[stat] + value, -6, 6);
        if(value > 0){
            StatusChangeMessages.Enqueue($"{Base.Name} has increased his {stat}");
        }else if(value < 0){
            StatusChangeMessages.Enqueue($"{Base.Name} has reduced his {stat}");
        }else{
            StatusChangeMessages.Enqueue($"{Base.Name} has not taken effect");
        }
    }

    //Se multiplica el ataque base por el nivel, se divide por 100 para que no sea un nro enorme. Y por si el nro da 0, se suma 1 (excepto la vida que es +10)
    public int MaxHP {get; private set ;}
    public int Attack => GetStat(Stat.Attack);
    public int Defense => GetStat(Stat.Defense);
    public int SpAttack => GetStat(Stat.SpAttack);
    public int SpDefense => GetStat(Stat.SpDefense);
    public int Speed => GetStat(Stat.Speed);

    
    public DamageDescription ReceiveDamage(Pokemon attacker, Move move){

        float critical = 1f;
        if(Random.Range(0f, 100f) < 8f){
            critical = 2f;
        }

        float type1 = TypeMatrix.GetMulteffectiveness(move.Base.Type, this.Base.Type1);
        float type2 = TypeMatrix.GetMulteffectiveness(move.Base.Type, this.Base.Type2);

        var damageDescription = new DamageDescription()
        {
            Critical = critical,
            Type     = type1 * type2,
            Fainted  = false
        };

        float attack  = (move.Base.isSpecialMove ? attacker.SpAttack : attacker.Attack);
        float defense = (move.Base.isSpecialMove ? this.SpDefense : this.Defense); 

        float modifiers = Random.Range(0.89f, 1.0f) * type1 * type2 * critical;
        float baseDamage = ((2 * attacker.Level / 5f + 2) * move.Base.Power * ((float) attack / defense )) / 50f + 2; 
        int totalDamage = Mathf.FloorToInt(baseDamage * modifiers);

        UpdateHP(totalDamage);
        if(HP <= 0){
            damageDescription.Fainted = true;
        }
        

        return damageDescription;
    }

    public void UpdateHP(int damage){
        HasHpChanged = true;
        previousHPValue = HP;
        HP -= damage;
        if(HP <= 0){
            HP = 0;
        }
    }

    public void SetConditionStatus(StatusConditionID id){
        StatusCondition = StatusConditionFactory.StatusCondition[id];
        StatusChangeMessages.Enqueue($"{Base.Name} {StatusCondition.StartMessage}");
    }

    public Move RandomMove(){
        var movesWithPP = Moves.Where(m => m.Pp > 0).ToList();
        if(movesWithPP.Count > 0){
            int randId = Random.Range(0, Moves.Count);
            return Moves[randId];
        }

        return null;
    }

    public bool NeedToLevelUp(){
        if(Experience > Base.GetNecessaryExpForLevel(_level + 1)){
            int currentMaxHP = MaxHP;
            _level++;
            HP += (MaxHP - currentMaxHP);
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel(){
        return Base.LearnableMoves.Where(lm => lm.Level == _level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove learnableMove){
        if(Moves.Count >= PokemonBase.NUMBER_OF_LEARNABLE_MOVES){
            return;
        }

        Moves.Add(new Move(learnableMove.Move));
    }

    public void OnFinishTurn(){
        StatusCondition?.OnFinishTurn?.Invoke(this);
    }

    public void OnBattleFinish(){
        ResetBoostings();
    }

}

public class DamageDescription{

    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Fainted { get; set; }
}
