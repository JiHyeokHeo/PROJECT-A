using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 추후 사이즈가 커지면 Controller로 분리
public class CharaterBase : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] Color selecetedColor = Color.cyan;
    [SerializeField] Color normalColor = Color.white;

    bool selected;
    Color originColor;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    
}