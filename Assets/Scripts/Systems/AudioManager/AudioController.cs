using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class AudioController : MonoBehaviour
{
#if UNITY_EDITOR
    public List<AudioControls> dependencies;    //todo: later change this to type only so i can automate with just selecting an event and getting the controls for it
                                                // todo: need to separate the audio controller from the audio maker4
    public UnityEvent action;
#endif
    // TODO: can add slider with Slider(float) and IntSlider(int) as well as MinMaxSlider(Vector2)
    // TODO: implement priority dependencies in order (i believe currently it just overrides each other)
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioController))]
public class AudioControllerCustomEditor : Editor
{
    Dictionary<string, object[]> methodInputs = new();
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioController script = (AudioController)target;

        if (!Application.isPlaying)
        {
            if (GUILayout.Button("Refresh Methods"))
            {
                methodInputs.Clear();
                Debug.Log("Refreshed Methods");
                Undo.RecordObject(script, "Refresh Methods");
                EditorUtility.SetDirty(script);
                serializedObject.Update();
                Repaint();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Method Buttons", EditorStyles.boldLabel);
            createMethodButtons();
        }
        else
        {
            EditorGUILayout.HelpBox("Refresh only available in Edit Mode.", MessageType.Info);
        }
    }

    private void createMethodButtons()
    {
        AudioController script = (AudioController)target;
        var dependencies = script.dependencies;

        foreach (var dependency in dependencies)
        {
            if (dependency == null) { continue; }
            var voidMethods = dependency.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(m => m.ReturnType == typeof(void) && !m.IsSpecialName);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("(" + dependency.GetType().Name + ")", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            foreach (var method in voidMethods)
            {
                generateCorrectButton(dependency, method);
                EditorGUILayout.Space();
            }
        }
        if (!dependencies.Any(item => item != null) || methodInputs.Count == 0)
        {
            EditorGUILayout.HelpBox("No methods available. Try adding dependencies!", MessageType.Info);
        }
    }

    private void generateCorrectButton(AudioControls source, MethodInfo method)
    {
        if (method == null) { return; }
        int buttonsPerRow = 2;
        int count = 0;
        ParameterInfo[] parameters = method.GetParameters();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(method.Name, EditorStyles.boldLabel);
        if (parameters.Length != 0)
        {
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
        }
        bool expand;
        string methodKey = method.Name + string.Join(", ", parameters.Select(p => p.ParameterType.Name));
        if (!methodInputs.ContainsKey(methodKey))
        {
            methodInputs[methodKey] = CreateDefaultValues(method.GetParameters());
        }
        object[] cachedInputs = methodInputs[methodKey];
        for (int i = 0; i < parameters.Length; i++)
        {
            Type type = parameters[i].ParameterType;
            string paramName = parameters[i].Name;
            object currentParamValue = cachedInputs[i];
            expand = (count % buttonsPerRow) == 1;
            if (type == typeof(int)) { cachedInputs[i] = EditorGUILayout.IntField(paramName, (int)(currentParamValue ?? 0), GUILayout.ExpandWidth(expand)); }
            else if (type == typeof(string)) { cachedInputs[i] = EditorGUILayout.TextField(paramName, (string)(currentParamValue ?? ""), GUILayout.ExpandWidth(expand)); }
            else if (type == typeof(float)) { cachedInputs[i] = EditorGUILayout.FloatField(paramName, (float)(currentParamValue ?? 0.0f), GUILayout.ExpandWidth(expand)); }
            else if (type == typeof(bool)) { cachedInputs[i] = EditorGUILayout.Toggle(paramName, (bool)(currentParamValue ?? false), GUILayout.ExpandWidth(expand)); }
            else if (type.IsEnum) { cachedInputs[i] = EditorGUILayout.EnumPopup(paramName, (Enum)currentParamValue != null ? (Enum)currentParamValue : (Enum)Enum.GetValues(type).GetValue(0), GUILayout.ExpandWidth(expand)); }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(type)) { cachedInputs[i] = EditorGUILayout.ObjectField(paramName, currentParamValue as UnityEngine.Object, type, true, GUILayout.ExpandWidth(expand)); }
            // could include vector fields but i dont think it is necessary
            else
            {
                EditorGUILayout.LabelField(paramName, "ERROR", GUILayout.ExpandWidth(expand));
            }
            count++;
            if (count % buttonsPerRow == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }
        if (count % buttonsPerRow != 0 && parameters.Length != 0)
        {
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
        }
        if (GUILayout.Button("Call"))
        {
            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = cachedInputs[i];
            }

            method.Invoke(source, args);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private object[] CreateDefaultValues(ParameterInfo[] parameters)
    {
        object[] defaults = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            Type t = parameters[i].ParameterType;
            if (parameters[i].HasDefaultValue && parameters[i].DefaultValue != DBNull.Value)
            {
                defaults[i] = parameters[i].DefaultValue;
            }
            else if (t == typeof(int))
                defaults[i] = 0;
            else if (t == typeof(float))
                defaults[i] = 0f;
            else if (t == typeof(bool))
                defaults[i] = false;
            else if (t == typeof(string))
                defaults[i] = "";
            else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
                defaults[i] = null;
            else
                defaults[i] = null; // fallback for unsupported types
        }

        return defaults;
    }
}
#endif