using System;
using System.Reflection;
using Unity.Entities;
using Unity.Properties;
using Unity.Properties.Adapters;

class ComponentEditorMethod
{
	abstract class Visitor : IPropertyVisitorAdapter
	{
		public abstract void Init(MethodInfo method, ParameterInfo[] args);
	}

	class Visitor<T> : Visitor, IVisit<T>
		where T : struct, IComponentData
	{
		delegate void InspectorGUIDelegate(ref T value, string label);
		delegate void InspectorGUIDelegateWithHide(ref T value, string label, out bool shouldHideComponent);

		Delegate m_guiMethod;

		public override void Init(MethodInfo method, ParameterInfo[] args)
		{
			m_guiMethod = Delegate.CreateDelegate(args.Length == 1 ? typeof(InspectorGUIDelegate) : typeof(InspectorGUIDelegateWithHide), method);
		}

		public VisitStatus Visit<TContainer>(Property<TContainer, T> property, ref TContainer container, ref T value)
		{
			bool shouldHideComponent = false;
			if(m_guiMethod is InspectorGUIDelegate guiDelegate) guiDelegate(ref value, property.Name);
			else if(m_guiMethod is InspectorGUIDelegateWithHide guiDelegateWithOverride) guiDelegateWithOverride(ref value, property.Name, out shouldHideComponent);
			return shouldHideComponent ? VisitStatus.Stop : VisitStatus.Handled;
		}
	}

	public static bool TryGetAdapter(Type componentType, out IPropertyVisitorAdapter visitor)
	{
		var method = componentType.GetMethod("OnInspectorGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		if (method != null && method.ReturnType == typeof(void))
		{
			var args = method.GetParameters();
			if ((args.Length == 1 && args[0].ParameterType == typeof(string)) ||
				(args.Length == 2 && args[0].ParameterType == typeof(string) && args[1].ParameterType.Name == "Boolean&"))
			{
				var result = (Visitor)Activator.CreateInstance(typeof(Visitor<>).MakeGenericType(componentType));
				result.Init(method, args);
				visitor = result;
				return true;
			}
		}
		visitor = null;
		return false;
	}
}