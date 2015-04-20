using System;
using System.Reflection;
using System.Reflection.Emit;

namespace GrEmit.Utils
{
    //TODO test
    public class EmitHelpers
    {
        public static T CreateDelegate<T>(DynamicMethod dm, object target) where T : class
        {
            var @delegate = dm.CreateDelegate(typeof(T), target);
            var result = (@delegate as T);
            if(result == null)
                throw new ArgumentException(String.Format("Type {0} not a delegate", typeof(T)));
            return result;
        }

        public static T CreateDelegate<T>(DynamicMethod dm) where T : class
        {
            var @delegate = dm.CreateDelegate(typeof(T));
            var result = (@delegate as T);
            if(result == null)
                throw new ArgumentException(String.Format("Type {0} not a delegate", typeof(T)));
            return result;
        }

        public static T EmitDynamicMethod<T>(string name, Module m, Action<GroboIL> emitCode) where T : class
        {
            var delegateType = typeof(T);
            //HACK
            var methodInfo = delegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            if(methodInfo == null)
                throw new ArgumentException(String.Format("Type {0} not a Delegate", delegateType));

            var dynamicMethod = new DynamicMethod(name, methodInfo.ReturnType, GetParameterTypes(methodInfo.GetParameters()), m, true);
            emitCode(new GroboIL(dynamicMethod));
            return CreateDelegate<T>(dynamicMethod);
        }

        public static T EmitDynamicMethod<T, TTarget>(string name, Module m, Action<GroboIL> emitCode, TTarget target) where T : class
        {
            var delegateType = typeof(T);
            //HACK
            var methodInfo = delegateType.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
            if(methodInfo == null)
                throw new ArgumentException(String.Format("Type {0} not a Delegate", delegateType));

            var dynamicMethod = new DynamicMethod(name, methodInfo.ReturnType, Concat(typeof(TTarget), GetParameterTypes(methodInfo.GetParameters())), m, true);
            emitCode(new GroboIL(dynamicMethod));
            return CreateDelegate<T>(dynamicMethod, target);
        }

        public static Type[] GetParameterTypes(ParameterInfo[] parameters)
        {
            var result = new Type[parameters.Length];
            for(var i = 0; i < parameters.Length; i++)
            {
                var parameterInfo = parameters[i];
                result[i] = parameterInfo.ParameterType;
            }
            return result;
        }

        public static Type[] Concat(Type a, Type[] b)
        {
            var result = new Type[b.Length + 1];
            result[0] = a;
            Array.Copy(b, 0, result, 1, b.Length);
            return result;
        }
    }
}