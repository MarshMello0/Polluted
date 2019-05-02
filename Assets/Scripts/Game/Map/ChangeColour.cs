using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColour : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> materials = new List<MeshRenderer>();
    private List<Color> buildingColours = new List<Color>();
    
    private void Start()
    {
        AddColours();
        SetRandomColour();
    }

    /// <summary>
    /// This adds the colours to the list, as this script is going to be on different buildings
    /// I want to make sure all their colours are the same, so I just change the one script
    ///
    /// I could make this static 
    /// </summary>
    private void AddColours()
    {
        buildingColours.Add(new Color(255,0,0));
        buildingColours.Add(new Color(0,255,0));
        buildingColours.Add(new Color(0,0,255));
    }


    /// <summary>
    /// This picks a random colour for that building from the list
    /// </summary>
    private void SetRandomColour()
    {
        int index = Random.Range(0, buildingColours.Count - 1);
        Material mat = new Material(Shader.Find("HDRenderPipeline/Lit"));
        mat.shader = Shader.Find("HDRenderPipeline/Lit");
        mat.SetColor("_BaseColor",buildingColours[index]);
        foreach (MeshRenderer meshRenderer in materials)
        {
            meshRenderer.material = mat;
        }
    }
}
