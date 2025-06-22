// namespace RedGaint
// {
//     public class TargetLockCheckNode : BTNode
//     {
//         private BotController bot;
//
//         public
//             TargetLockCheckNode(BotController bot)
//         {
//             this.bot = bot;
//         }
//
//         protected override BTStatus ExecuteNode()
//         {
//             if (bot.detectedPlayer == null) return BTStatus.Failure;
//
//             var targetController = bot.detectedPlayer.GetComponent<BaseCharacterController>();
//             return targetController.TryTargetLock(bot) ? BTStatus.Success : BTStatus.Failure;
//         }
//     }
// }//RedGaint