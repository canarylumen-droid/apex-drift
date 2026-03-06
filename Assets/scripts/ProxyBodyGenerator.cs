using UnityEngine;

/// <summary>
/// Phase 19: Proxy Body Generator.
/// The ultimate safeguard against "Empty Street" syndrome.
/// If an FBX fails to load, this script builds a 3D proxy instantly.
/// </summary>
public static class ProxyBodyGenerator
{
    public static GameObject CreateHumanoidProxy(string label)
    {
        GameObject root = new GameObject("Proxy_" + label);
        
        // Head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0, 1.7f, 0);
        head.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        
        // Body (Torso)
        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torso.transform.SetParent(root.transform);
        torso.transform.localPosition = new Vector3(0, 1.0f, 0);
        torso.transform.localScale = new Vector3(0.6f, 1.0f, 0.3f);
        
        // Color based on type
        Renderer r = torso.GetComponent<Renderer>();
        if (label.Contains("SWAT")) r.material.color = Color.blue;
        else if (label.Contains("Dancer")) r.material.color = Color.magenta;
        else r.material.color = Color.white;

        // Add Label in 3D Space
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(root.transform);
        textObj.transform.localPosition = new Vector3(0, 2.2f, 0);
        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = label.ToUpper();
        tm.fontSize = 20;
        tm.alignment = TextAnchor.MiddleCenter;
        tm.anchor = TextAnchor.MiddleCenter;

        return root;
    }

    public static GameObject CreateCarProxy(string brand)
    {
        GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.name = "Proxy_" + brand;
        car.transform.localScale = new Vector3(2, 1, 4);
        
        Renderer r = car.GetComponent<Renderer>();
        r.material.color = (brand == "Lambo") ? Color.yellow : Color.red;

        // Add 4 Wheels (Proxy)
        for (int i = 0; i < 4; i++)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.transform.SetParent(car.transform);
            wheel.transform.localScale = new Vector3(0.4f, 0.1f, 0.4f);
            float x = (i % 2 == 0) ? 0.6f : -0.6f;
            float z = (i < 2) ? 0.7f : -0.7f;
            wheel.transform.localPosition = new Vector3(x, -0.4f, z);
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }

        return car;
    }
}
