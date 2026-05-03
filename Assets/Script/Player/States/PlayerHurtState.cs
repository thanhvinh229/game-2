using UnityEngine;

public class PlayerHurtState : PlayerState
{
    private float timer;

    public PlayerHurtState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Bắt đầu đếm thời gian bị choáng
        timer = player.hurtDuration;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        // Dùng Lerp để giảm dần vận tốc đẩy lùi về 0 (tạo độ ma sát)
        player.velocity.x = Mathf.Lerp(player.velocity.x, 0, Time.deltaTime * 5f);
        player.velocity.z = Mathf.Lerp(player.velocity.z, 0, Time.deltaTime * 5f);

        // Hết thời gian choáng -> Quay lại trạng thái đứng im để có thể di chuyển tiếp
        if (timer <= 0)
        {
            player.ChangeState(player.idleState);
        }
    }
}
