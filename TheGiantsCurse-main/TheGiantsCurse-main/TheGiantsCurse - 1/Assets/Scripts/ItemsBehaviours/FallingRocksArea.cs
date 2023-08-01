using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRocksArea : MonoBehaviour
{
    public BoxCollider area;
    public float frequency;

    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject fallingAreaCircle;

    private GameObject rockInstance;
    private GameObject holeInstance;
    private Vector3 chosenPosition;
    private float tmp;

    private void Start()
    {
        tmp = frequency;
    }

    // Update is called once per frame
    void Update()
    {
        if(tmp > 0 && FinalTrackManagement.instance.activeBoulderArea)
        {
            tmp -= Time.deltaTime;
            if (tmp <= 0)
            {
                tmp = 0;
                chosenPosition = GetRandomPointInsideCollider(area);
                SpawnRock();
            }
        }
    }

    public Vector3 GetRandomPointInsideCollider(BoxCollider boxCollider)
    {
        Vector3 extents = boxCollider.size / 2f;
        Vector3 point = new Vector3(
            Random.Range(-extents.x, extents.x) + boxCollider.center.x,
            0,
            Random.Range(-extents.z, extents.z) + boxCollider.center.z
        );
        return boxCollider.transform.TransformPoint(point);
    }

    private void SpawnRock()
    {
        Debug.Log("Spawn rock!");
        rockInstance = Instantiate(rockPrefab, new Vector3(chosenPosition.x, 40, chosenPosition.z), Quaternion.identity);
        rockInstance.gameObject.GetComponent<Rigidbody>().AddTorque(transform.right * 5, ForceMode.Impulse);
        holeInstance = Instantiate(fallingAreaCircle, chosenPosition, Quaternion.identity);
        Destroy(holeInstance, 2f);
        holeInstance = null;
        tmp = frequency;
    }


}
