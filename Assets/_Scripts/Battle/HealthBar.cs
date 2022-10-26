using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    //private void Start()
    //{
    //    healthBar.transform.localScale = new Vector3(0.5f, 1.0f);
    //}

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void setHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
    }
}
