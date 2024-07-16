using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class ItemPrefabGenerator : EditorWindow
{
    // ���ӿ�����Ʈ�� ����Ʈ�� �ޱ� ����
    public GameObject[] Items;
    ScriptableObject scriptableObj;
    SerializedObject serialObj;
    SerializedProperty serialProp;

    // ����Ʈ�� �־��� ������
    private Object Light;
    private Object Particle;

    [MenuItem("Window/CustomWindow/Item")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ItemPrefabGenerator>();
    }

    private void OnEnable()
    {
        // ����Ʈ �ʱ� �۾�
        scriptableObj = this;
        serialObj = new SerializedObject(scriptableObj);
        serialProp = serialObj.FindProperty("Items");
    }

    private void OnGUI()
    {
        // Ŀ���� �����Ϳ��� ����Ʈ�� ������ ���� ��� ������Ʈ �ϴ� ������� ������
        EditorGUILayout.PropertyField(serialProp, true);
        serialObj.ApplyModifiedProperties();
        serialObj.UpdateIfRequiredOrScript();

        // �ʿ��� ������ ����
        Light = EditorGUILayout.ObjectField("Light", Light, typeof(GameObject), false);
        Particle = EditorGUILayout.ObjectField("Particle", Particle, typeof(GameObject), false);

        // ��ư ������ ����
        if(GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    public void Generate()
    {
        foreach(GameObject item in Items)
        {
            // RigidBody�� �ִ� -> �̹� �ѹ� Generate �ߴ� -> �ʿ��� �۾��� �Ѵ�.
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
            
            // ����Ʈ �޾��ֱ�
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

            // ��ƼŬ �޾��ֱ�
            particle.SetParent(item.transform);
            particle.localPosition = Vector3.zero;
            particle.localRotation = Quaternion.identity;
            particle.localScale = Vector3.one;

            // item.transform.position = Vector3.zero;
        }
    }
    
}
