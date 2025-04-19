using System;
using System.Collections.Generic;

namespace Escaper.Core.GameLogic
{
    /// <summary>
    /// ゲームの状態を管理するクラス！めっちゃ重要だよ！
    /// </summary>
    public class GameState
    {
        private Dictionary<string, object> _stateData;

        public GameState()
        {
            _stateData = new Dictionary<string, object>();
        }

        /// <summary>
        /// 状態をセットするよ！型安全だから安心！
        /// </summary>
        public void SetState<T>(string key, T value)
        {
            _stateData[key] = value;
        }

        /// <summary>
        /// 状態を取得するよ！見つからない場合はデフォルト値を返すよ！
        /// </summary>
        public T GetState<T>(string key)
        {
            if (_stateData.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            return default;
        }

        /// <summary>
        /// その状態が存在するかチェックするよ！
        /// </summary>
        public bool HasState(string key)
        {
            return _stateData.ContainsKey(key);
        }
    }
}