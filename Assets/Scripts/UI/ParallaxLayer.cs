using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LayerMode
{
    RepeatByCycle, 
    RandomEachTime 
}

[System.Serializable]
public class SpriteSet
{
    public Sprite[] sprites;
}

public class ParallaxLayer : MonoBehaviour
{
    public LayerMode mode = LayerMode.RepeatByCycle;

    public float scrollSpeed = 1f;
    public Sprite[] spriteVariants;

    [Header("RepeatByCycle Settings")]
    public int repeatThreshold = 5;

    public float spriteHeight = 10f;
    private Transform[] parts;
    private int recycleCount = 0;
    private int currentSpriteIndex = 0;

    [Header("Nivelado")]
    public List<SpriteSet> levelSprites = new List<SpriteSet>();


    private void OnEnable()
    {
        if (GameProgressManager.Instance != null){ 
            GameProgressManager.Instance.OnLevelChanged += OnLevelChanged;}
       
    }

    private void OnDisable()
    {
        if (GameProgressManager.Instance != null){
            GameProgressManager.Instance.OnLevelChanged -= OnLevelChanged;}
            
    }

    void OnLevelChanged(int newLevel)
    {
        if (newLevel < levelSprites.Count){
        spriteVariants = levelSprites[newLevel].sprites;}

        foreach (Transform part in parts)
        {
            if (mode == LayerMode.RandomEachTime)
                AssignRandomSprite(part);
            else
                ChangeToNextSprite(part);
        }
            
    }

    void Start()
    {
        SpriteRenderer sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteHeight = sr.bounds.size.y;

        parts = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            parts[i] = transform.GetChild(i);
        }

        if (mode == LayerMode.RandomEachTime)
        {
            foreach (Transform part in parts)
            {
                AssignRandomSprite(part);
            }
        }
    }

    void Update()
    {
        foreach (Transform part in parts)
        {
            part.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

            float cameraBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
            float spriteBottom = part.position.y + spriteHeight / 2f;

            if (spriteBottom < cameraBottom)
            {
                float highestY = GetHighestPartY();
                part.position = new Vector3(part.position.x, highestY + spriteHeight, part.position.z);

                if (mode == LayerMode.RepeatByCycle)
                {
                    recycleCount++;

                    if (recycleCount % repeatThreshold == 0)
                    {
                        ChangeToNextSprite(part);
                    }
                }
                else if (mode == LayerMode.RandomEachTime)
                {
                    AssignRandomSprite(part);
                }
            }
        }
    }

    float GetHighestPartY()
    {
        float highest = float.MinValue;
        foreach (Transform part in parts)
        {
            if (part.position.y > highest)
                highest = part.position.y;
        }
        return highest;
    }

     void ChangeToNextSprite(Transform part)
    {
        currentSpriteIndex = (currentSpriteIndex + 1) % spriteVariants.Length;

        var sr = part.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteVariants[currentSpriteIndex];
        }
    }

    void AssignRandomSprite(Transform part)
    {
        if (spriteVariants.Length == 0) return;

        int index = Random.Range(0, spriteVariants.Length);
        var sr = part.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = spriteVariants[index];
        }
    }
}
