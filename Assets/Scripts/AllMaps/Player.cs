using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;

namespace AllMaps
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform tf;
        [SerializeField] private SkeletonAnimation anim;
        private string[] idleAnims = {"Idle_0","Idle_1","Idle_10","Idle_11"};
        private string[] moveAnims = { "Mov0", "Mov1", "Mov_10","Mov_11" };
        [SerializeField] private float moveSpeed = 2;
        public bool IsMoving { get; private set; }
        public void Init(Gender gender)
        {
            var skeletion = anim.Skeleton;
            switch (gender)
            {
                case Gender.Male:
                    skeletion.SetSkin("0");
                    break;
                case Gender.Female:
                    skeletion.SetSkin("10");
                    break;
            }
        }
        public void SetPosition(Vector2 position)
        {
            tf.position = position;
        }

        private void SetMoving(bool isMoving)
        {
            IsMoving = isMoving;
            var state = anim.AnimationState;
            if (isMoving)
            {
                state.SetAnimation(0, moveAnims[0], true);
            }
            else
            {
                state.SetAnimation(0, idleAnims[0], true);
            }
        }
        public void DoMove(List<Node> nodes,Action completed)
        {
            SetMoving(true);
            StartCoroutine(IEDoMove(nodes,completed));
        }
        
        private IEnumerator IEDoMove(List<Node> nodes,Action completed)
        {
            foreach (var node in nodes)
            {
                var nodePositions = node.MoveNode.NodesPosition;
                var nextMapId = node.NextMap.Id;
                var finishMapId = node.MoveNode.FinishNode;
                if (nextMapId != finishMapId)
                {
                    nodePositions = nodePositions.Reverse().ToArray();
                }
                foreach (var nodePosition in nodePositions)
                {
                    yield return MoveTo(nodePosition);
                }
                var mapPos = node.NextMap.transform.position;
                yield return MoveTo(mapPos);
            }
            SetMoving(false);
            completed?.Invoke();
            IEnumerator MoveTo(Vector2 target)
            {
                var position = tf.position;
                var isLeft = (target.x - position.x) < 0;
                var y = isLeft ? 180 : 0;
                tf.eulerAngles = new Vector3(0, y, 0);
                while (Vector2.Distance(position, target) > 0.05f)
                {
                    position= Vector2.MoveTowards(position, target, moveSpeed * Time.deltaTime);
                    tf.position = position;
                    yield return  null;
                }
            }
        }
    }
}
