using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
#if UNIUTY_EDITOR
public class ItemPrefabGenerator : EditorWindow
{
    // 게임오브젝트를 리스트로 받기 위해
    public GameObject[] Items;
    ScriptableObject scriptableObj;
    SerializedObject serialObj;
    SerializedProperty serialProp;

    // 이펙트로 넣어줄 프리펩
    private Object Light;
    private Object Particle;

    [MenuItem("Window/CustomWindow/Item")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ItemPrefabGenerator>();
    }

    private void OnEnable()
    {
        // 리스트 초기 작업
        scriptableObj = this;
        serialObj = new SerializedObject(scriptableObj);
        serialProp = serialObj.FindProperty("Items");
    }

    private void OnGUI()
    {
        // 커스텀 에디터에서 리스트는 수정된 것을 계속 업데이트 하는 방법으로 보여줌
        EditorGUILayout.PropertyField(serialProp, true);
        serialObj.ApplyModifiedProperties();
        serialObj.UpdateIfRequiredOrScript();

        // 필요한 프리펩 지정
        Light = EditorGUILayout.ObjectField("Light", Light, typeof(GameObject), false);
        Particle = EditorGUILayout.ObjectField("Particle", Particle, typeof(GameObject), false);

        // 버튼 누르면 실행
        if(GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    public void Generate()
    {
        foreach(GameObject item in Items)
        {
            // RigidBody가 있다 -> 이미 한번 Generate 했다 -> 필요한 작업만 한다.
            if(item.GetComponent<Rigidbody>() == null)
            {
                item.AddComponent<Rigidbody>();

                SphereCollider col = item.AddComponent<SphereCollider>();
                col.center = new Vector3(0, 1, 0);
                col.radius = 0.5f;

                SphereCollider trigger = item.AddComponent<SphereCollider>();
                trigger.center = new Vector3(0, 2, 0);
                trigger.radius = 3;

                item.AddComponent<Item>();
            }
            
            // 라이트 달아주기
            Transform light = item.transform.Find("EffectLight");
            if (light == null)
            {
                GameObject itemObj = Instantiate(Light) as GameObject;
                light = itemObj.transform;
                light.SetParent(item.transform);
                light.name = Light.name;
            }

            light.localPosition = Vector3.zero;
            light.localRotation = Quaternion.identity;
            light.localScale = Vector3.one;

            Transform particle = item.transform.Find("EffectParticle");
            if (particle == null)
            {
                GameObject itemObj = Instantiate(Particle) as GameObject;
                particle = itemObj.transform;
                particle.SetParent(item.transform);
                particle.name = Particle.name;
            }

            // 파티클 달아주기
            particle.SetParent(item.transform);
            particle.localPosition = Vector3.zero;
            particle.localRotation = Quaternion.identity;
            particle.localScale = Vector3.one;

            // item.transform.position = Vector3.zero;
        }
    }
    
}

#endif