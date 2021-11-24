using System;
using CommandShell.Helpers;

namespace CommandShell.Infrastucture
{
    public class DefaultCommandActivator : ICommandActivator
    {
        public virtual object Create(Type type)
        {
            Asserts.OperationNotAllowed(type.GetConstructor(Type.EmptyTypes) == null, string.Format("Options type '{0}' has to be a non static, non abstract class and provide parameterless constructor.", type));
            return Activator.CreateInstance(type);
        }

        public virtual void Release(object command)
        {
            var disposable = command as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
