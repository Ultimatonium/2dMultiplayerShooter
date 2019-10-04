using UnityEngine;
using UnityEngine.Networking;

public class SetRandomPosition : NetworkBehaviour
{
    private void Start()
    {
        if (transform.parent == null) return;
        ResetPosition(transform.transform.parent.lossyScale.x);
    }

    [ServerCallback]
    public void ResetPosition(float scale)
    {
        float pos = 5 * scale;
        transform.Translate(new Vector2(Random.Range(pos, -pos), Random.Range(pos, -pos)), Space.World);
    }
}
