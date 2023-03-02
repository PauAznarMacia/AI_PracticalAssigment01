using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject powerUp;
    public GameObject hunter;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            if (GameObject.Find("POWER UP"))
            {
                Destroy(GameObject.Find("POWER UP"));
            }
            GameObject newPowerUp = (GameObject)Instantiate(powerUp, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)), Quaternion.identity); 
            newPowerUp.name = "POWER UP";

        }
        /*
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {

            if (GameObject.Find("HUNTER"))
            {
                Destroy(GameObject.Find("HUNTER"));
            }
            GameObject newHunter = (GameObject)Instantiate(hunter, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)), Quaternion.identity);
            newHunter.name = "HUNTER";
            hunter.SetActive(true);

        }*/
    }
}
