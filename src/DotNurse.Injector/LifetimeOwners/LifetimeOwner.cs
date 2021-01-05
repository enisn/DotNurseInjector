using System;
using System.Collections.Generic;

namespace DotNurse.Injector.LifetimeOwners
{
    internal class LifetimeOwner : ILifetimeOwner, ISingletonOwner, IScopedOwner, IDisposable
    {
        private readonly List<object> objects = new List<object>();
        private bool isDisposed;

        public void TakeOwnership(object obj)
        {
            if (!objects.Contains(obj))
                objects.Add(obj);
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;

            foreach (var obj in objects)
            {
                (obj as IDisposable)?.Dispose();
            }

            objects.Clear();
        }
    }
}
