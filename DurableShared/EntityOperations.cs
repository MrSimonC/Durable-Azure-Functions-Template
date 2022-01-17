using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DurableShared;

public class EntityOperations
{
    public async Task<EntityStateResponse<T>> GetEntityAsync<T>(IDurableEntityClient client, string uniqueKey) where T : class
    {
        var entityId = GetEntityId<T>(uniqueKey);
        return await client.ReadEntityStateAsync<T>(entityId);
    }

    public static EntityId GetEntityId<T>(string uniqueKey) where T : class
    {
        return new EntityId(nameof(T), uniqueKey);
    }
}
