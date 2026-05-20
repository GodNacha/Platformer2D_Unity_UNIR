using UnityEngine;
using System.Collections;

public class Boxes : MonoBehaviour
{
    public float delayDestroy = 2.5f; // Tiempo en segundos antes de destruir la caja
    public void Destroy()
    {
        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(delayDestroy);
        Destroy(gameObject);
    }
}
