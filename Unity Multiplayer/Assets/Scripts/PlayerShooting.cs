using Mirror;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private int damage = 10;
    [SerializeField] private float range = 100f;
    [SerializeField] private GameObject hitEffectPrefab; // Префаб искр/крови

    void Update()
    {
        // Только владелец персонажа может стрелять
        if (!isLocalPlayer) return;

        if (Input.GetButtonDown("Fire1"))
        {
            // Передаем позицию и направление камеры
            Camera cam = Camera.main;
            CmdShoot(cam.transform.position, cam.transform.forward);
        }
    }

    // Выполняется на СЕРВЕРЕ
    [Command]
    void CmdShoot(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Проверяем, попали ли в игрока
            PlayerHealth targetHealth = hit.transform.GetComponent<PlayerHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                RpcShowHitEffect(hit.point, hit.normal);    
            }
        }
    }

    // Выполняется на ВСЕХ КЛИЕНТАХ
    [ClientRpc]
    void RpcShowHitEffect(Vector3 pos, Vector3 normal)
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        }
    }
}