# EntityComponentInspector
Package for implementing custom entity component inspectors. updated and modified by Alec C (roguesaloon). Find original package below
https://github.com/OndrejPetrzilka/EntityComponentInspector.git

## Installation
Add following to your package manifest (Packages/manifest.json):

`"entity-inspector-extension": "https://github.com/roguesaloon/EntityComponentInspector"`

## Usage
### Adding inspector code directly to component struct

Add method `void OnEditorGUI(string label)` to your component.

```
using Unity.Entities;

public struct DebugName : IComponentData
{
    public NativeString64 Value;

#if UNITY_EDITOR
    void OnEditorGUI(string label)
    {
        Value = new NativeString64(UnityEditor.EditorGUILayout.TextField(label, Value.ToString()));
    }
#endif
}
```

additionally there is now an optional override (added by roguesaloon) to not render the component in the inspector (allowing it to be completely overridden, or hidden)

Add method `void OnEditorGUI(string label, out bool shouldOverride)` to your component. And set shouldOverride to false to hide component in editor.

```
  
### Writing separate editor class

Create new class implementing `IComponentEditor<T>` where T is component type.

```
using Unity.Properties;
using UnityEditor;

public class DebugNameEditor : IComponentEditor<DebugName>
{
    public VisitStatus Visit<TContainer>(Property<TContainer, T> property, ref TContainer container, ref T value)
    {
        value.Value = new NativeString64(EditorGUILayout.TextField(property.GetName(), value.Value.ToString()));

        return VisitStatus.Handled; // Change this to VisitStatus.Stop to prevent component being rendered at all
    }
}
```
