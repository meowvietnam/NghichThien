using Base;
using LongDH.LongDHModules;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using LongDHModules;
using UnityEngine;

namespace AllMaps
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform tf;
        [SerializeField] private SkeletonAnimation anim;
        private string genderStr = "";
        [SerializeField] private float speed = 2;
        public bool IsSwordRiding { get; private set; }
        public bool IsMoving { get; private set; }


        public void Init(Gender gender, bool isSwordRiding)
        {
            this.IsSwordRiding = isSwordRiding;
            var skeletion = anim.Skeleton;
            switch (gender)
            {
                case Gender.Male:
                    genderStr = "0";
                    break;
                case Gender.Female:
                    genderStr = "10";
                    break;
            }
            anim.Skeleton.SetSkin(genderStr);
            anim.timeScale = speed;
            SetMoving(false);
            Pool.Set(this);

            // test
        }
          
        public void SetPosition(Vector2 position)
        {
            tf.position = position;
        }

        private void SetMoving(bool isMoving)
        {
            IsMoving = isMoving;
            var swordRiding = IsSwordRiding ? "1" : "0";
            var moving = isMoving ? "Mov_1" : "Idle_";
            var animationName = $"{moving}{swordRiding}";
            anim.AnimationState.SetAnimation(0, animationName, true);
        }
        public void DoMove(List<Node> nodes,Action completed)
        {
            SetMoving(true);
            StartCoroutine(IEDoMove(nodes,completed));
        }
        
        private IEnumerator IEDoMove(List<Node> nodes,Action completed)
        {
            if (IsSwordRiding)
            {
                var lastNode = nodes.Last();
                var target = lastNode.NextMap.PlayerPosition;
                yield return MoveTo(target);
            }
            else
            {
                foreach (var node in nodes)
                {
                    var moveNode = node.MoveNode;
                    if (moveNode != null)
                    {
                        var nodePositions = moveNode.NodesPosition;
                        var nextMapId = node.NextMap.Id;
                        var finishMapId = moveNode.FinishNode;
                        if (nextMapId != finishMapId)
                        {
                            nodePositions = nodePositions.Reverse().ToArray();
                        }
                        foreach (var nodePosition in nodePositions)
                        {
                            yield return MoveTo(nodePosition);
                        }
                    }
                    var mapPos = node.NextMap.PlayerPosition;
                    yield return MoveTo(mapPos);
                }
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
                    position= Vector2.MoveTowards(position, target, speed * Time.deltaTime);
                    tf.position = position;
                    yield return  null;
                }
            }
        }
    }
}
