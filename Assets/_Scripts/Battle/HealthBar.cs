using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void setHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
        healthBar.GetComponent<Image>().color = ColorManager.SharedInstance.BarColor(normalizedValue);
    }

    public IEnumerator SetSmoothHP(float normalizedValue){
        //Esto es hacerlo a pata sin dotween
        // float currentScale = healthBar.transform.localScale.x;
        // float updateQuantity = currentScale - normalizedValue;

        // while(currentScale - normalizedValue > Mathf.Epsilon){
        //     currentScale -= updateQuantity * Time.deltaTime;
        //     healthBar.transform.localScale = new Vector3(currentScale, 1);
        //     healthBar.GetComponent<Image>().color = BarColor;
        //     yield return null;
        // }
        // healthBar.transform.localScale = new Vector3(normalizedValue, 1);

        var seq = DOTween.Sequence();
        seq.Append(healthBar.transform.DOScaleX(normalizedValue, 1f));
        seq.Join(healthBar.GetComponent<Image>().DOColor(ColorManager.SharedInstance.BarColor(normalizedValue), 1f));
        yield return seq.WaitForCompletion();
        
        //Por si el epsilon queda rondando en algun decimal en el ultimo frame
        
    }
}
