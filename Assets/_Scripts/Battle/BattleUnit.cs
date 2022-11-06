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

    [SerializeField] private BattleHUD hud;

    public BattleHUD Hud => hud;

    public bool IsPlayer => isPlayer;

    private Pokemon pokemon;

    public Pokemon Pokemon
    {
        get => pokemon;
        set => pokemon = value;
    }
    

    private Image pokemonImage;

    private Vector3 initialPosition;

    private Color initialColor;

    [SerializeField] private float startTimeAnimation = 1.0f, attackTimeAnimation = 0.3f, hitTimeAnimation = 0.15f, dieTimeAnimation = 1.0f, capturedTimeAnim = 0.6f;

    private void Awake() {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition; //Local position es las coordenadas locales en base al padre
        initialColor = pokemonImage.color;
       
    }

    public void SetupPokemon(Pokemon pokemon)
    {
        Pokemon = pokemon;

        pokemonImage.sprite = (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
        pokemonImage.color  = initialColor; 

        hud.SetPokemonData(pokemon);
        transform.localScale = new Vector3(1f, 1f, 1f);

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

    public IEnumerator PlayCapturedAnimation(){
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOFade(0, capturedTimeAnim));
        seq.Join(transform.DOScale(new Vector3(0.25f, 0.25f, 1f), capturedTimeAnim));
        seq.Join(transform.DOLocalMoveY(initialPosition.y + 50f, capturedTimeAnim));
        yield return seq.WaitForCompletion();
    }

    public IEnumerator PlayBreakOutAnimation(){
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOFade(1, capturedTimeAnim));
        seq.Join(transform.DOScale(new Vector3(1f, 1f, 1f), capturedTimeAnim));
        seq.Join(transform.DOLocalMoveY(initialPosition.y, capturedTimeAnim));
        yield return seq.WaitForCompletion();
    }
}
