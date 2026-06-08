using System.Collections.Generic;

namespace YG
{
    /// <summary>
    /// Поля сохранения прогресса уровней
    /// </summary>
    public partial class SavesYG
    {
        public bool levelProgressInitialized;
        public int lastUnlockedLevelIndex = 1;
        public int totalVictories;
        public List<string> completedLevelIds = new();
    }
}