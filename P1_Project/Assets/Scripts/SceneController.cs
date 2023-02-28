using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject powerUp;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (GameObject.Find("PowerUp(Clone)"))
            {
                Destroy(GameObject.Find("PowerUp(Clone)"));
            }
            Instantiate(powerUp, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,1)), Quaternion.identity);
        }
    }
}
