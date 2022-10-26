using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

//Requiere si o si el componente animator
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private bool isMoving;
    [SerializeField]
    private float speed;
    private Vector2 input;

    //Guion bajo para las propiedades que son del propio objeto
    private Animator _animator;

    public LayerMask solidObjectsLayer, pokemonLayer;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    //El update detecta el movimiento y arranca su corrutina
    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Evitar movimiento en diagonal
            if(input.x != 0)
            {
                //Cuando hay movimiento horizontal pongo en 0 la Y
                input.y = 0;
            }

            //Si la entrada de datos no es el vector 0,0 , hay movimiento
            if(input != Vector2.zero)
            {
                //Le damos al animator los valores de X e Y
                _animator.SetFloat("Move X", input.x);
                _animator.SetFloat("Move Y", input.y);

                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                //Ejecuto la corrutina si el camino está disponible
                if (WayIsAvailable(targetPosition))
                {
                    StartCoroutine(MoveTowards(targetPosition));
                }
            }
        }   
    }

    //Se ejecuta después de todos los updates, es el "ultimo" 
    //Es el encargado de ver lo que pasa en el ultimo frame de las acciones, en este caso la animacion 
    private void LateUpdate()
    {
        _animator.SetBool("Is Moving", isMoving);
    }

    IEnumerator MoveTowards(Vector3 destination)
    {
        //Antes de mover, tengo que ver que realmente pueda, porque si hay un collider no podria atravesarlo, por ende un while infinito (nunca llegaria a la posicion)
        isMoving = true;

        while(Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
               
            //No tengo nada para devolver y espero el siguiente frame
            yield return null;
        }

        transform.position = destination;
        isMoving = false;

        CheckForPokemon();
    }

    
    private bool WayIsAvailable(Vector3 target)
    {
        //Verifica que el camino esté disponible para dirigirse y prender la corrutina
        if (Physics2D.OverlapCircle(target, 0.15f, solidObjectsLayer) != null)
        {
            //Esto devuelve colliders
            return false;
        }

        return true;
    }

    private void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.15f, pokemonLayer) != null)
        {
            if(Random.Range(0, 100) < 15){
                Debug.Log("Empezar batalla pokemon");
            }
        }
    }
}
