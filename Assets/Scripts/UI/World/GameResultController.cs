using UnityEngine;

/// <summary>
/// Следит за уничтожением главных баз и завершает матч
/// </summary>
public sealed class GameResultController : ValidatedMonoBehaviour
{
    [SerializeField] private Castle _playerCastle;
    [SerializeField] private Castle _enemyCastle;
    [SerializeField] private GameResultPanel _resultPanel;

    private bool _gameFinished;

    protected override void Awake()
    {
        base.Awake();

        if (!enabled)
            return;

        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    protected override bool ValidateInternal()
    {
        bool valid = true;

        valid &= ValidationUtility.IsAssigned(this, _playerCastle, nameof(_playerCastle));
        valid &= ValidationUtility.IsAssigned(this, _enemyCastle, nameof(_enemyCastle));
        valid &= ValidationUtility.IsAssigned(this, _resultPanel, nameof(_resultPanel));

        return valid;
    }

    private void Subscribe()
    {
        _playerCastle.OnCastleDestroyed += OnCastleDestroyed;
        _enemyCastle.OnCastleDestroyed += OnCastleDestroyed;
    }

    private void Unsubscribe()
    {
        _playerCastle.OnCastleDestroyed -= OnCastleDestroyed;
        _enemyCastle.OnCastleDestroyed -= OnCastleDestroyed;
    }

    private void OnCastleDestroyed(Castle destroyedCastle)
    {
        if (_gameFinished || destroyedCastle == null)
            return;

        if (destroyedCastle == _playerCastle)
        {
            FinishGame(false);
            return;
        }

        if (destroyedCastle == _enemyCastle)
        {
            FinishGame(true);
            return;
        }
    }

    private void FinishGame(bool victory)
    {
        _gameFinished = true;
        Debug.Log("ава");

        if (victory)
            _resultPanel.ShowVictory();
        else
            _resultPanel.ShowDefeat();
    }
}