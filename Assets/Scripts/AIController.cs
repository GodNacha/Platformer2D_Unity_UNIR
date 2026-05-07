using UnityEngine;

public class AIController : MonoBehaviour
{
    CharacterController characterController2D;
    [SerializeField] Transform target;

    private void Awake()
    {
        characterController2D = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 rawMove = Vector2.zero;

        if (target)
        {
           if (transform.position.x > target.position.x)
           {
                rawMove = Vector2.left;
           }
           else
           {
                rawMove = Vector2.right;
           }

           if (Mathf.Abs(transform.position.x - target.position.x) < 0.1f) //Para que se quede quieto al llegar al target, y no se quede vibrando por estar cambiando constantemente de dirección
            {
                rawMove = Vector2.zero;
           }

        }
       
            characterController2D.SetRawMove(rawMove);
    }

    
}
