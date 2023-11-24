using DG.Tweening;
using UnityEngine;

public class Cloth : ExecutableObject
{
    public ClothType type;
    public int colorCode;
    
    [SerializeField] private MeshRenderer clothMeshRenderer;
    [SerializeField] private int materialVisibilityPropertyID;
    [SerializeField] private Material clippableMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private float matPropertyStartValue = 0.08f;
    [SerializeField] private float matPropertyEndValue = -0.1f;
    [SerializeField] private bool reverseSewingDirection;

    private void Awake()
    {
        materialVisibilityPropertyID = Shader.PropertyToID("_Flow");
    }

    public void GetColored(float executionTime,Color targetColor)
    {
        ColoringPot coloringPot=executer as ColoringPot;
        colorCode = coloringPot.colorCode;
        clothMeshRenderer.material.DOColor(targetColor, executionTime);
    }

    public void GetSewed(float executionTime)
    {
        transform.localPosition = Vector3.zero; // stick to sew transform point start (parent)
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        clothMeshRenderer.material = clippableMaterial;

        if (reverseSewingDirection)
        {
            var propertyValue = matPropertyEndValue;
            DOTween.To(() => propertyValue, x => propertyValue = x, matPropertyStartValue, executionTime)
                .OnUpdate(() => { clippableMaterial.SetFloat(materialVisibilityPropertyID, propertyValue); }).OnComplete(
                    () => { clothMeshRenderer.material = normalMaterial; });
        }
        else
        {
            var propertyValue = matPropertyStartValue;
            DOTween.To(() => propertyValue, x => propertyValue = x, matPropertyEndValue, executionTime)
                .OnUpdate(() => { clippableMaterial.SetFloat(materialVisibilityPropertyID, propertyValue); }).OnComplete(
                    () => { clothMeshRenderer.material = normalMaterial; });
        }
        
    }
}

public enum ClothType
{
    Socks,
    Bra,
    Slip,
    Short
}