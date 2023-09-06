using System;

namespace CommandShell.Infrastucture
{
    public interface ICommandActivator
    {
        object Create(Type type);

        void Release(object command);
    }
}
