using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IPlantCelestials : MonoBehaviour {

    public GameObject ArbitraryCelestial;
    public Queue<GameObject> celestialsQueue;
    private int counter = 1;
    public float yDistance;
    public Sprite[] sprites;
    private Vector2 prefabPos;
    public float xRndRange;


    // Use this for initialization
    void Start () {
        sprites = Resources.LoadAll<Sprite>("celestialspritesheet");
        prefabPos = GameObject.Find("StartCelestial").transform.position;
        celestialsQueue = new Queue<GameObject>();
        for (int i = 0; i < 5; i++) {
            CelestialFactory();
        }
    }

    public Queue<GameObject> getCelestialsQueue() {
        return celestialsQueue;
    }

    public void CelestialFactory() {
        prefabPos.y += yDistance;
        prefabPos.x = Random.Range(-xRndRange, xRndRange);
        GameObject obj = (GameObject)Instantiate(ArbitraryCelestial, prefabPos, Quaternion.identity);
        obj.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0,100)];
        obj.name = "prefab" + counter;
        celestialsQueue.Enqueue(obj);
        counter++;
    }
}
