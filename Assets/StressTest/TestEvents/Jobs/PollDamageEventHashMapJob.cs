using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct SinglePollDamageEventHashMapJob : IJob
{
    [ReadOnly]
    public EntityTypeHandle EntityType;
    [ReadOnly]
    public NativeMultiHashMap<Entity, DamageEvent> DamageEventsMap;
    [NativeDisableParallelForRestriction]
    public ComponentDataFromEntity<Health> HealthFromEntity;

    public void Execute()
    {
        var enumerator = DamageEventsMap.GetEnumerator();
        while(enumerator.MoveNext())
        {
            Entity targetEntity = enumerator.Current.Key;
            DamageEvent damageEvent = enumerator.Current.Value;
            if (HealthFromEntity.HasComponent(targetEntity))
            {
                Health health = HealthFromEntity[targetEntity];
                health.Value -= damageEvent.Value;
                HealthFromEntity[targetEntity] = health;
            }
        }
    }
}

[BurstCompile]
public struct ClearDamageEventHashMapJob : IJob
{
    public NativeMultiHashMap<Entity, DamageEvent> DamageEventsMap;

    public void Execute()
    {
        DamageEventsMap.Clear();
    }
}