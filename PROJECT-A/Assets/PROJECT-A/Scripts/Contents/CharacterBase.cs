using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���� ����� Ŀ���� Controller�� �и�
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