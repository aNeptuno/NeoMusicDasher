
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    #region "Singleton"
    public static Player Instance { get; private set; } // Singleton instance

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
    [SerializeField] private float moveSpeed = 3.0f;
    Rigidbody2D rb;
    private Vector3 targetPos;
    private Animator animator;
    private bool isMoving = false;
    private bool canMove;

    public int playerLives = 5;

    public bool canLoseLife;

    public Transform startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform;
    }

    // Update is called once per frame
    void Update()
    {
        canMove = GameManager.Instance.gameStarted;

        if (Input.GetMouseButtonDown(0) && canMove)
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;
            isMoving = true;
            UpdateAnimationDirection();
        }

        if (Input.GetMouseButtonUp(0) && canMove)
        {
            isMoving = false;
            rb.velocity = Vector2.zero;
        }

        if (isMoving && canMove)
        {
            MovePlayer();
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void MovePlayer()
    {
        Vector2 direction = (targetPos - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Check if the player has reached the target position
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
        }
    }

    private void UpdateAnimationDirection()
    {
        if (targetPos.x < transform.position.x)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", -1); // Move left
        }
        else
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", 1); // Move right
        }
    }

    public void LoseLife()
    {
        if (playerLives == 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            playerLives -= 1;
            UIController.Instance.UpdateLife(playerLives);
        }
    }

    IEnumerator Die()
    {
        animator.Play("Death");
        yield return new WaitForSeconds(1f);
        GameManager.Instance.EndGame();
    }

    public void ResetPlayerData()
    {
        transform.position = startPosition.position;
        rb.velocity = Vector2.zero;
        isMoving = false;
        canMove = false;
        canLoseLife = false;
        playerLives = 5;
        animator.Play("Idle");
    }
}
