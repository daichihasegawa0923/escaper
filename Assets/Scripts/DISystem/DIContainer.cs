#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DISystem
{
    public abstract class DIContainer
    {
        protected abstract Dictionary<Type, object> Instances { get; }

        public object? Get(Type type)
        {
            var target = Instances.Where(instance => type == instance.GetType()).FirstOrDefault().Value;
            if (target == null)
            {
                return null;
            }
            return target;
        }

        public void Inject(object target)
        {
            var fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<InjectField>(true) != null)
                {
                    field.SetValue(target, Get(field.FieldType));
                }
            }
        }
    }
}