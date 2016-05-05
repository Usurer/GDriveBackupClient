using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDriveClientLib.Abstractions;
using Newtonsoft.Json;

namespace GDriveBackupClient
{
    public class FileCacheManager
    {
        private readonly string Filename;

        public FileCacheManager(string filename)
        {
            Filename = filename;
            if (!File.Exists(Filename))
            {
                using (var stream = File.Create(Filename))
                {
                    stream.Close();
                }
            }
        }

        public void AddOrUpdate(INode node, string parentId)
        {
            var fileContents = File.ReadAllText(Filename);
            var cache = string.IsNullOrEmpty(fileContents)
                ? new List<CacheObject>()
                : JsonConvert.DeserializeObject<List<CacheObject>>(fileContents);
            var exsistingRecord = cache.SingleOrDefault(x => x.Id.Equals(node.Id));
            if (exsistingRecord != null)
            {
                exsistingRecord.Name = node.Name;
                exsistingRecord.ParentId = parentId;
            }
            else
            {
                cache.Add(new CacheObject
                {
                    Id = node.Id,
                    Name = node.Name,
                    ParentId = parentId,
                });
            }
            File.WriteAllText(Filename, JsonConvert.SerializeObject(cache));
        }

        public string GetIdByNameAndParent(string name, string parentId)
        {
            var fileContents = File.ReadAllText(Filename);
            var cache = string.IsNullOrEmpty(fileContents) ? new CacheObject[0] : JsonConvert.DeserializeObject<CacheObject[]>(fileContents);
            if (cache.Length == 0)
            {
                return null;
            }
            return cache.SingleOrDefault(x => x.ParentId.Equals(parentId) && x.Name.Equals(name))?.Id;
        }


        private class CacheObject
        {
            public string Id { get; set; }

            public string ParentId { get; set; }

            public string Name { get; set; }
        }
    }
}