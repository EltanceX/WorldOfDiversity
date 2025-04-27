using Engine;
using Game;

namespace GlassMod
{
    public class TorcherinoParticle : FireParticleSystem
    {
        public TorcherinoParticle(Vector3 position, float size, float maxVisibilityDistance) : base(position, size, maxVisibilityDistance)
        {

        }
        public override bool Simulate(float dt)
        {
            m_age += dt;
            bool flag = false;
            if (m_visible || m_age < 2f)
            {
                m_toGenerate += IsStopped ? 0f : (5f * dt);
                for (int i = 0; i < Particles.Length; i++)
                {
                    Particle particle = Particles[i];
                    if (particle.IsActive)
                    {
                        flag = true;
                        particle.Time += dt;
                        particle.TimeToLive -= dt;
                        if (particle.TimeToLive > 0f)
                        {
                            particle.Position.Y += particle.Speed * dt;
                            particle.TextureSlot = (int)MathUtils.Min(9f * particle.Time / 1.25f, 8f);
                        }
                        else
                        {
                            particle.IsActive = false;
                        }
                    }
                    else if (m_toGenerate >= 1f)
                    {
                        particle.IsActive = true;
                        particle.Position = m_position + (0.25f * m_size * new Vector3(m_random.Float(-1f, 1f), 0f, m_random.Float(-1f, 1f)));
                        particle.Color = new Color(255, 139, 38, 255);
                        particle.Size = new Vector2(m_size);
                        particle.Speed = m_random.Float(0.45f, 0.55f) * m_size / 0.15f;
                        particle.Time = 0f;
                        particle.TimeToLive = m_random.Float(0.5f, 2f);
                        particle.FlipX = m_random.Int(0, 1) == 0;
                        particle.FlipY = m_random.Int(0, 1) == 0;
                        m_toGenerate -= 1f;
                    }
                }
                m_toGenerate = MathUtils.Remainder(m_toGenerate, 1f);
            }
            m_visible = false;
            if (IsStopped)
            {
                return !flag;
            }
            return false;
        }
    }
}
