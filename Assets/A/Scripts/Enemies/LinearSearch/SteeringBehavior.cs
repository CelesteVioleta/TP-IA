using UnityEngine;
using UnityEngine.Rendering;

public static class SteeringBehavior
{
    public static Vector3 Seek(Transform self, Vector3 target)
    {
        Vector3 dir = target - self.position;
        dir.y = 0;

        return dir.normalized;
    }

    public static Vector3 Flee(Transform self, Vector3 target)
    {
        Vector3 dir = self.position - target;
        dir.y = 0;

        return dir.normalized;  
    }

    public static Vector3 Arrive(Transform self, Vector3 target, float slowRadius)
    {
        Vector3 dir = target - self.position;
        
        float distance = dir.magnitude;
        if(distance < 0.01f)
        {
            return Vector3.zero;
        }

        float speedFactor = Mathf.Clamp01(distance / slowRadius);

        return dir.normalized * speedFactor;
    }

    public static Vector3 CalculateFuturePos(Transform self, Transform target, Rigidbody targetRb, float maxPredictionTime)
    {
        Vector3 targetVelocity = Vector3.zero;
        targetVelocity = targetRb.linearVelocity;

        Vector3 toTarget = target.position - self.position;
        toTarget.y = 0;

        float distance = toTarget.magnitude;
        float predictionTime = Mathf.Clamp(distance / 5f, 0f, maxPredictionTime);

        return target.position + targetVelocity * predictionTime;
    }

    public static Vector3 Chase(Transform self, Transform target, Rigidbody targetRb, float maxPredictionTime)
    {
        Vector3 futurePos = CalculateFuturePos(self, target, targetRb, maxPredictionTime);

        return Seek(self, futurePos);
    }

    public static Vector3 Evade(Transform self, Transform target, Rigidbody targetRb, float maxPredictionTime)
    {
        Vector3 futurePos = CalculateFuturePos(self, target, targetRb, maxPredictionTime);

        return Flee(self, futurePos);
    }

    public static Vector3 Wander(Vector3 currentDirection, float maxAngleChange)
    {
        float randomAngle = UnityEngine.Random.Range(-maxAngleChange, maxAngleChange);
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);

        Vector3 newDirection = rotation * currentDirection;
        newDirection.y = 0f;

        return newDirection.normalized;    
    }
}
