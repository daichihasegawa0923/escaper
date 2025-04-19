using Escaper.Core.Events;
using Escaper.Samples.SimpleEscapeGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Escaper.Samples.SimpleEscapeGame
{
    /// <summary>
    /// サンプルゲームのシーンを管理するクラス！めっちゃカッコいいよ！
    /// </summary>
    public class SimpleEscapeGameScene : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] private SimpleEscapeGame _game;
        [SerializeField] private SimpleEscapeGameTest _test;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private GameObject _keyObject;
        [SerializeField] private GameObject _noteObject;
        [SerializeField] private GameObject _doorObject;

        [Header("Buttons")]
        [SerializeField] private Button _pickupKeyButton;
        [SerializeField] private Button _pickupNoteButton;
        [SerializeField] private Button _solveMainPuzzleButton;
        [SerializeField] private Button _solveSubPuzzleButton;
        [SerializeField] private Button _showHintButton;

        private void Start()
        {
            // ボタンの設定！
            SetupButtons();

            // イベントの購読！
            GameEventSystem.Subscribe("ItemPicked", OnItemPicked);
            GameEventSystem.Subscribe("PuzzleSolved", OnPuzzleSolved);
        }

        private void OnDestroy()
        {
            // イベントの購読解除！
            GameEventSystem.Unsubscribe("ItemPicked", OnItemPicked);
            GameEventSystem.Unsubscribe("PuzzleSolved", OnPuzzleSolved);
        }

        /// <summary>
        /// ボタンの設定をするよ！
        /// </summary>
        private void SetupButtons()
        {
            _pickupKeyButton.onClick.AddListener(() =>
            {
                _game.PickupItem("key");
                _keyObject.SetActive(false);
                UpdateStatusText();
            });

            _pickupNoteButton.onClick.AddListener(() =>
            {
                _game.PickupItem("note");
                _noteObject.SetActive(false);
                UpdateStatusText();
            });

            _solveMainPuzzleButton.onClick.AddListener(() =>
            {
                _game.SolvePuzzle("main");
                _doorObject.SetActive(false);
                UpdateStatusText();
            });

            _solveSubPuzzleButton.onClick.AddListener(() =>
            {
                _game.SolvePuzzle("sub");
                UpdateStatusText();
            });

            _showHintButton.onClick.AddListener(() =>
            {
                ShowHint();
            });
        }

        /// <summary>
        /// ステータステキストを更新するよ！
        /// </summary>
        private void UpdateStatusText()
        {
            string status = "現在の状態:\n";
            status += _game.HasItem("key") ? "✅ 鍵を取得済み\n" : "❌ 鍵を未取得\n";
            status += _game.HasItem("note") ? "✅ メモを取得済み\n" : "❌ メモを未取得\n";
            status += _game.IsPuzzleSolved("main") ? "✅ ドアを開けた\n" : "❌ ドアを未開放\n";
            status += _game.IsPuzzleSolved("sub") ? "✅ メモの謎を解いた\n" : "❌ メモの謎を未解決\n";

            _statusText.text = status;
        }

        /// <summary>
        /// ヒントを表示するよ！
        /// </summary>
        private void ShowHint()
        {
            if (_game.HasItem("note") && !_game.IsPuzzleSolved("sub"))
            {
                _hintText.text = "ヒント: メモには「1234」と書かれているよ！";
            }
            else
            {
                _hintText.text = "ヒント: まずは部屋の中を探してみよう！";
            }
        }

        /// <summary>
        /// 謎解きが解けたときの処理！
        /// </summary>
        private void OnPuzzleSolved(object puzzleId)
        {
            Debug.Log($"謎解き {puzzleId} を解いたよ！");
            UpdateStatusText();
        }

        /// <summary>
        /// アイテムを取得したときの処理！
        /// </summary>
        private void OnItemPicked(object itemId)
        {
            Debug.Log($"アイテム {itemId} を取得したよ！");
            UpdateStatusText();
        }
    }
}