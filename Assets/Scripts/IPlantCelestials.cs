﻿using UnityEngine;
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
        int range = Random.Range(0, 95);
        Sprite temp = sprites[range];
        sprites[range] = sprites[95];
        for (int i = 95; i < 99; i++){
            sprites[i] = sprites[i+1];
        }
        sprites[99] = temp;
        obj.GetComponent<SpriteRenderer>().sprite = sprites[range];
        obj.name = "prefab" + counter;
        celestialsQueue.Enqueue(obj);
        counter++;
    }
}
