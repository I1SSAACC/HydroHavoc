using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class HeadHider : MonoBehaviour
{
    public void Hide() =>
        GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
}