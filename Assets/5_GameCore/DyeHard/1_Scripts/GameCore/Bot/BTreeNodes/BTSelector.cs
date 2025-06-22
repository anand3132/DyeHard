using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RedGaint.Games.DyeHard
{
    // Represents the result of a Behavior Tree node execution
    public enum BTStatus
    {
        Running,
        Success,
        Failure
    }

    #region Node

    /// <summary>
    /// Executes a condition and returns Success if true, otherwise Failure.
    /// Used to represent decision branches in the tree.
    /// </summary>
    public class BTCondition : BTNode
    {
        private System.Func<bool> condition;
        private BTNode child;

        public BTCondition(System.Func<bool> condition, BTNode child)
        {
            this.condition = condition;
            this.child = child;
        }

        protected override BTStatus ExecuteNode()
        {
            if (condition())
            {
                return child.Execute();
            }
            return BTStatus.Failure;
        }
    }


    /// <summary>
    /// Waits for a given amount of time before succeeding.
    /// Used for delays or cooldowns between actions.
    /// </summary>
    public class BTWait : BTNode
    {
        private float waitTime;
        private float startTime = -1;

        public BTWait(float waitTime)
        {
            this.waitTime = waitTime;
        }

        protected override BTStatus ExecuteNode()
        {
            if (startTime < 0)
                startTime = Time.time;

            if (Time.time - startTime >= waitTime)
            {
                startTime = -1;
                return BTStatus.Success;
            }

            return BTStatus.Running;
        }
    }
    
        public class BTConditionalCounterNode : BTNode
        {
            private BTNode child;
            private int successThreshold;
            private int currentSuccessCount;

            public System.Action OnThresholdReached;

            public BTConditionalCounterNode(BTNode child, int successThreshold)
            {
                this.child = child;
                this.successThreshold = successThreshold;
            }

            protected override BTStatus ExecuteNode()
            {
                var status = child.Execute();

                if (status == BTStatus.Success)
                {
                    currentSuccessCount++;
                    Debug.Log($"[BT] {child.nodeName} success count: {currentSuccessCount}/{successThreshold}");

                    if (currentSuccessCount >= successThreshold)
                    {
                        OnThresholdReached?.Invoke();
                        return BTStatus.Success;
                    }
                }

                return status;
            }

            public override void Reset()
            {
                base.Reset();
                currentSuccessCount = 0;
                child.Reset();
            }
        }

    #endregion

    #region Composite

    /// <summary>
    /// Selector: Tries each child in order, returns Success on the first Success or Running.
    /// Returns Failure only if all children fail.
    /// </summary>
    public class BTSelector : BTComposite
    {
        public BTSelector() : base() { }
        public BTSelector(params BTNode[] nodes) : base(nodes) { }

        protected override BTStatus ExecuteNode()
        {
            foreach (var child in children)
            {
                var status = child.Execute();
                if (status != BTStatus.Failure)
                    return status;
            }
            return BTStatus.Failure;
        }
    }

    /// <summary>
    /// Sequence: Executes children in order.
    /// Returns Failure or Running immediately if a child fails or is running.
    /// Returns Success only if all children succeed.
    /// </summary>

    public class BTSequence : BTComposite
    {
        private static int unnamedCount = 0;

        public BTSequence(string name = null, params BTNode[] nodes) : base(nodes)
        {
            this.nodeName = !string.IsNullOrEmpty(name) ? name : $"BTSequence_{++unnamedCount}";
        }

        protected override BTStatus ExecuteNode()
        {
            foreach (var child in children)
            {
                var status = child.Execute();
                if (status != BTStatus.Success)
                    return status;
            }

            return BTStatus.Success;
        }
    }


    /// <summary>
    /// Parallel: Executes all children.
    /// Returns Failure if any child fails.
    /// Returns Success if at least one child succeeds and none fail.
    /// Returns Running otherwise.
    /// </summary>

    public class BTParallel : BTComposite
    {
        private static int unnamedCount = 0;

        public BTParallel(string name = null, params BTNode[] nodes) : base(nodes)
        {
            this.nodeName = !string.IsNullOrEmpty(name) ? name : $"BTParallel_{++unnamedCount}";
        }

        protected override BTStatus ExecuteNode()
        {
            bool anySuccess = false;

            foreach (var child in children)
            {
                var status = child.Execute();

                if (status == BTStatus.Success)
                    anySuccess = true;
                else if (status == BTStatus.Failure)
                    return BTStatus.Failure;
            }

            return anySuccess ? BTStatus.Success : BTStatus.Running;
        }
    }



    /// <summary>
    /// RandomSelector: Shuffles the children each tick and executes them in new random order.
    /// Returns Success or Running if any child returns so; otherwise Failure.
    /// </summary>
    public class BTRandomSelector : BTComposite
    {
        protected override BTStatus ExecuteNode()
        {
            Shuffle(children);
            foreach (var child in children)
            {
                var status = child.Execute();
                if (status != BTStatus.Failure)
                    return status;
            }
            return BTStatus.Failure;
        }

        // Fisher–Yates shuffle
        private void Shuffle(List<BTNode> btNodes)
        {
            for (int i = 0; i < btNodes.Count; i++)
            {
                int rand = Random.Range(i, btNodes.Count);
                (btNodes[i], btNodes[rand]) = (btNodes[rand], btNodes[i]);
            }
        }
    }

    /// <summary>
    /// PrioritySelector: Similar to Selector but tracks running child index to resume from.
    /// Gives priority to the first successful or running node.
    /// </summary>
    public class BTPrioritySelector : BTComposite
    {
        private int lastRunningChild = -1;

        protected override BTStatus ExecuteNode()
        {
            for (int i = 0; i < children.Count; i++)
            {
                var status = children[i].Execute();

                if (status == BTStatus.Running)
                {
                    lastRunningChild = i;
                    return BTStatus.Running;
                }

                if (status == BTStatus.Success)
                {
                    lastRunningChild = -1;
                    return BTStatus.Success;
                }
            }

            lastRunningChild = -1;
            return BTStatus.Failure;
        }
    }

    #endregion

    #region Decorator

    /// <summary>
    /// Inverter: Reverses the result of its child node.
    /// Success → Failure, Failure → Success, Running remains unchanged.
    /// </summary>
    public class BTInverter : BTDecorator
    {
        public BTInverter(BTNode child) : base(child) { }

        protected override BTStatus ExecuteNode()
        {
            var status = child.Execute();
            return status == BTStatus.Success ? BTStatus.Failure :
                   status == BTStatus.Failure ? BTStatus.Success :
                   BTStatus.Running;
        }
    }

    /// <summary>
    /// Repeater: Repeats its child node a specific number of times or infinitely (-1).
    /// </summary>
    public class BTRepeater : BTDecorator
    {
        private int repeatCount;
        private int currentCount;

        public BTRepeater(BTNode child, int repeatCount = -1) : base(child)
        {
            this.repeatCount = repeatCount;
        }

        protected override BTStatus ExecuteNode()
        {
            if (repeatCount > 0 && currentCount >= repeatCount)
                return BTStatus.Success;

            var status = child.Execute();
            if (status != BTStatus.Running)
            {
                currentCount++;
                if (repeatCount < 0 || currentCount < repeatCount)
                    return BTStatus.Running;
            }
            return status;
        }
    }

    /// <summary>
    /// UntilFail: Repeats the child until it fails, then returns Success.
    /// </summary>
    public class BTUntilFail : BTDecorator
    {
        public BTUntilFail(BTNode child) : base(child) { }

        protected override BTStatus ExecuteNode()
        {
            var status = child.Execute();
            return status == BTStatus.Failure ? BTStatus.Success : BTStatus.Running;
        }
    }

    /// <summary>
    /// RepeatUntilSuccess: Repeats the child until it succeeds.
    /// </summary>
    public class BTRepeatUntilSuccess : BTDecorator
    {
        public BTRepeatUntilSuccess(BTNode child) : base(child) { }

        protected override BTStatus ExecuteNode()
        {
            var status = child.Execute();
            return status == BTStatus.Success ? BTStatus.Success : BTStatus.Running;
        }
    }

    /// <summary>
    /// Timeout: Forces child to finish within a time limit.
    /// If it takes too long, the node fails.
    /// </summary>
    public class BTTimeout : BTNode
    {
        private readonly BTNode child;
        private readonly float timeoutDuration;
        private float startTime;
        private bool isTiming;
        private Action onTimeoutSuccess;

        public BTTimeout(BTNode child, float timeoutDuration, Action onTimeoutSuccess = null)
        {
            this.child = child;
            this.timeoutDuration = timeoutDuration;
            this.onTimeoutSuccess = onTimeoutSuccess;
        }

        protected override BTStatus ExecuteNode()
        {
            if (!isTiming)
            {
                startTime = Time.time;
                isTiming = true;
            }

            if (Time.time - startTime >= timeoutDuration)
            {
                isTiming = false;
                return BTStatus.Failure;
            }

            var status = child.Execute();

            if (status == BTStatus.Success)
            {
                isTiming = false;
                onTimeoutSuccess?.Invoke(); // 🔥 Trigger after success
                return BTStatus.Success;
            }

            return status;
        }
    }


    /// <summary>
    /// Simple key-value store used by behavior tree nodes for data sharing.
    /// Acts like a blackboard for AI logic.
    /// </summary>
    public class BTBlackboard
    {
        public Dictionary<string, object> data = new Dictionary<string, object>();

        public void Set<T>(string key, T value) => data[key] = value;
        public T Get<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
        public bool Has(string key) => data.ContainsKey(key);
    }

    #endregion
}
