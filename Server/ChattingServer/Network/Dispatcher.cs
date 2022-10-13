using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace Network
{
    public interface FunctionBase<Session, PacketIndex, Return>
    {
        public Return ExecuteFunction(Session _session, PacketBase<PacketIndex> _packet_base);
    }

    public class Dispatcher<Session, PacketIndex, Return> where PacketIndex : notnull
    {
        private Dictionary<PacketIndex, Info> m_func;
        private ReaderWriterLockSlim m_lock;
        private Return m_fail_value;

        public class Info
        {
            public Info()
            {
                Func = null;
                PacketType = null;
            }

            public FunctionBase<Session, PacketIndex, Return>? Func;
            public Type? PacketType;
        }

        public class FunctionPointer<PacketObject> : FunctionBase<Session, PacketIndex, Return> where PacketObject : PacketBase<PacketIndex>
        {
            public FunctionPointer(Func<Session, PacketObject, Return> _func, Return _fail)
                : base()
            {
                m_func = _func;
                m_fail_value = _fail;
            }

            public Return ExecuteFunction(Session _session, PacketBase<PacketIndex> _packet_base)
            {
                PacketObject obj;

                try
                {
                    obj = (PacketObject)_packet_base;
                }
                catch
                {
                    return m_fail_value;
                }

                return m_func(_session, obj);
            }

            public Func<Session, PacketObject, Return> m_func;
            public Return m_fail_value;
        }

        public Dispatcher(Return _dispatcher_fail_value)
        {
            m_func = new Dictionary<PacketIndex, Info>();
            m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            m_fail_value = _dispatcher_fail_value;
        }

        public void RegistFunction<PacketObject>(PacketIndex _packet_index, Func<Session, PacketObject, Return> _func) where PacketObject : PacketBase<PacketIndex>
        {
            m_lock.EnterWriteLock();

            if (m_func.ContainsKey(_packet_index) == false)
            {
                m_func.Add(_packet_index, new Info() { Func = new FunctionPointer<PacketObject>(_func, m_fail_value), PacketType = typeof(PacketObject) });
            }
            else
            {
                m_func[_packet_index].Func = new FunctionPointer<PacketObject>(_func, m_fail_value);
                m_func[_packet_index].PacketType = typeof(PacketObject);
            }

            m_lock.ExitWriteLock();
        }

        public void RegistClass(object logic)
        {
            Type type = logic.GetType();
            if (type.IsClass == false ||
                type.GetMethods().Length <= 0)
            {
                return;
            }

            foreach (MethodInfo methodinfo in type.GetMethods())
            {
                ParameterInfo[] parameters = methodinfo.GetParameters();
                if (parameters.Length != 2)
                {
                    continue;
                }

                if (typeof(Session).Equals(parameters[0].ParameterType)  == false ||
                    parameters[1].ParameterType.IsSubclassOf(typeof(PacketBase<PacketIndex>))  == false)
                {
                    continue;
                }

                object? packet = Activator.CreateInstance(parameters[1].ParameterType);
                if (packet == null)
                {
                    continue;
                }

                Func<Type[], Type> funcType;
                IEnumerable<Type> paramTypes = methodinfo.GetParameters().Select(p => p.ParameterType);
                if (methodinfo.ReturnType.Equals((typeof(void))) == true)
                {
                    funcType = Expression.GetActionType;
                }
                else
                {
                    funcType = Expression.GetFuncType;
                    paramTypes = paramTypes.Concat(new[] { methodinfo.ReturnType });
                }

                PacketBase<PacketIndex> packetBase = (PacketBase<PacketIndex>)packet;
                Delegate funcHandle = methodinfo.CreateDelegate(funcType(paramTypes.ToArray()), logic);
                Type thisType = this.GetType();
                MethodInfo? registfunc = thisType.GetMethod("RegistFunction");
                MethodInfo? genericfunc = registfunc?.MakeGenericMethod(parameters[1].ParameterType);
                object? result = genericfunc?.Invoke(this, new object[] { packetBase.Index, funcHandle });
            }
        }

        public void Clear()
        {
            m_lock.EnterWriteLock();
            m_func.Clear();
            m_lock.ExitWriteLock();
        }

        public bool GetFunction(PacketIndex _packet_index, out FunctionBase<Session, PacketIndex, Return>? _func, out Type? _packet_type)
        {
            _func = null;
            _packet_type = null;

            bool result = false;

            m_lock.EnterReadLock();

            if(m_func.ContainsKey(_packet_index) == true)
            {
                _func = m_func[_packet_index].Func;
                _packet_type = m_func[_packet_index].PacketType;
                result = true;
            }

            m_lock.ExitReadLock();

            return result;
        }
    }

    public interface FunctionBase<Session, PacketIndex>
    {
        public void ExecuteFunction(Session _session, PacketBase<PacketIndex> _packet_base);
    }

    public class Dispatcher<Session, PacketIndex> where PacketIndex : notnull
    {
        private Dictionary<PacketIndex, Info> m_func;
        private ReaderWriterLockSlim m_lock;
        public class Info
        {
            public Info()
            {
                Func = null;
                PacketType = null;
            }

            public FunctionBase<Session, PacketIndex>? Func;
            public Type? PacketType;
        }

        public class FunctionPointer<PacketObject> : FunctionBase<Session, PacketIndex> where PacketObject : PacketBase<PacketIndex>
        {
            public FunctionPointer(Action<Session, PacketObject> _func)
                : base()
            {
                m_func = _func;
            }

            public void ExecuteFunction(Session _session, PacketBase<PacketIndex> _packet_base)
            {
                PacketObject obj;

                try
                {
                    obj = (PacketObject)_packet_base;
                }
                catch
                {
                    return;
                }

                m_func(_session, obj);
            }

            public Action<Session, PacketObject> m_func;
        }

        public Dispatcher()
        {
            m_func = new Dictionary<PacketIndex, Info>();
            m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public void RegistFunction<PacketObject>(PacketIndex _packet_index, Action<Session, PacketObject> _func) where PacketObject : PacketBase<PacketIndex>
        {
            m_lock.EnterWriteLock();

            if (m_func.ContainsKey(_packet_index) == false)
            {
                m_func.Add(_packet_index, new Info() { Func = new FunctionPointer<PacketObject>(_func), PacketType = typeof(PacketObject) });
            }
            else
            {
                m_func[_packet_index].Func = new FunctionPointer<PacketObject>(_func);
                m_func[_packet_index].PacketType = typeof(PacketObject);
            }

            m_lock.ExitWriteLock();
        }

        public void RegistClass(object logic)
        {
            Type type = logic.GetType();
            if (type.IsClass == false||
               type.GetMethods().Length <= 0)
            {
                return;
            }

            foreach (MethodInfo methodinfo in type.GetMethods())
            {
                ParameterInfo[] parameters = methodinfo.GetParameters();
                if (parameters.Length != 2)
                {
                    continue;
                }

                if (typeof(Session).Equals(parameters[0].ParameterType) == false ||
                    parameters[1].ParameterType.IsSubclassOf(typeof(PacketBase<PacketIndex>)) == false)
                {
                    continue;
                }

                object? packet = Activator.CreateInstance(parameters[1].ParameterType);
                if (packet == null)
                {
                    continue;
                }

                Func<Type[], Type> funcType;
                IEnumerable<Type> paramTypes = methodinfo.GetParameters().Select(p => p.ParameterType);
                if (methodinfo.ReturnType.Equals((typeof(void))) == true)
                {
                    funcType = Expression.GetActionType;
                }
                else
                {
                    funcType = Expression.GetFuncType;
                    paramTypes = paramTypes.Concat(new[] { methodinfo.ReturnType });
                }

                PacketBase<PacketIndex> packetBase = (PacketBase<PacketIndex>)packet;
                Delegate funcHandle = methodinfo.CreateDelegate(funcType(paramTypes.ToArray()), logic);
                Type thisType = this.GetType();
                MethodInfo? registfunc = thisType.GetMethod("RegistFunction");
                MethodInfo? genericfunc = registfunc?.MakeGenericMethod(parameters[1].ParameterType);
                object? result = genericfunc?.Invoke(this, new object[] { packetBase.Index, funcHandle });
            }
        }

        public void Clear()
        {
            m_lock.EnterWriteLock();
            m_func.Clear();
            m_lock.ExitWriteLock();
        }

        public bool GetFunction(PacketIndex _packet_index, out FunctionBase<Session, PacketIndex>? _func, out Type? _packet_type)
        {
            _func = null;
            _packet_type = null;

            bool result = false;

            m_lock.EnterReadLock();

            if (true == m_func.ContainsKey(_packet_index))
            {
                _func = m_func[_packet_index].Func;
                _packet_type = m_func[_packet_index].PacketType;
                result = true;
            }

            m_lock.ExitReadLock();

            return result;
        }
    }
}
