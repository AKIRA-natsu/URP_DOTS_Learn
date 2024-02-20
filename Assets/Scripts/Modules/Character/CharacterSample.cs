using System;
using System.Linq;
using System.Threading.Tasks;
using AKIRA.Attribute;
using AKIRA.Manager;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AKIRA.Behaviour.AI {
    public class CharacterSample : EntityBase {
        private PlayerInput input;
        private IKBehaviour ikBehaviour;
        public Collider[] colliders;

        private Transform lookPoint;
        private Vector3 moveDir;


        private static int SpeedHash = Animator.StringToHash("Speed");

        private void Awake() {
            input = this.GetComponent<PlayerInput>();
            ikBehaviour = this.GetComponentInChildren<IKBehaviour>();
            lookPoint = this.transform.Find("[LookPoint]");
        }

        private void Start() {
            // add component data
            this.AddComponentData(new HeadIKComponent(ikBehaviour));
            this.AddComponentData(new HandIKComponent(ikBehaviour));

            // set camera follow
            InitFreeCamera();

            // Curor
            Cursor.visible = false;
        }

        public override void OnUpdate() {
            Move();
        }

        private void InitFreeCamera() {
            // 主摄像机参数设置
            var camera = CameraExtend.GetCamera(GameData.Camera.Main).GetComponent<CinemachineFreeLook>();
            camera.m_YAxis.m_InputAxisName = "Mouse ScrollWheel";
            camera.LookAt = lookPoint;
            camera.Follow = lookPoint;
        }

        private void Move() {
            var inputPosition = input.actions["Move"].ReadValue<Vector2>();
            // 相对摄像机的前后左右
            var dir = Quaternion.Euler(0, CameraExtend.Transform.localEulerAngles.y, 0) * new Vector3(inputPosition.x, 0, inputPosition.y);
            if (dir.sqrMagnitude <= Mathf.Epsilon)
                moveDir = Vector3.zero;
            else
                moveDir = Vector3.Lerp(moveDir, dir, Time.deltaTime * 5f);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(moveDir.Equals(Vector3.zero) ? this.transform.forward : moveDir), Time.deltaTime * 10f);
            
            var isRunning = input.actions["Run"].IsPressed();
            var curSpeedValue = ikBehaviour.Animator.GetFloat(SpeedHash);
            var targetSpeedValue = 0f;
            // 如果转向小于50才移动
            if (Vector3.Angle(this.transform.forward, moveDir.normalized) <= 50f) {
                this.transform.Translate(2f * (isRunning ? 1.5f : 1f) * Time.deltaTime * moveDir, Space.World);
                targetSpeedValue = moveDir.Equals(Vector3.zero) ? 0 : isRunning ? 1f : .5f;
            }
            ikBehaviour.Animator.SetFloat(SpeedHash, Mathf.Lerp(curSpeedValue, targetSpeedValue, Time.deltaTime * 5f));
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireSphere(this.transform.position, 2f);
        }
    }

    [SystemLauncher]
    public class HeadIKSystem : Singleton<HeadIKSystem>, IUpdate
    {
        protected HeadIKSystem() { }

        public override async Task Initialize()
        {
            await Task.Yield();
            this.Regist();
        }

        public void OnUpdate()
        {
            foreach (var (entity, component) in World.QueryWithEntity<HeadIKComponent>()) {
                var colliders = Physics.OverlapSphere(entity.transform.position, 2f, 1 << Layer.Default);
                component.SetLookTarget(colliders?.FirstOrDefault()?.transform ?? null);
            }
        }
    }
}