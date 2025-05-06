using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class HeadHider : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _renderer;

    public void Hide() =>
        _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
}