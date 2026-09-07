using Base;
using LongDH.LongDHModules;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using CreateCharacter;
using LongDH.Script.ObjectPool;
using LongDHModules;
using UnityEngine;
using Network;
using UnityEngine.UI;

namespace AllMaps
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform tf;
        [SerializeField] private Transform mainTf;
        [SerializeField] private SkeletonAnimation anim;
        [SerializeField] private Text nameText;
        private string genderStr = "";
        [SerializeField] private float speed = 2;
        public bool IsSwordRiding { get; private set; }
        public bool IsMoving { get; private set; }
        public NewCharacterGraphic newVisual;

        public void SetInstance()
        {
            Pool.Set(this);
        }

        public void SetName(string name, Color color)
        {
            nameText.text = name;
            nameText.color = color;
        }

        public void Init(Gender gender, bool isSwordRiding, Appearance appearance)
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
            EVehicleType vehicleType = isSwordRiding ? EVehicleType.Sword : EVehicleType.Base;

            newVisual.InitVisual(vehicleType, gender, appearance.Skins, appearance.Colors);
        }

        public void SetPosition(Vector2 position)
        {
            mainTf.position = position;
        }

        private void SetMoving(bool isMoving)
        {
            IsMoving = isMoving;
            var swordRiding = IsSwordRiding ? "1" : "0";
            var moving = isMoving ? "Mov_1" : "Idle_";
            var animationName = $"{moving}{swordRiding}";
            anim.AnimationState.SetAnimation(0, animationName, true);
        }

        public void DoMove(List<Node> nodes, Action completed)
        {
            SetMoving(true);
            StartCoroutine(IEDoMove(nodes, completed));
        }

        private IEnumerator IEDoMove(List<Node> nodes, Action completed)
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
                var position = mainTf.position;
                var isLeft = (target.x - position.x) < 0;
                var y = isLeft ? 180 : 0;
                tf.eulerAngles = new Vector3(0, y, 0);
                while (Vector2.Distance(position, target) > 0.05f)
                {
                    position = Vector2.MoveTowards(position, target, speed * Time.deltaTime);
                    mainTf.position = position;
                    yield return null;
                }
            }
        }

        public void ReturnPool()
        {
            mainTf.gameObject.SetActive(false);
            ObjectPool.Recycle(mainTf.gameObject);
        }

        public void SetActive(bool active)
        {
            mainTf.gameObject.SetActive(active);
        }
    }
}