using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] GameObject CharacterPrefab;
    [SerializeField] GameObject bossPrefab;
    void Start()
    {
        var start = new Vector2(-2, 2);
        for (int i = 0; i < 4; i++)
        {
            var p = start + new Vector2(i % 2, i / 2);
            Instantiate(CharacterPrefab, p, Quaternion.identity);
        }
        var t =Instantiate(bossPrefab, new Vector2(4, 2), Quaternion.identity);
        t.SetActive(true);
    }
}
