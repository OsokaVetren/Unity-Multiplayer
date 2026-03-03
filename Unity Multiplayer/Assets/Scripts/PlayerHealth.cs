using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health = 100;

    private Vector3 spawnPoint;

    void Start()
    {
        spawnPoint = transform.position;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        if (health <= 0) return;
        health -= damage;

        if (health <= 0)
        {
            // Вместо прямого перемещения вызываем TargetRpc
            Respawn();
        }
    }

    void Respawn()
    {
        health = 100;
        // Отправляем команду конкретному клиенту, который владеет этим игроком
        TargetRespawn(connectionToClient, spawnPoint);
    }

    // TargetRpc выполняется ТОЛЬКО на том клиенте, которому принадлежит этот объект
    [TargetRpc]
    void TargetRespawn(NetworkConnection target, Vector3 position)
    {
        // 1. Отключаем контроллер, чтобы он не блокировал перемещение
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        // 2. Перемещаем
        transform.position = position;

        // 3. Сбрасываем физику (если есть Rigidbody)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // В новых версиях Unity .linearVelocity, в старых .velocity
            rb.angularVelocity = Vector3.zero;
        }

        // 4. Включаем контроллер обратно
        if (controller != null) controller.enabled = true;

        Debug.Log("Клиент перемещен в точку спавна");
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (newHealth <= 0) Debug.Log("Смерть!");
    }
}