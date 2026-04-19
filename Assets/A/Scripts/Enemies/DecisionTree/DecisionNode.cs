using System;
using UnityEngine;

public abstract class DecisionNode
{
    public abstract void Evaluate(EnemyController3 enemy, EnemyContext context);
}

public class QuestionNode : DecisionNode
{
    private Func<EnemyContext, bool> question;

    private DecisionNode trueNode;
    private DecisionNode falseNode;

    public QuestionNode(Func<EnemyContext, bool> question, DecisionNode trueNode, DecisionNode falseNode)
    {
        this.question = question;
        this.trueNode = trueNode;
        this.falseNode = falseNode;
    }

    public override void Evaluate(EnemyController3 enemy, EnemyContext context)
    {
        if (question(context))
        {
            trueNode.Evaluate(enemy, context);
        }
        else
        {
            falseNode.Evaluate(enemy, context);
        }

    }
}

public class ActionNode : DecisionNode
{
    private Action<EnemyController3> action;

    public ActionNode(Action<EnemyController3> action)
    {
        this.action = action;
    }

    public override void Evaluate(EnemyController3 enemy, EnemyContext context)
    {
        action(enemy);
    }
}