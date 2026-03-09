using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Settings")]
    public float recoilX = -2f;      // На сколько подбросит вверх
    public float recoilY = 0.5f;     // Разброс влево-вправо
    public float recoilZ = 0.35f;    // Удар в плечо (назад)
    
    public float snappiness = 6f;    // Скорость самого рывка
    public float returnSpeed = 2f;   // Скорость возврата в исходное состояние

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    void Update()
    {
        // Плавно возвращаем целевое вращение к нулю
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        // Плавно подтягиваем текущее вращение к целевому
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        
        // Применяем вращение
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void FireRecoil()
    {
        // Добавляем случайное смещение при выстреле
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}