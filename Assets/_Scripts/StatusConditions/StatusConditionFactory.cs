using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusConditionFactory
{
    public static Dictionary<StatusConditionID, StatusCondition> StatusCondition { get; set; } =
        new Dictionary<StatusConditionID, StatusCondition>()
        {
            {
                StatusConditionID.PSN,
                new StatusCondition(){
                    Name = "Poison",
                    Description = "Causes the pokemon to take damage each turn",
                    StartMessage = "has been poisoned",
                    OnFinishTurn = PoisonEffect
                }
            },
            {
                StatusConditionID.BRN,
                new StatusCondition(){
                    Name = "Burned",
                    Description = "Causes the pokemon to take damage each turn",
                    StartMessage = "has been burned",
                    OnFinishTurn = BurnEffect
                }
            }
        };

    static void PoisonEffect(Pokemon pokemon){
        pokemon.UpdateHP(Mathf.CeilToInt((float) pokemon.MaxHP / 8.0f));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} suffers from the effects of the poison!");
    }

    static void BurnEffect(Pokemon pokemon){
        pokemon.UpdateHP(Mathf.CeilToInt((float)pokemon.MaxHP / 15.0f));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} suffers from the effects of the burn!");
    }
}

public enum StatusConditionID { 
    NONE,
    BRN, 
    FRZ, 
    PAR, 
    PSN, 
    SLP 
}
