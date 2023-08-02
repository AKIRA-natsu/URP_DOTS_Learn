using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;

namespace AKIRA.Behaviour.Camera {
    // public partial struct CameraSystem : ISystem {
    //     [BurstCompile]
    //     public void OnCreate(ref SystemState state) { }

    //     [BurstCompile]
    //     public void OnUpdate(ref SystemState state) { }
    // }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct CameraFollowSystem : ISystem {
        // 跟随实体
        private Entity target;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CameraFollowComponent>();
        }

        public void OnUpdate(ref SystemState state) {
            // 获得CameraFollowComponent组件实体
            if (target == Entity.Null) {
                var query = SystemAPI.QueryBuilder().WithAll<CameraFollowComponent>().Build();
                var targets = query.ToEntityArray(Allocator.Temp);
                if (targets.Length == 0)
                    return;
                target = targets[0];
            }

            var camera = CameraExtend.Transform;
            var targetTrans = SystemAPI.GetComponent<LocalToWorld>(target);
            var followComponent = SystemAPI.GetComponent<CameraFollowComponent>(target);

            // 判断是否跟随
            if (followComponent.follow)
                camera.position = 
                    Vector3.Lerp(camera.position, targetTrans.Position + followComponent.offset, SystemAPI.Time.DeltaTime * followComponent.lerpSpeed);

        
            // 判断是否看着
            if (followComponent.lookAt)
                camera.LookAt(targetTrans.Position);
        }
    }
}