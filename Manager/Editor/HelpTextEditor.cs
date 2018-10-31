
using Framework.UI;
using UnityEditor;

namespace Framework.Editor
{
    //[CustomEditor(typeof(HelpText))]
    //class HelpTextEditor: UnityEditor.Editor
    //{
    //    private SerializedObject obj;
    //    private HelpText helptext;
    //    //private SerializedProperty type;
    //    private SerializedProperty isfloat;
    //    //private SerializedProperty height;

    //    void OnEnable()
    //    {
    //        obj = new SerializedObject(target);
    //        isfloat = obj.FindProperty("Isfloat");
    //    }


    //    public override void OnInspectorGUI()
    //    {
    //        helptext = (HelpText)target;
    //        helptext.m_Type = (HelpText.Type)EditorGUILayout.EnumPopup("m_Type", helptext.m_Type);

    //        if (helptext.m_Type == HelpText.Type.Money)
    //        {
    //            EditorGUILayout.PropertyField(isfloat);

    //        }

    //        base.OnInspectorGUI();
    //    }
    //}
}
