using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health = 100;

    private Vector3 spawnPoint; // Точка возрождения

    void Start()
    {
        // Запоминаем место, где игрок появился в самом начале
        spawnPoint = transform.position;
    }

    [Server] // Этот метод может вызываться только на сервере
    public void TakeDamage(int damage)
    {
        if (health <= 0) return; // Чтобы не умирать дважды

        health -= damage;

        if (health <= 0)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        // 1. Восстанавливаем здоровье на сервере
        health = 100;

        // 2. Перемещаем игрока в начальную точку
        // ВАЖНО: Если у вас есть CharacterController, его нужно временно выключить
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        transform.position = spawnPoint;

        if (controller != null) controller.enabled = true;

        // 3. Сообщаем клиентам, что нужно сбросить визуальные эффекты (если есть)
        RpcOnRespawn();
    }

    [ClientRpc]
    void RpcOnRespawn()
    {
        // Здесь можно сбросить эффекты, например, очистить экран от крови
        Debug.Log("Я возродился!");
        
        // Если вы при смерти отключали скрипты управления, включите их здесь
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        // Обновление UI здоровья
        Debug.Log($"HP: {newHealth}");
    }
}