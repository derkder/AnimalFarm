using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuAnim : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer sr;
    public Image image;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        image.sprite = sr.sprite;
    }
}
