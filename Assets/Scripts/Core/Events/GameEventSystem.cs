using System;
using System.Collections.Generic;

namespace Escaper.Core.Events
{
    /// <summary>
    /// ゲームのイベントを管理するクラス！めっちゃ便利だよ！
    /// </summary>
    public static class GameEventSystem
    {
        private static readonly Dictionary<string, List<Action<object>>> _eventHandlers = new Dictionary<string, List<Action<object>>>();

        /// <summary>
        /// イベントを購読するよ！
        /// </summary>
        public static void Subscribe(string eventName, Action<object> handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<Action<object>>();
            }
            _eventHandlers[eventName].Add(handler);
        }

        /// <summary>
        /// イベントの購読を解除するよ！
        /// </summary>
        public static void Unsubscribe(string eventName, Action<object> handler)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName].Remove(handler);
            }
        }

        /// <summary>
        /// イベントを発行するよ！
        /// </summary>
        public static void Publish(string eventName, object data = null)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                foreach (var handler in _eventHandlers[eventName])
                {
                    handler.Invoke(data);
                }
            }
        }
    }
}