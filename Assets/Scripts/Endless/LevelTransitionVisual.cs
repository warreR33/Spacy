using System.Collections;
using UnityEngine;

public class LevelTransitionVisual : MonoBehaviour
{
    public float transitionSpeed = 5f;

    private bool hasTriggeredTransition = false;
    private bool isTransitioning = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        gameObject.SetActive(false);
    }

    public void StartTransition()
    {
        isTransitioning = true;
        gameObject.SetActive(true);
        float spriteHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        float startY = mainCamera.transform.position.y + mainCamera.orthographicSize + spriteHeight / 2f;

        transform.position = new Vector3(mainCamera.transform.position.x, startY, 0);
    }

    void OnEnable()
    {
        hasTriggeredTransition = false;
    }

    void Update()
    {
        if (!isTransitioning) return;

        transform.Translate(Vector3.down * transitionSpeed * Time.deltaTime);

        float spriteHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        float bottomOfSprite = transform.position.y - spriteHeight / 2f;
        float topOfSprite = transform.position.y + spriteHeight / 2f;
        float bottomOfCamera = mainCamera.transform.position.y - mainCamera.orthographicSize;

        if (!hasTriggeredTransition && bottomOfSprite <= bottomOfCamera)
        {
            hasTriggeredTransition = true;
            GameProgressManager.Instance.CompleteTransition();
        }

        // Se desactiva cuando el sprite salió completamente de la pantalla
        if (topOfSprite <= bottomOfCamera)
        {
            isTransitioning = false;
            gameObject.SetActive(false);
        }
    }
}
