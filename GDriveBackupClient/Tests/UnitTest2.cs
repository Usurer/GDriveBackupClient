using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDriveBackupClient;
using GDriveClientLib.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Worker.Cache;

namespace Tests
{
    [TestClass]
    public class FileCacheManagerTest
    {
        private readonly Dictionary<Guid, string> RandomStringStorage = new Dictionary<Guid, string>();

        [TestMethod]
        public void TestAdd()
        {
            var manager = new FileCacheManager("cache");
            var root = GetRadomString();

            manager.AddOrUpdate(CreateNodeMoq(), root);

            Assert.IsTrue(File.Exists("cache"));
            Assert.IsTrue(!string.IsNullOrEmpty(File.ReadAllText("cache")));

            //cleanup
            File.Delete("cache");
        }

        [TestMethod]
        public void TestRead()
        {
            var manager = new FileCacheManager("cache");
            var root = GetRadomString();
            var node1 = new Node {Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString()};
            var node2 = new Node {Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString()};
            var child = new Node {Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString()};

            manager.AddOrUpdate(node1, root);
            manager.AddOrUpdate(node2, root);
            manager.AddOrUpdate(child, node1.Id);

            var fromCache = manager.GetIdByNameAndParent(child.Name, node1.Id);
            Assert.AreEqual(child.Id, fromCache);

            //cleanup
            File.Delete("cache");
        }

        [TestMethod]
        public void TestUpdate()
        {
            var manager = new FileCacheManager("cache");
            var root = GetRadomString();
            var node1 = new Node {Id = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString()};

            manager.AddOrUpdate(node1, root);
            node1.Name = GetRadomString();
            manager.AddOrUpdate(node1, root);

            var id = manager.GetIdByNameAndParent(node1.Name, root);

            Assert.AreEqual(node1.Id, id);

            //cleanup
            File.Delete("cache");
        }

        private INode CreateNodeMoq()
        {
            return new Node
            {
                Id = GetRadomString(),
                Name = GetRadomString()
            };
        }

        private string GetRadomString()
        {
            var s = Guid.NewGuid();
            while (RandomStringStorage.ContainsKey(s))
            {
                s = Guid.NewGuid();
            }
            RandomStringStorage.Add(s, s.ToString());
            return s.ToString();
        }

        public class Node : INode
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public NodeType NodeType { get; set; }

            public IEnumerable<INode> Children
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
        }
    }
}