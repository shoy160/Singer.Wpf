using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Singer.Core.Messaging
{
    /// <summary> 消息机制 </summary>
    public class Messenger : IMessenger
    {
        private ConcurrentDictionary<Type, List<ActionAndToken>> _receiveActions;
        private readonly object _registerLock = new object();
        /// <summary> 默认的消息管理器 </summary>
        public static Messenger Default => Singleton<Messenger>.Instance ??
                                           (Singleton<Messenger>.Instance = new Messenger());

        public void Register<TMessage>(object receiver, Action<TMessage> action, object token = null)
        {
            if (receiver == null)
                return;
            Register(receiver.GetType(), action, token);
        }

        public void Register<TMessage>(Type receiverType, Action<TMessage> action, object token = null)
        {
            var messageType = typeof(TMessage);
            _receiveActions = _receiveActions ?? new ConcurrentDictionary<Type, List<ActionAndToken>>();
            if (!_receiveActions.TryGetValue(messageType, out var actions))
            {
                actions = new List<ActionAndToken>();
                _receiveActions.TryAdd(messageType, actions);
            }
            actions.Add(new ActionAndToken<TMessage>(action, token, receiverType));
        }

        public void Register<TMessage>(TMessage receiver, Action action, object token = null)
        {
            Register<TMessage>(receiver, msg => action.Invoke(), token);
        }

        public void Notify<TMessage>(TMessage message, object token = null)
        {
            NotifyTargetOrType(message, null, token);
        }
        public void Notify<TMessage>(object token = null)
        {
            NotifyTargetOrType(default(TMessage), null, token);
        }

        public void Notify<TMessage, TReceive>(TMessage message, object token = null)
        {
            NotifyTargetOrType(message, typeof(TReceive), token);
        }

        public void Notify<TMessage>(TMessage message, Type receiveType, object token = null)
        {
            NotifyTargetOrType(message, receiveType, token);
        }

        public void Notify<TMessage, TReceive>(object token = null)
        {
            NotifyTargetOrType(default(TMessage), typeof(TReceive), token);
        }

        public void Unregister(object receiver)
        {
            Unregister<bool>(receiver);
        }

        public void Unregister<TMessage>(object receiver, object token = null, Action<TMessage> action = null)
        {
            if (receiver == null || _receiveActions == null)
                return;
            lock (_registerLock)
            {
                var len = _receiveActions.Keys.Count;
                for (var i = 0; i < len; i++)
                {
                    var key = _receiveActions.Keys.ToArray()[i];
                    List<ActionAndToken> receiveActions;
                    if (!_receiveActions.TryGetValue(key, out receiveActions))
                        continue;
                    var actionLen = receiveActions.Count;
                    for (var j = 0; j < actionLen; j++)
                    {
                        var actionAndToken = receiveActions[j];
                        if (actionAndToken.TargetType != receiver.GetType())
                            continue;
                        if (token != null && (actionAndToken.Token == null || !actionAndToken.Token.Equals(token)))
                            continue;
                        if (action != null)
                        {
                            var item = (ActionAndToken<TMessage>)actionAndToken;
                            if (item.Action != action)
                                continue;
                        }
                        receiveActions.Remove(actionAndToken);
                        j--;
                        actionLen--;
                    }
                    if (receiveActions.Any())
                        continue;
                    _receiveActions.TryRemove(key, out receiveActions);
                    i--;
                    len--;
                }
            }
        }

        private void NotifyTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            if (_receiveActions == null || !_receiveActions.Any())
                return;
            var messageType = typeof(TMessage);
            _receiveActions = _receiveActions ?? new ConcurrentDictionary<Type, List<ActionAndToken>>();
            List<ActionAndToken> actions;
            if (!_receiveActions.TryGetValue(messageType, out actions) || actions == null || !actions.Any())
                return;
            var tokenActions =
                actions.Where(
                    t => t.Token == null && token == null || t.Token != null && t.Token.Equals(token));
            if (messageTargetType != null)
                tokenActions = tokenActions.Where(t => t.TargetType == messageTargetType ||
                                                       t.TargetType.IsSubclassOf(messageTargetType));
            var actionList = tokenActions.ToList();
            foreach (var action in actionList)
            {
                var messageAction = action as ActionAndToken<TMessage>;
                messageAction?.Action(message);
            }
        }

        private class ActionAndToken
        {
            public Type TargetType { get; protected set; }
            public object Token { get; protected set; }
        }

        private class ActionAndToken<T> : ActionAndToken
        {
            public Action<T> Action { get; }
            public ActionAndToken(Action<T> action, object token, Type targetType)
            {
                Action = action;
                Token = token;
                TargetType = targetType;
            }
        }
    }
}
