using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class IgnitionScript : MonoBehaviour
{
    VisualEffect visualEffect;

    // Start is called before the first frame update
    void Start()
    {
        visualEffect = this.GetComponent<VisualEffect>();
    }

    public void SetColor(Color color)
    {
        visualEffect.SetVector4("Color", new Vector4(color.r, color.g, color.b, 1.0f));
    }

    public void FadeIn()
    {
        if (visualEffect == null)
        {
            visualEffect = this.GetComponent<VisualEffect>();
        }

        visualEffect.Stop();
        visualEffect.SetFloat("Rate", 1.0f);
        visualEffect.Play();
        StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(3f);
        FindObjectOfType<GameManager>().ReturnEffectToPool(gameObject);
    }
}
