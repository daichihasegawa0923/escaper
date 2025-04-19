using System;
using System.Collections.Generic;
using Escaper.Core.Events;

namespace Escaper.Core.GameLogic
{
    /// <summary>
    /// 謎解き要素を管理するクラス！めっちゃ面白い謎解きを作れるよ！
    /// </summary>
    public class Puzzle
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSolved { get; private set; }
        public List<string> Hints { get; private set; }
        public List<string> RequiredItems { get; private set; }
        public Action OnSolve { get; set; }

        public Puzzle(string name, string description)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
            IsSolved = false;
            Hints = new List<string>();
            RequiredItems = new List<string>();
        }

        /// <summary>
        /// ヒントを追加するよ！
        /// </summary>
        public void AddHint(string hint)
        {
            Hints.Add(hint);
        }

        /// <summary>
        /// 必要なアイテムを追加するよ！
        /// </summary>
        public void AddRequiredItem(string itemId)
        {
            RequiredItems.Add(itemId);
        }

        /// <summary>
        /// 謎解きを解くよ！
        /// </summary>
        public bool Solve(List<string> usedItems)
        {
            if (IsSolved) return true;

            // 必要なアイテムがすべて使われているかチェック！
            bool hasAllItems = RequiredItems.TrueForAll(item => usedItems.Contains(item));

            if (hasAllItems)
            {
                IsSolved = true;
                OnSolve?.Invoke();
                GameEventSystem.Publish("PuzzleSolved", Id);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 謎解きをリセットするよ！
        /// </summary>
        public void Reset()
        {
            IsSolved = false;
            GameEventSystem.Publish("PuzzleReset", Id);
        }

        /// <summary>
        /// ヒントを取得するよ！
        /// </summary>
        public string GetHint(int index)
        {
            if (index >= 0 && index < Hints.Count)
            {
                return Hints[index];
            }
            return null;
        }
    }
}