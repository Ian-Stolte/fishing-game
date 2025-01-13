using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float delay;
    public bool fade;

    void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        if (fade)
        {
            for (float i = 0; i < delay; i += 0.01f)
            {
                GetComponent<CanvasGroup>().alpha = 1 - Mathf.Pow(i/delay, 2);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }
        Destroy(gameObject);
    }
}
