using System.Collections.Generic;
using System.Linq;
using GDriveClientLib.Abstractions;

namespace GDriveBackupClient
{
    public class MemoryCacheManager
    {
        private readonly List<CacheObject> Storage;

        public MemoryCacheManager()
        {
            Storage = new List<CacheObject>();
        }

        public void AddOrUpdate(INode node, string parentId)
        {
            var exsistingRecord = Storage.SingleOrDefault(x => x.Id.Equals(node.Id));
            if (exsistingRecord != null)
            {
                exsistingRecord.Name = node.Name;
                exsistingRecord.ParentId = parentId;
            }
            else
            {
                Storage.Add(new CacheObject
                {
                    Id = node.Id,
                    Name = node.Name,
                    ParentId = parentId
                });
            }
        }

        public string GetIdByNameAndParent(string name, string parentId)
        {
            if (Storage.Count == 0)
            {
                return null;
            }
            return Storage.SingleOrDefault(x => x.ParentId.Equals(parentId) && x.Name.Equals(name))?.Id;
        }


        private class CacheObject
        {
            public string Id { get; set; }

            public string ParentId { get; set; }

            public string Name { get; set; }
        }
    }
}