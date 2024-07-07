using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region "Singleton"
    public static GameManager Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // If not, set it to this instance
            DontDestroyOnLoad(gameObject); // Make this instance persist across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if another instance already exists
        }
    }
    #endregion
    public List<GameObject> notes;
    public float maxX;
    public Transform spawnPoint;
    private float spawnRate = 1.5f;

    public bool gameStarted = false;

    private float time;
    private int displayedTime;

    private int playerScore;

    private int gameDificulty = 1;

    public GameObject background;

    public List<Sprite> bgSprites;

    int currentSpriteIndex = 0;

    void Update()
    {
        if (gameStarted)
        {
            time += Time.deltaTime;
            int newTime = Mathf.FloorToInt(time);

            if (newTime != displayedTime)
            {
                displayedTime = newTime;
                if (UIController.Instance != null)
                {
                    UIController.Instance.UpdateTime(displayedTime);
                }
            }

            if (displayedTime > 20 * gameDificulty)
            {
                if (spawnRate == 0f)
                    NextLevel();
                spawnRate -= .5f;
                gameDificulty += 1;
            }
        }
    }

    private void NextLevel()
    {
        UIController.Instance.Fade(true);
        spawnRate = 1.5f;
        spawnRate -= .5f;
        if (spawnRate < 0) spawnRate = 0;
        AudioManager.Instance.PlayNextTrack();
        int newIndex = Random.Range(1, bgSprites.Count);
        if (newIndex == currentSpriteIndex)
        {
            if (currentSpriteIndex == bgSprites.Count - 1)
                newIndex--;
            else
                newIndex++;
        }
        currentSpriteIndex = newIndex;
        background.GetComponent<SpriteRenderer>().sprite = bgSprites[currentSpriteIndex];
    }

    private void StartSpawning()
    {
        InvokeRepeating("SpawnWrongNote", 2f, spawnRate * 3);
        InvokeRepeating("SpawnNote", 1f, spawnRate);
        Player.Instance.canLoseLife = true;
    }

    private void SpawnWrongNote()
    {
        SpawnNotePrefab(true);
    }

    private void SpawnNote()
    {
        SpawnNotePrefab(false);
    }

    private void SpawnNotePrefab(bool isWrong)
    {
        Vector3 spawnPos = spawnPoint.position;

        int index = Random.Range(0, notes.Count);

        spawnPos.x = Random.Range(-maxX, maxX);

        GameObject note = notes[index];
        note.GetComponent<Note>().isWrongNote = isWrong;

        Instantiate(note, spawnPos, Quaternion.identity);
    }

    public void StartGame()
    {
        gameStarted = true;
        UpdateScore(playerScore);
        UIController.Instance.UpdateTime(displayedTime);
        UIController.Instance.UpdateLife(3);

        StartSpawning();
    }

    public void UpdateScore(int points)
    {
        playerScore += points;
        if (playerScore < 0) playerScore = 0;
        if (UIController.Instance != null)
            UIController.Instance.UpdateScore(playerScore);
    }

    public void EndGame()
    {
        CancelInvoke("SpawnWrongNote");
        CancelInvoke("SpawnNote");
        Player.Instance.playerLives = 3;
        Player.Instance.SetStartPosition();
        UIController.Instance.ShowFinalStats(playerScore, displayedTime);
        gameStarted = false;
        playerScore = 0;
        time = 0;
        displayedTime = 0;
        spawnRate = 1.5f;
        currentSpriteIndex = 0;
        background.GetComponent<SpriteRenderer>().sprite = bgSprites[currentSpriteIndex];
    }

    public void SpawnNotesParticles(ParticleSystem noteParticles, Transform noteTransform)
    {
        StartCoroutine(SpawnAndDestroyNotesParticles(noteParticles, noteTransform));
    }
    IEnumerator SpawnAndDestroyNotesParticles(ParticleSystem noteParticles, Transform noteTransform)
    {
        ParticleSystem particlesInstance = Instantiate(noteParticles, noteTransform.position, Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(2f);
        particlesInstance.Stop();
        while (particlesInstance.IsAlive())
        {
            yield return null;
        }
        Destroy(particlesInstance.gameObject);
    }
}
