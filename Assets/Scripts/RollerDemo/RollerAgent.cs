//RollerAgent.cs
//继承Agent用于重写智能体的AgentReset，CollectObservations，AgentAction等方法。
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
public class RollerAgent : Agent
{
    public Transform Target;
    public float speed = 10;
    Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    //收集观察结果
    public override void CollectObservations(VectorSensor sensor)
    {
        //观察目标球和智能体的位置
        sensor.AddObservation(Target.position);
        sensor.AddObservation(this.transform.position);
        //观察智能体的速度
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        //在这里因为目标球是不会动的，智能体也不会在y轴上又运动，所以没有必要观察这些值的变化。
        //sensor.AddObservation(rBody.velocity.y);
    }

    //处理动作，并根据当前动作评估奖励信号值    
    public override void AgentAction(float[] vectorAction)
    {
        //------ 动作处理
        // 接受两个动作数值
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        //------ 奖励信号

        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);
        // 到达目标球
        if (distanceToTarget < 1.42f)
        {
            //奖励值+1.0f
            SetReward(1.0f);
            Done();
        }

        // 掉落场景外
        if (this.transform.position.y < 0)
        {
            Done();
        }
    }

    //Reset时调用
    public override void AgentReset()
    {
        if (this.transform.position.y < 0)
        {
            //如果智能体掉下去，则重置位置+重置速度
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }
        //将目标球重生至一个新的随机位置
        Target.position = new Vector3(UnityEngine.Random.value * 8 - 4, 0.5f, UnityEngine.Random.value * 8 - 4);
    }

    public override float[] Heuristic()
    {
        var action = new float[2];

        action[0] = -Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}