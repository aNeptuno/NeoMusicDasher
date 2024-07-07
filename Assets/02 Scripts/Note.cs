using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public bool isWrongNote;
    [SerializeField] private GameObject note;

    [SerializeField] private ParticleSystem noteParticles;

    void Start()
    {
        if (isWrongNote) note.SetActive(false);
    }

    void Update()
    {
        if (transform.position.y < -6f)
        {
           StartCoroutine(WaitAndDestroy());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Limit"))
            StartCoroutine(WaitAndDestroy());
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().canLoseLife)
            {
                if (isWrongNote)
                {
                    GameManager.Instance.UpdateScore(-100);
                    Player.Instance.LoseLife();
                    AudioManager.Instance.PlayWrong();
                }
                else
                {
                    GameManager.Instance.SpawnNotesParticles(noteParticles, transform);
                    GameManager.Instance.UpdateScore(200);
                    AudioManager.Instance.PlayNote();
                }
                StartCoroutine(WaitAndDestroy());
            }
        }
    }

    IEnumerator WaitAndDestroy()
    {
        if (!isWrongNote) GameManager.Instance.UpdateScore(-10);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
