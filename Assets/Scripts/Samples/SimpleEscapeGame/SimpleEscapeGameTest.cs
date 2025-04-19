using Escaper.Samples.SimpleEscapeGame;
using UnityEngine;
using UnityEngine.UI;

namespace Escaper.Samples.SimpleEscapeGame
{
    /// <summary>
    /// サンプルゲームのテストクラス！めっちゃ便利だよ！
    /// </summary>
    public class SimpleEscapeGameTest : MonoBehaviour
    {
        [SerializeField] private SimpleEscapeGame _game;
        [SerializeField] private Button _pickupKeyButton;
        [SerializeField] private Button _pickupNoteButton;
        [SerializeField] private Button _solveMainPuzzleButton;
        [SerializeField] private Button _solveSubPuzzleButton;

        void Start()
        {
            // ボタンの設定！
            _pickupKeyButton.onClick.AddListener(() => _game.PickupItem("key"));
            _pickupNoteButton.onClick.AddListener(() => _game.PickupItem("note"));
            _solveMainPuzzleButton.onClick.AddListener(() => _game.SolvePuzzle("main"));
            _solveSubPuzzleButton.onClick.AddListener(() => _game.SolvePuzzle("sub"));
        }

        void OnDestroy()
        {
            // ボタンの解除！
            _pickupKeyButton.onClick.RemoveAllListeners();
            _pickupNoteButton.onClick.RemoveAllListeners();
            _solveMainPuzzleButton.onClick.RemoveAllListeners();
            _solveSubPuzzleButton.onClick.RemoveAllListeners();
        }
    }
}