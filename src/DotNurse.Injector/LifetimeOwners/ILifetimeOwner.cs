namespace DotNurse.Injector.LifetimeOwners
{
    internal interface ILifetimeOwner
    {
        void TakeOwnership(object obj);
    }
}
