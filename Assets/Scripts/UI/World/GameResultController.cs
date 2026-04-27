using UnityEngine;

/// <summary>
/// Следит за уничтожением главных баз и завершает матч
/// </summary>
public class GameResultController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Castle playerCastle;
    [SerializeField] private Castle enemyCastle;
    [SerializeField] private GameResultPanel resultPanel;

    private bool gameFinished;

    private void Awake()
    {
        ValidateReferences();
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void ValidateReferences()
    {
        if (playerCastle == null)
            Debug.LogError($"{name}: Player Castle is not assigned.", this);

        if (enemyCastle == null)
            Debug.LogError($"{name}: Enemy Castle is not assigned.", this);

        if (resultPanel == null)
            Debug.LogError($"{name}: GameResultPanel is not assigned.", this);
    }

    private void Subscribe()
    {
        if (playerCastle != null)
            playerCastle.OnCastleDestroyed += OnCastleDestroyed;

        if (enemyCastle != null)
            enemyCastle.OnCastleDestroyed += OnCastleDestroyed;
    }

    private void Unsubscribe()
    {
        if (playerCastle != null)
            playerCastle.OnCastleDestroyed -= OnCastleDestroyed;

        if (enemyCastle != null)
            enemyCastle.OnCastleDestroyed -= OnCastleDestroyed;
    }

    private void OnCastleDestroyed(Castle destroyedCastle)
    {
        if (gameFinished || destroyedCastle == null)
            return;

        if (destroyedCastle == playerCastle)
        {
            FinishGame(false);
            return;
        }

        if (destroyedCastle == enemyCastle)
        {
            FinishGame(true);
            return;
        }
    }

    private void FinishGame(bool victory)
    {
        gameFinished = true;

        if (victory)
            resultPanel?.ShowVictory();
        else
            resultPanel?.ShowDefeat();

        Time.timeScale = 0f;
    }
}
