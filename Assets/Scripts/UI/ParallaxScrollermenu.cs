using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    public ParallaxLayerMenu[] layers;

    private float imageWidth;

    void Start()
    {
        if (layers.Length > 0 && layers[0].image1 != null && layers[0].image2 != null)
        {
            // Asumimos que ambas imágenes tienen el mismo ancho
            imageWidth = layers[0].image1.GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    void Update()
    {
        foreach (ParallaxLayerMenu layer in layers)
        {
            MoveLayer(layer);
        }
    }

    void MoveLayer(ParallaxLayerMenu layer)
    {
        layer.image1.position += Vector3.left * layer.speed * Time.deltaTime;
        layer.image2.position += Vector3.left * layer.speed * Time.deltaTime;

        // Si image1 sale completamente de la pantalla, la movemos detrás de image2
        if (layer.image1.position.x <= -imageWidth)
        {
            layer.image1.position += Vector3.right * imageWidth * 2f;
        }

        if (layer.image2.position.x <= -imageWidth)
        {
            layer.image2.position += Vector3.right * imageWidth * 2f;
        }
    }
}

[System.Serializable]
public class ParallaxLayerMenu
{
    public Transform image1;
    public Transform image2;
    public float speed = 1f;
}