using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int health = 100;

    [SerializeField] private int maxHealth = 100;

    private Image healthBarFill;

    private Vector3 spawnPoint;

    void Start()
    {

        if (isLocalPlayer)
        {
            // Ищем твой новый объект по имени
            GameObject fillObj = GameObject.Find("Healthbar_fill");
            if (fillObj != null)
            {
                healthBarFill = fillObj.GetComponent<Image>();
                UpdateUI(health);
            }
        }

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

        if (isLocalPlayer && healthBarFill != null)
        {
            UpdateUI(newHealth);
        }
    }

    void UpdateUI(int currentHealth)
    {
        // fillAmount принимает значения от 0.0 до 1.0
        // Делим текущее ХП на максимальное
        healthBarFill.fillAmount = 1 - (float)currentHealth / maxHealth;
    }
}