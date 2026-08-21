using System.Collections;
using UnityEngine;

public class BossRock : MonoBehaviour
{
    Rigidbody rigid;
    float angularPower = 2f;
    float scaleValue = 0.1f;
    public float attackBeforeDuration;

    bool isShoot;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(attackBeforeDuration);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;


            yield return null;
        }
    }
}
