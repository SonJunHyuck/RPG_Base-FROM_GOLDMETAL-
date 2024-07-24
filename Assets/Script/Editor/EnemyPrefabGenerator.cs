using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

#if UNIUTY_EDITOR

public class EnemyPrefabGenerator : EditorWindow
{
    public Object fromObj;
    public Object toObj;

    [MenuItem("Window/CustomWindow/Enemy")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<EnemyPrefabGenerator>();
    }

    private void OnGUI()
    {
        // 필요한 프리펩 지정
        fromObj = EditorGUILayout.ObjectField("A", fromObj, typeof(GameObject), false);
        toObj = EditorGUILayout.ObjectField("B", toObj, typeof(GameObject), false);

        // 버튼 누르면 실행
        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    void Generate()
    {
        GameObject clonedObject = Instantiate(fromObj, Vector3.zero, Quaternion.identity) as GameObject;
        GameObject clonedObject2 = Instantiate(toObj, Vector3.zero, Quaternion.identity) as GameObject;
        // B 게임 오브젝트에 A 게임 오브젝트의 컴포넌트를 추가합니다.
        foreach (Component component in clonedObject.GetComponents<Component>())
        {
            if (component != null)
            {
                // 컴포넌트를 복사하여 B 게임 오브젝트에 추가합니다.
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(clonedObject2);
            }
        }
    }
}

#endif