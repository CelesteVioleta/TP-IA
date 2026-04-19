using UnityEngine;

public class EnemyDecisionTree : MonoBehaviour
{
    private DecisionNode rootNode;

    private void Awake()
    {
        ActionNode patrolNode = new ActionNode(enemy => enemy.Patrol());
        ActionNode chaseNode = new ActionNode(enemy => enemy.Chase());

        rootNode = new QuestionNode(
            context => 
                context.los.CheckRange(context.self, context.player) 
                && context.los.CheckAngle(context.self, context.player) 
                && !context.los.CheckObstacles(context.self, context.player),
            chaseNode,
            patrolNode
        );
    }

    public void Evaluate(EnemyController3 enemy, EnemyContext context)
    {
        rootNode.Evaluate(enemy, context);
    }
}
