// using UnityEngine;
// using DG.Tweening;
//
// public class PickUpsAnimation : MonoBehaviour
// {
//     [SerializeField] private float _moveDistance = 1;
//     [SerializeField] private float _moveDuration = 1;
//     [SerializeField] private float _rotationDuration = 2.5f;
//     [SerializeField] private Ease _moveEase = Ease.InOutSine;
//     [SerializeField] private Ease _rotationEase = Ease.Linear;
//     [SerializeField] private LoopType _moveLoopType = LoopType.Yoyo;
//     [SerializeField] private LoopType _rotationLoopType = LoopType.Restart;
//     void Start()
//     {
//       this.transform.DOMoveY(this.transform.position.y + _moveDistance, _moveDuration).From(this.transform.position.y).SetEase(_moveEase).SetLoops(-1, _moveLoopType);
//       this.transform.DOLocalRotate(new Vector3(this.transform.rotation.eulerAngles.x    , this.transform.rotation.eulerAngles.y + 360, this.transform.rotation.eulerAngles.z), _rotationDuration, RotateMode.FastBeyond360).From(this.transform.rotation.eulerAngles).SetEase(_rotationEase).SetLoops(-1, _rotationLoopType);
//         
//     }
//
//     // Update is called once per frame
//   
// }
