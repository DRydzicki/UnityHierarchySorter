#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;

public class ObjectSorter : Editor
{
    [MenuItem("GameObject/Hierarchy Sorter/Sort Childrens", false, 10)]
    public static void SortChildren(MenuCommand command)
    {
        GameObject parent = command.context as GameObject;
        List<Transform> children = parent.GetComponentsInChildren<Transform>().ToList();
        Transform transform = parent.GetComponent<Transform>();

        SortChildrenByName(transform);
        foreach (Transform t in children)
            SortChildrenByName(t);

        void SortChildrenByName(Transform parent)
        {
            List<Transform> children = parent.Cast<Transform>().ToList();
            foreach (var child in children)
                if (Regex.IsMatch(child.name, @"\([0-9]*\)$"))
                    child.name = InsertZeros(child.name);

            children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
            for (int i = 0; i < children.Count; i++)
                children[i].SetSiblingIndex(i);

            foreach (var child in children)
                if (Regex.IsMatch(child.name, @"\([0-9]*\)$"))
                    child.name = RemoveZeros(child.name);
        }

        string InsertZeros(string name)
        {
            string number = name.Substring(name.LastIndexOf('(')+1, name.LastIndexOf(')') - name.LastIndexOf('(')-1);
            number = "(" + number.PadLeft(5, '0') + ")";
            Debug.Log(number);
            name = Regex.Replace(name, @"\(\d*\)", "");
            return name + number;
        }

        string RemoveZeros(string name)
        {
            string number = name.Substring(name.LastIndexOf('('), name.LastIndexOf(')') - name.LastIndexOf('(')+1);
            number = Regex.Replace(number, @"\([0]*", m => Regex.Replace(m.Value, @"[0]*", ""));
            name = Regex.Replace(name, @"\(\d*\)", "");
            return name + number;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    [MenuItem("GameObject/Hierarchy Sorter/Remove Numbers", false, 10)]
    public static void RemoveNumbers(MenuCommand command)
    {
        GameObject parent = command.context as GameObject;
        List<Transform> children = parent.GetComponentsInChildren<Transform>().ToList();
        Transform transform = parent.GetComponent<Transform>();

        if (children.Count == 0)
            RemoveNumbersInCollection(transform.Cast<Transform>().ToList());
        else
            foreach (Transform t in children) RemoveNumbersInCollection(t.Cast<Transform>().ToList());

        void RemoveNumbersInCollection(ICollection<Transform> collection)
        {
            foreach (Transform t in collection) t.name = NewName(t.name);
        }
        string NewName(string name) => Regex.Replace(name, @"\([^\d]*(\d+)[^\d]*\)", "").Trim();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }


}
#endif