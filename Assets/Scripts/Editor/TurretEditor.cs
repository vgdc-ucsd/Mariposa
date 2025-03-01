using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Turret))]
public class TurretEditor : Editor
{
    public VisualTreeAsset VisualTree;

    private Turret turret;
    private Toggle canRotateToggle;
    private PropertyField rotationSpeed;

    private void OnEnable()
    {
        turret = (Turret)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);

        canRotateToggle = root.Q<Toggle>("canRotateToggle");
        rotationSpeed = root.Q<PropertyField>("rotationSpeed");

        canRotateToggle.value = turret.CanRotate;
        rotationSpeed.SetEnabled(turret.CanRotate);
        canRotateToggle.RegisterValueChangedCallback(OnCanRotateToggled);
        return root;
    }

    private void OnCanRotateToggled(ChangeEvent<bool> e)
    {
        rotationSpeed.SetEnabled(e.newValue);
        turret.CanRotate = e.newValue;
    }
}
