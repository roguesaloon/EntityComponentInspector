using Unity.Properties.Adapters;

public interface IComponentEditor<T> : IComponentEditor, IVisit<T>
{
}
