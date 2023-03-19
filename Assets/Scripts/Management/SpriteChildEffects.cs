using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SpriteChildEffects : MonoBehaviour
{

    [SerializeField] SpriteRenderer[] childSpriteRenderers;

    public Color color;
    public Material material;



    void OnEnable()
    {
        UpdateRenderers();
    }

    void OnTransformChildrenChanged()
    {
        //Debug.Log("SpriteSorter.OnTransformChildrenChanged()");
        UpdateRenderers();
    }

    void UpdateChildSprites()
    {
        childSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void UpdateRenderers()
    {
#if UNITY_EDITOR
        UpdateChildSprites();
        foreach (SpriteRenderer sr in childSpriteRenderers)
        {
            if (material != null)
                sr.material = material;
            if (color != null)
                sr.color = color;
        }
#endif
    }


}
