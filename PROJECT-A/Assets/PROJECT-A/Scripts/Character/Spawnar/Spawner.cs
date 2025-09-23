using A;
using System.Collections;
using System.Resources;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] GameObject CharacterPrefab;
    [SerializeField] GameObject CharacterPrefab2;
    [SerializeField] GameObject CharacterPrefab3;
    [SerializeField] GameObject bossPrefab;
    void Start()
    {
        var start = new Vector2(-2, 2);
        Instantiate(CharacterPrefab, start, Quaternion.identity);
        Instantiate(CharacterPrefab2, start + new Vector2(2, -2), Quaternion.identity);
        Instantiate(CharacterPrefab3, start + new Vector2(4, -2), Quaternion.identity);
        //for (int i = 0; i < 4; i++)
        //{
        //    var p = start + new Vector2(i % 2, i / 2);
        //    Instantiate(CharacterPrefab, p, Quaternion.identity);
        //}
        var t = ResourceManagerEX.Singleton.Instantiate("Coppybara", new Vector2(4, 2), Quaternion.identity);
        t.SetActive(true);
    }
}
