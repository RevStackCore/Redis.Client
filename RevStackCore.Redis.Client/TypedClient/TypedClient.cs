using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StackExchange.Redis;
using RevStackCore.Pattern;
using RevStackCore.Serialization;
using RevStackCore.Extensions;

namespace RevStackCore.Redis.Client
{
	/// <summary>
	/// POCO typed client for StackExchange.Redis.
	/// </summary>
	public class TypedClient<TEntity, TKey> where TEntity : class, IEntity<TKey>
	{
		private readonly IDatabase _db;
		private readonly string _type;
		private Type _tKeyType;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:RevStackCore.Redis.TypedClient`2"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		public TypedClient(RedisDbContext context)
		{
			_db = context.Database();
			_type = typeof(TEntity).Name;
			_tKeyType = typeof(TKey);
		}

		/// <summary>
		/// Gets all the entities of Type T.
		/// </summary>
		/// <returns>All entities of type T</returns>
		public IEnumerable<TEntity> GetAll()
		{
			return getAllValues();
		}

		public TEntity GetById(TKey id)
		{
			var key = GetTypedURNKey(id);
			if (!_db.KeyExists(key)) return default(TEntity);
			var value = _db.StringGet(key);
			return Json.DeserializeObject<TEntity>(value);
		}

        public TEntity GetById(TKey id, bool fromCache)
        {
            if(!fromCache)
            {
                return GetById(id);
            }
            string key = Convert.ToString(id);
            var value = _db.StringGet(key);
            return Json.DeserializeObject<TEntity>(value);
        }

        /// <summary>
        /// Insert the specified entity.
        /// </summary>
        /// <returns>The new entity</returns>
        /// <param name="entity">Entity.</param>
        public TEntity Insert(TEntity entity)
		{
			//check for null reference type
			if (entity == default(TEntity))
				return default(TEntity);
			//check if entity has Id assigned
			// if not, if type string, guid, assign new Guid string
			// if type int,long assign incremented value of current length.ToString() of the typed set
			if (!entity.HasPropertyValue<TEntity,TKey>())
			{
				entity = assignEntityId(entity);
				if (entity == null)
				{
					throw new Exception("Entity requires an assigned Id value for nonstandard Id value type");
				}
			}
			AddKeyToTypedSet(entity.Id);
			StoreEntity(entity);
			return entity;
		}

        /// <summary>
        /// Insert the specified entity with expiration. No internal bookkeeping kept for cached entries
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="entity">Entity.</param>
        public TEntity Insert(TEntity entity, TimeSpan expiry)
        {
            //check for null reference type
            if (entity == default(TEntity))
                return default(TEntity);
            //check if entity has Id assigned
            // if not, if type string, guid, assign new Guid string
            // if type int,long assign incremented value of current length.ToString() of the typed set
            if (!entity.HasPropertyValue<TEntity, TKey>())
            {
                entity = assignEntityId(entity);
                if (entity == null)
                {
                    throw new Exception("Entity requires an assigned Id value for nonstandard Id value type");
                }
            }

            StoreEntity(entity,expiry);
            return entity;
        }

        /// <summary>
        /// Store/Update the specified entity.
        /// </summary>
        /// <returns>The stored entity</returns>
        /// <param name="entity">Entity.</param>
        public TEntity Store(TEntity entity)
		{
			//check for null reference type
			if (entity == default(TEntity))
				return default(TEntity);
			//check if entity has Id assigned, throw error if no assignment
			if (!entity.HasPropertyValue<TEntity, TKey>())
			{
				throw new Exception("New entity requires an assigned Id value");
			}
			StoreEntity(entity);
			return entity;
		}

		/// <summary>
		/// Delete the specified entity.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="entity">Entity.</param>
		public void Delete(TEntity entity)
		{
			if (entity == default(TEntity))
				return;
			//delete from set
			RemoveKeyFromTypedSet(entity.Id);
			//delete from urn
			RemoveKeyFromUrn(entity.Id);
		}

        /// <summary>
        /// Delete the specified entity from Cache.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="fromCache">If set to <c>true</c> from cache.</param>
        public void Delete(TEntity entity, bool fromCache)
        {
            if (entity == default(TEntity))
                return;
            if(!fromCache)
            {
                Delete(entity);
            }
            string key = Convert.ToString(entity.Id);
            _db.KeyDelete(key);
        }

        /// <summary>
        /// Adds the key to typed set.
        /// </summary>
        /// <param name="key">Key.</param>
        public void AddKeyToTypedSet(TKey key)
		{
			string id = key.ToString();
			string setKey = GetSetKey();
			_db.SetAdd(setKey, id);
		}

		/// <summary>
		/// Gets the length of the typed set.
		/// </summary>
		/// <returns>The typed set length.</returns>
		public long GetTypedSetLength()
		{
			string setKey = GetSetKey();
			var length = _db.SetLength(setKey);
			return length;
		}

		/// <summary>
		/// Gets the incremented key value.
		/// </summary>
		/// <returns>The incremented key value.</returns>
		public long GetIncrementedKeyValue()
		{
			var length = GetTypedSetLength();
			length += 1;
			return length;
		}

		/// <summary>
		/// Gets the set keys for the entity type.
		/// </summary>
		/// <returns>The set keys.</returns>
		public IEnumerable<string> GetSetKeys()
		{
			string setKey = GetSetKey();
			var members = _db.SetMembers(setKey);
			return members.Select(x => x.ToString());
		}

		/// <summary>
		/// Gets the set urn keys for the entity type.
		/// </summary>
		/// <returns>The set urn keys.</returns>
		public IEnumerable<string> GetSetUrnKeys()
		{
			string setKey = GetSetKey();
			var members = _db.SetMembers(setKey);
			return members.Select(x => x.ToString().ToTypedUrnKey(_type));
		}

		/// <summary>
		/// Stores the entity.
		/// </summary>
		/// <param name="entity">Entity.</param>
		public void StoreEntity(TEntity entity)
		{
			string key = Convert.ToString(entity.Id).ToTypedUrnKey(_type);
			string json = Json.SerializeObject(entity);
			_db.StringSet(key, json);
		}
        /// <summary>
        /// Stores the entity with expiration
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="expiry">Expiry.</param>
        public void StoreEntity(TEntity entity, TimeSpan expiry)
        {
            string key = Convert.ToString(entity.Id);
            string json = Json.SerializeObject(entity);
            _db.StringSet(key, json, expiry);
        }

        /// <summary>
        /// Removes the key from urn.
        /// </summary>
        /// <param name="key">Key.</param>
        public void RemoveKeyFromUrn(TKey key)
		{
			var id = GetTypedURNKey(key);
			_db.KeyDelete(id);
		}

		/// <summary>
		/// Removes the key from typed set.
		/// </summary>
		/// <param name="key">Key.</param>
		public void RemoveKeyFromTypedSet(TKey key)
		{
			string id = key.ToString();
			string setKey = GetSetKey();
			_db.SetRemove(setKey, id);
		}

		/// <summary>
		/// Gets the set key.
		/// </summary>
		/// <returns>The set key.</returns>
		public string GetSetKey()
		{
			return "ids:" + _type;
		}

		/// <summary>
		/// Gets the typed urn formatted Key.
		/// </summary>
		/// <returns>The typed urn formatted key</returns>
		/// <param name="key">Key.</param>
		public string GetTypedURNKey(TKey key)
		{
			return "urn:" + _type.ToLower() + ":" + Convert.ToString(key);
		}


		#region "Private"

		/// <summary>
		/// Gets all values.
		/// </summary>
		/// <returns>The all values.</returns>
		private IEnumerable<TEntity> getAllValues()
		{
			var keys = GetSetUrnKeys();
			if (keys == null || keys.Count() < 1) return new List<TEntity>();
			var redisKeys = keys.Select(key => (RedisKey)key).ToArray();
			var resultBytesArray = _db.StringGet(redisKeys);
			var results = new List<TEntity>();
			foreach (var resultBytes in resultBytesArray)
			{
				var result = Json.DeserializeObject<TEntity>(resultBytes);
				results.Add(result);
			}
			return results;
		}

		/// <summary>
		/// Assigns the entity identifier.
		/// </summary>
		/// <returns>The entity identifier.</returns>
		/// <param name="entity">Entity.</param>
		private TEntity assignEntityId(TEntity entity)
		{
			long length;
			Type type = entity.GetType();
			var info = type.GetProperty("Id");
			if (_tKeyType == typeof(int))
			{
				length = GetIncrementedKeyValue();
				int intLength = Convert.ToInt32(length);
				info.SetValue(entity, length);
				return entity;
			}
			else if (_tKeyType == typeof(long))
			{
				length = GetIncrementedKeyValue();
				info.SetValue(entity, length);
				return entity;
			}
			else if (_tKeyType == typeof(Guid))
			{
				info.SetValue(entity, Guid.NewGuid());
				return entity;
			}
			else if (_tKeyType == typeof(String))
			{
				info.SetValue(entity, Guid.NewGuid().ToString());
				return entity;
			}
			else
			{
				return null;
			}
		}


		#endregion


	}
}
