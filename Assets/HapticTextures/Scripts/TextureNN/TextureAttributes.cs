using UnityEngine;

public class TextureAttributes : MonoBehaviour
{
    [HideInInspector]
    public TextureAttributeStore texture;
    
    [SerializeField,HideInInspector]
    private Renderer currentRenderer;

    public Texture2D heightMap;
    public Vector3 heightMapSize, heightMapPos, minBounds, maxBounds;

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateTexture();
    }
#endif

    private void OnEnable()
    {
        UpdateTexture();
    }

    private void UpdateTexture()
    {
        currentRenderer = GetComponent<Renderer>();
        heightMap = currentRenderer.sharedMaterial.GetTexture("_ParallaxMap") as Texture2D;
        heightMapSize = currentRenderer.bounds.size;
        heightMapPos = currentRenderer.transform.position;
        minBounds = GetMinBounds(heightMapSize, heightMapPos);
        maxBounds = GetMaxBounds(heightMapSize, heightMapPos);
    }

    private Vector3 GetMinBounds(Vector3 _heightMapSize, Vector3 _heightMapPos)
    {
        return new Vector3(_heightMapPos.x - _heightMapSize.x / 2, 0, _heightMapPos.z - _heightMapSize.z / 2);
    }

    private Vector3 GetMaxBounds(Vector3 _heightMapSize, Vector3 _heightMapPos)
    {
        return new Vector3(_heightMapPos.x + _heightMapSize.x / 2, 0, _heightMapPos.z + _heightMapSize.z / 2);
    }
}