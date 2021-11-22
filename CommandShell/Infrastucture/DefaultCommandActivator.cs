using System;

namespace CommandShell.Infrastucture
{
    public class DefaultCommandActivator : ICommandActivator
    {
        public virtual object Create(Type type)
        {
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
