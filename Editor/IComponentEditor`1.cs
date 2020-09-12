using Unity.Properties.Adapters;

public interface IComponentInspector<T> : IComponentInspector, IVisit<T>
{
}
