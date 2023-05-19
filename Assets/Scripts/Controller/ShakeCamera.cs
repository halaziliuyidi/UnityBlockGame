using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private Transform cameraTrn;
    [SerializeField]
    private Vector3 positionStrength;
    [SerializeField]
    private Vector3 rotationStrength;

    private void Awake()
    {
        cameraTrn = transform;
        EventManager.AddListener(GameConstManager.comboUp2,CameraShake);
    }

    private void CameraShake(object[] args)
    {
        cameraTrn.DOComplete();
        cameraTrn.DOShakePosition(0.3f,positionStrength);
        cameraTrn.DOShakeRotation(0.3f,rotationStrength);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(GameConstManager.comboUp2, CameraShake);
    }
}