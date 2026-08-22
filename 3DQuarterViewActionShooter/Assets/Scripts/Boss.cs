using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;
    public float predictionRange;   // 플레이어 이동 예측 범위
    public float thinkDuration;     // 다음 패턴 결정까지 걸리는 시간
    public float shotDuration;
    public float bigShotDuration;
    public float tauntDuration;

    Vector3 lookVec;
    Vector3 tauntVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        navAgent.isStopped = true;
        StartCoroutine(Think());
    }

    void Start()
    {
        isLook = true;
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(horizontal, 0f, vertical) * predictionRange;
            transform.LookAt(targetTransform.position + lookVec);
        }
        else
        {
            navAgent.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(thinkDuration);

        int randomAction = Random.Range(0, 5);

        switch (randomAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        animator.SetTrigger("doShot");

        // 첫번째 미사일 0.2초 뒤 발사
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.targetTransform = targetTransform;

        // 두번째 미사일 0.2 + 0.3초 뒤 발사
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.targetTransform = targetTransform;

        yield return new WaitForSeconds(shotDuration);

        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        isLook = false;
        animator.SetTrigger("doBigShot");

        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(bigShotDuration);

        isLook = true;

        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = targetTransform.position + lookVec;

        isLook = false;
        boxCollider.enabled = false;
        navAgent.isStopped = false;
        animator.SetTrigger("doTaunt");

        yield return new WaitForSeconds(attackBeforeDuration);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(attackDuration);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(tauntDuration);
        isLook = true;
        boxCollider.enabled = true;
        navAgent.isStopped = true;

        StartCoroutine(Think());
    }
}
