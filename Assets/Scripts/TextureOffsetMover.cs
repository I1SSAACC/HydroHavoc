using UnityEngine;

public class TextureOffsetMover : MonoBehaviour
{
    public float speed = 0.5f;
    private Material material;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
        else
        {
            Debug.LogError("Renderer �� ������ �� ������� " + gameObject.name);
        }
    }

    void Update()
    {
        if (material != null)
        {
            float offset = Time.time * speed;
            material.mainTextureOffset = new Vector2(0, offset);
        }
        else
        {
            Debug.LogError("�������� �� ��������������� �� ������� " + gameObject.name);
        }
    }
}
