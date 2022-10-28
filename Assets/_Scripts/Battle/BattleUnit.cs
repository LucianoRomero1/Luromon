using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    

    [SerializeField] private Image pokemonImage;

    private Vector3 initialPosition;

    private Color initialColor;

    [SerializeField] private float startTimeAnimation = 1.0f, attackTimeAnimation = 0.3f, hitTimeAnimation = 0.15f, dieTimeAnimation = 1.0f;

    private void Awake() {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition; //Local position es las coordenadas locales en base al padre
        initialColor = pokemonImage.color;
       
    }


    public void SetupPokemon()
    {
        pokemon = new Pokemon(_base, _level);
        pokemonImage.sprite = (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);

        StartAnimationBattle();
    }

    public void StartAnimationBattle(){
        pokemonImage.transform.localPosition = new Vector3(initialPosition.x + (isPlayer ? - 1 : 1) * 400, initialPosition.y);

        pokemonImage.transform.DOLocalMoveX(initialPosition.x, startTimeAnimation);
    }

    public void AttackAnimationBattle(){
        var seq = DOTween.Sequence(); //Estoy instanciando una secuencia
        //Es un array por lo tanto le agrego un move en X y el primer parámetro es la pos desde donde sale hasta donde va a ir
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x + (isPlayer ? 1 : -1) * 60, attackTimeAnimation));
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x, attackTimeAnimation));
    }

    public void ReceiveAttackAnimation(){
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOColor(Color.gray, hitTimeAnimation));
        seq.Append(pokemonImage.DOColor(initialColor, hitTimeAnimation));
    }

    public void DieAnimationBattle(){
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveY(initialPosition.y - 200, dieTimeAnimation));
        seq.Join(pokemonImage.DOFade(0f, dieTimeAnimation));
    }
}
