using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Caching.Repository.Extensions;
public static class DistributedCacheExtensions
{
	public static async Task SetRecordAsync<T>(this IDistributedCache cache,
		string recordId,
		T data,
		TimeSpan? absoluteExpireTime = null,
		TimeSpan? unusedExpireTime = null)
	{
		var ops = new DistributedCacheEntryOptions();

		ops.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
		ops.SlidingExpiration = unusedExpireTime;

		var jsonData = JsonSerializer.Serialize(data);

		await cache.SetStringAsync(recordId, jsonData, ops);
	}

	public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
	{
		var jsonData = await cache.GetStringAsync(recordId);

		if (jsonData is null)
			return default(T);

		return JsonSerializer.Deserialize<T>(jsonData);
	}
}