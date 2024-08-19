using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    private static int _height = 20;
    private static int _width = 12;
    public Transform[,] grid = new Transform[_width, _height];
    public PlayerController player;
    public bool canSpawn = true;
    public Transform blockHolder;
    public float fallTime = 0.6f;
    public int medalAmount;
    public GameObject medal;
    private Transform medalHolder;

    public TextMeshProUGUI medalText;

    private int medalCollected;

    private Animator anim;
    public bool isWon = false;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        medalHolder = transform.Find("MedalHolder");
        SpawnRandomMedal();
    }

    private void SpawnRandomMedal()
    {
        if (medalHolder.childCount == 0)
        {
            List<Vector2Int> gridPositions = RandomGridPositions();

            foreach (Vector2Int position in gridPositions)
            {
                Instantiate(medal, new Vector3(position.x, position.y, 0), Quaternion.identity, medalHolder);
            }
        }
        else
        {
            foreach (Transform child in medalHolder)
            {
                Destroy(child.gameObject);
            }
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {

            IsWonAnimation();
        }
    }
    private List<Vector2Int> RandomGridPositions()
    {

        List<Vector2Int> pos = new List<Vector2Int>();
        int minSpacing = 3; 

        while (pos.Count < medalAmount)
        {
            Vector2Int randomPosition = new Vector2Int(Random.Range(1, _width), Random.Range(2, _height - 1));

            // Check if the new position is at least 'minSpacing' away from existing positions
            bool isValid = true;
            foreach (Vector2Int existingPosition in pos)
            {
                if (Mathf.Abs(randomPosition.x - existingPosition.x) < minSpacing ||
                    Mathf.Abs(randomPosition.y - existingPosition.y) < minSpacing)
                {
                    isValid = false;
                    break;
                }
            }
            if (randomPosition.y == 2) randomPosition.y = 4;
            if (isValid && !pos.Contains(randomPosition))
            {
                pos.Add(randomPosition);
            }
        }
        return pos;

    }

    public void IncreaseMedalScore()
    {
        medalCollected++;
        UIController.instance.ChangeMedalText(medalCollected);
    }

    public void IsWonAnimation()
    {
        player.GetComponent<BoxCollider2D>().enabled = false;
        canSpawn = false;
        float originalY = transform.position.y;
        transform.DOMoveY(originalY + 30, 1.5f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            foreach (Transform child in blockHolder)
            {
                Destroy(child.gameObject);
            }
            // After the first movement, set the Y position to -30
            transform.position = new Vector3(transform.position.x, originalY - 30, transform.position.z);

            // Move back to the original Y position over 2 seconds with ease
            transform.DOMoveY(originalY, 1.5f).SetEase(Ease.InOutBack);
            
        });
    }

}
