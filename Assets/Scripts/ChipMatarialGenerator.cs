using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipMatarialGenerator : MonoBehaviour
{
    public static ChipMatarialGenerator Instance;
    [SerializeField] Material clipMaterial;
    [SerializeField] int genMatCount;
    [SerializeField] List<Material> materialList;
    [HideInInspector] public List<Material> unusedMaterialList;


#if UNITY_EDITOR
    private void OnValidate()
    {
        SingletonSetUp();
        GenerateMaterial();
    }
#endif

    private void SingletonSetUp()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void GenerateMaterial()
    {
        if (materialList.Count == genMatCount)
            return;
        if (materialList.Count > genMatCount)
            materialList.Clear();
        for (int i = 0; i < genMatCount; i++)
        {
            Material newMat = new Material(clipMaterial);
            newMat.color = Random.ColorHSV();
            materialList.Add(newMat);
        }
    }
    public Material GetChipMaterial() {
        if (unusedMaterialList.Count == 0) {
            unusedMaterialList = new List<Material>(materialList);
        }
        Material selectedMaterial = unusedMaterialList[Random.Range(0, unusedMaterialList.Count)];
        unusedMaterialList.Remove(selectedMaterial);
        return selectedMaterial;
    }
}
