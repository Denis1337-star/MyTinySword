using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Следит за уничтожением главных баз и завершает матч.
/// </summary>
public class GameResultController : MonoBehaviour
{
    [SerializeField] private GameResultPanel resultPanel;

    private Castle playerCastle;
    private Castle enemyCastle;
    private bool gameFinished;

    private void Awake()
    {
        ResolveReferences();
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void ResolveReferences()
    {
        if (resultPanel == null)
            resultPanel = FindObjectOfType<GameResultPanel>(true);

        Castle[] castles = FindObjectsOfType<Castle>(true);

        foreach (Castle castle in castles)
        {
            if (castle == null || castle.FactionMember == null)
                continue;

            if (castle.FactionMember.Faction == FactionType.Player)
                playerCastle = castle;
            else if (castle.FactionMember.Faction == FactionType.Enemy)
                enemyCastle = castle;
        }
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

        gameFinished = true;

        if (destroyedCastle == playerCastle)
            resultPanel?.ShowDefeat();
        else if (destroyedCastle == enemyCastle)
            resultPanel?.ShowVictory();

        Time.timeScale = 0f;
    }
}
