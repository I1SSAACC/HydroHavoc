using UnityEngine;

public class RigBoneModifier : MonoBehaviour
{
    [SerializeField] private Transform[] _constraints;

    private void Update()
    {
        foreach (Transform constraint in _constraints)
            constraint.position = transform.position;
    }
}