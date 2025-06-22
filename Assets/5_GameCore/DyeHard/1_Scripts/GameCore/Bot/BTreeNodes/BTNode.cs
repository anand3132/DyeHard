using System.Collections.Generic;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    #region BaseClasses
    public enum BTLogLevel { None, Basic, Verbose }

    /// <summary>
    /// Abstract base class for all Behavior Tree nodes.
    /// Defines the structure for execution, logging, and optional control operations.
    /// </summary>
    public abstract class BTNode
    {
        // Toggle from anywhere
        public static BTLogLevel LogLevel = BTLogLevel.None; 

        // Optional name for debugging purposes
        public string nodeName;
        private static int indentLevel = 0;
        /// <summary>
        /// Called to execute the node.
        /// Internally calls the overridden ExecuteNode method and logs the result.
        /// </summary>
        /// <returns>Status of execution (Running, Success, or Failure)</returns>
        public virtual BTStatus Execute()
        {
            indentLevel++;
            BTStatus result = ExecuteNode();

            if (LogLevel != BTLogLevel.None)
            {
                string prefix = new string('─', indentLevel * 2);
                string displayName = string.IsNullOrEmpty(nodeName) ? GetType().Name : nodeName;
                string logMessage = $"{prefix} [{displayName}] → {result}";

                if (LogLevel == BTLogLevel.Basic || result != BTStatus.Running)
                {
                    switch (result)
                    {
                        case BTStatus.Success:
                            Debug.Log($"<color=green>{logMessage}</color>");
                            break;
                        case BTStatus.Failure:
                            Debug.Log($"<color=red>{logMessage}</color>");
                            break;
                        case BTStatus.Running:
                            Debug.Log($"<color=yellow>{logMessage}</color>");
                            break;
                    }
                }
            }

            indentLevel--;
            return result;
        }
        
        
        
        
        /// <summary>
        /// Core logic to be implemented by each specific node type.
        /// </summary>
        protected abstract BTStatus ExecuteNode();

        /// <summary>
        /// Optional method to abort execution. Can be overridden in derived classes.
        /// Useful for canceling long-running tasks or cleanup.
        /// </summary>
        public virtual void Abort() { }

        /// <summary>
        /// Optional method to reset internal state. Can be overridden for reusability.
        /// </summary>
        public virtual void Reset() { }
    }

    /// <summary>
    /// Abstract base class for composite nodes, which can contain multiple child nodes.
    /// Provides management and execution of its children.
    /// </summary>
    public abstract class BTComposite : BTNode
    {
        // List of child nodes under this composite
        protected List<BTNode> children = new List<BTNode>();

        // Default constructor
        protected BTComposite() { }

        // Constructor accepting an initial set of child nodes
        public BTComposite(params BTNode[] nodes)
        {
            children.AddRange(nodes);
        }

        /// <summary>
        /// Dynamically add a child node to the composite.
        /// </summary>
        public void AddChild(BTNode node)
        {
            children.Add(node);
        }
    }

    /// <summary>
    /// Abstract base class for decorator nodes, which wrap a single child node.
    /// Used to alter the behavior or result of its child.
    /// </summary>
    public abstract class BTDecorator : BTNode
    {
        // The single child node wrapped by the decorator
        protected BTNode child;

        // Constructor assigning the child node
        protected BTDecorator(BTNode child)
        {
            this.child = child;
        }
    }

    #endregion
}
