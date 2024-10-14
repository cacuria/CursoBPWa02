using Microsoft.Extensions.Caching.Memory;

namespace Bdb.Curso.HttpApi.Host.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }


        /// <summary>
        /// Establece un valor en el caché con una expiración relativa.
        /// </summary>
        /// <typeparam name="T">Tipo de datos a almacenar en caché.</typeparam>
        /// <param name="key">Clave del caché.</param>
        /// <param name="value">Valor a almacenar.</param>
        /// <param name="expiration">Tiempo de expiración relativo al caché.</param>
        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                SlidingExpiration = TimeSpan.FromMinutes(5) // Opcional: ajusta la expiración deslizante si lo deseas
            };

            _cache.Set(key, value, cacheEntryOptions);
        }


        /// <summary>
        /// Obtiene un valor del caché.
        /// </summary>
        /// <typeparam name="T">Tipo de datos almacenados en caché.</typeparam>
        /// <param name="key">Clave del caché.</param>
        /// <returns>Valor almacenado en caché o default(T) si no existe.</returns>
        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }


        /// <summary>
        /// Elimina una entrada específica del caché.
        /// </summary>
        /// <param name="key">Clave del caché a eliminar.</param>
        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Elimina todas las entradas del caché.
        /// </summary>
        public void ClearAllCaches()
        {
            // Nota: IMemoryCache no proporciona una forma directa de eliminar todas las entradas
            // Sin embargo, puedes gestionar esto eliminando todas las claves que conoces, si es necesario.
            // Ejemplo para eliminar entradas específicas:
                        
            _cache.Remove("GetAllData");
            _cache.Remove("anotherData");
            // Agrega más claves según las necesidades
        }

    }



}
