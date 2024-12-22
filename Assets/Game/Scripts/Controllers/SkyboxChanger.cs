using UnityEngine;

public class SkyboxChanger : Singleton<SkyboxChanger>
{
    public void ChangeSkybox(Material skyboxMaterial)
    {
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }
}
