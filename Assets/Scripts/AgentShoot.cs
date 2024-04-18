using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class AgentShoot : MonoBehaviour
{
    float timer;
    public float timeBetweenBullets = 0.15f;
    Ray shootRay = new Ray();
    RaycastHit hit;
    [SerializeField] private AgentInputs _playerInputs;
    [SerializeField] private Move_Look_Agnt ai;
    [SerializeField] private GameObject shootPos;
    [SerializeField] private Camera mainCam;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private Material winMaterial;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (_playerInputs.IsShooting)
        {
            Shoot();
        }

    }

    public void Shoot()
    {
        if (timer < timeBetweenBullets || Time.timeScale == 0)
        {
            return;
        }
        timer = 0f;

        bool enemyHit = false;
        if (Physics.Raycast(shootPos.transform.position, mainCam.transform.forward, out hit))
        {
            Debug.DrawRay(shootPos.transform.position, mainCam.transform.forward* 10);

            if (hit.transform.tag == "Enemy")
            {
                ai.AddReward(1);
                floorMeshRenderer.material = winMaterial;
                hit.transform.GetComponent<AI_Box_Target>().Kill();
                ai.EndEpisode();
                enemyHit = true;
            }
        }

        if (!enemyHit && ai != null)
        {
            ai.AddReward(-0.1f);
        }
    }

}
