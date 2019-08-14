using System;

namespace Singer.Core.Messaging
{
    /// <summary> 通信接口 </summary>
    public interface IMessenger
    {
        /// <summary> 注册消息 </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <param name="receiver">接收者</param>
        /// <param name="action">消息执行方法</param>
        /// <param name="token">消息令牌</param>
        void Register<TMessage>(object receiver, Action<TMessage> action, object token = null);

        /// <summary> 注册消息 </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="receiver"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        void Register<TMessage>(TMessage receiver, Action action, object token = null);

        /// <summary> 通知消息 </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="token"></param>
        void Notify<TMessage>(TMessage message, object token = null);

        /// <summary> 通知消息 </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="token"></param>
        void Notify<TMessage>(object token = null);

        /// <summary> 通知消息 </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TReceive"></typeparam>
        /// <param name="message"></param>
        /// <param name="token"></param>
        void Notify<TMessage, TReceive>(TMessage message, object token = null);

        /// <summary> 通知消息 </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TReceive"></typeparam>
        /// <param name="token"></param>
        void Notify<TMessage, TReceive>(object token = null);

        /// <summary> 注销消息 </summary>
        /// <param name="receiver"></param>
        void Unregister(object receiver);

        /// <summary> 注销消息 </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="receiver"></param>
        /// <param name="token"></param>
        /// <param name="action"></param>
        void Unregister<TMessage>(object receiver, object token = null, Action<TMessage> action = null);
    }
}
