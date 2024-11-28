using UnityEngine;

public class LightingFix : MonoBehaviour
{
    private void Awake()
    {
        // Force the lighting settings of the active scene to reapply
        DynamicGI.UpdateEnvironment();
    }
}