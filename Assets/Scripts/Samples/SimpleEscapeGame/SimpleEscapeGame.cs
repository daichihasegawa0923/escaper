using Escaper.Core.Events;
using Escaper.Core.GameLogic;
using UnityEngine;

namespace Escaper.Samples.SimpleEscapeGame
{
    /// <summary>
    /// 簡単な脱出ゲームのサンプル！めっちゃ面白いよ！
    /// </summary>
    public class SimpleEscapeGame : MonoBehaviour
    {
        private GameState _gameState;
        private GameFlow _gameFlow;
        private Puzzle _mainPuzzle;
        private Puzzle _subPuzzle;

        void Start()
        {
            // ゲームの初期化！
            InitializeGame();

            // イベントの購読！
            GameEventSystem.Subscribe("PuzzleSolved", OnPuzzleSolved);
        }

        void OnDestroy()
        {
            // イベントの購読解除！
            GameEventSystem.Unsubscribe("PuzzleSolved", OnPuzzleSolved);
        }

        /// <summary>
        /// ゲームを初期化するよ！
        /// </summary>
        private void InitializeGame()
        {
            // ゲーム状態の初期化！
            _gameState = new GameState();
            _gameState.SetState("hasKey", false);
            _gameState.SetState("hasNote", false);

            // ゲームフローの初期化！
            _gameFlow = new GameFlow(_gameState);

            // メインの謎解きを作成！
            _mainPuzzle = new Puzzle("ドアの謎", "鍵を使ってドアを開けよう！");
            _mainPuzzle.AddRequiredItem("key");
            _mainPuzzle.OnSolve = () => Debug.Log("メインの謎解きを解いたよ！");

            // サブの謎解きを作成！
            _subPuzzle = new Puzzle("メモの謎", "メモに書かれている謎を解こう！");
            _subPuzzle.AddRequiredItem("note");
            _subPuzzle.AddHint("メモをよく読んでみよう！");
            _subPuzzle.OnSolve = () => Debug.Log("サブの謎解きを解いたよ！");
        }

        /// <summary>
        /// アイテムを取得するよ！
        /// </summary>
        public void PickupItem(string itemId)
        {
            _gameState.SetState($"has{itemId}", true);
            GameEventSystem.Publish("ItemPicked", itemId);
            Debug.Log($"{itemId}を取得したよ！");
        }

        /// <summary>
        /// アイテムを持っているかチェックするよ！
        /// </summary>
        public bool HasItem(string itemId)
        {
            return _gameState.GetState<bool>($"has{itemId}");
        }

        /// <summary>
        /// 謎解きを解くよ！
        /// </summary>
        public void SolvePuzzle(string puzzleId)
        {
            var usedItems = new System.Collections.Generic.List<string>();
            if (_gameState.GetState<bool>("hasKey")) usedItems.Add("key");
            if (_gameState.GetState<bool>("hasNote")) usedItems.Add("note");

            if (puzzleId == "main")
            {
                _mainPuzzle.Solve(usedItems);
            }
            else if (puzzleId == "sub")
            {
                _subPuzzle.Solve(usedItems);
            }
        }

        /// <summary>
        /// 謎解きが解けたかチェックするよ！
        /// </summary>
        public bool IsPuzzleSolved(string puzzleId)
        {
            if (puzzleId == "main")
            {
                return _mainPuzzle.IsSolved;
            }
            else if (puzzleId == "sub")
            {
                return _subPuzzle.IsSolved;
            }
            return false;
        }

        /// <summary>
        /// 謎解きが解けたときの処理！
        /// </summary>
        private void OnPuzzleSolved(object puzzleId)
        {
            Debug.Log($"謎解き {puzzleId} を解いたよ！");
            // ここでゲームの進行を制御するよ！
        }
    }
}