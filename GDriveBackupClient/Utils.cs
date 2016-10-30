using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDriveClientLib.Abstractions;

namespace GDriveBackupClient
{
    class Utils
    {
        public class ComparisonResult
        {
            public HashSet<string> CommonNodesFistSequenceIds { get; set; }

            public HashSet<string> CommonNodesSecondSequenceIds { get; set; } 
        }

        public class NodeComparer : IEqualityComparer<INode>
        {
            public bool Equals(INode x, INode y)
            {
                return x.NodeType == y.NodeType && x.Name == y.Name;
            }

            public int GetHashCode(INode obj)
            {
                return (obj.NodeType.ToString() + obj.Name).GetHashCode();
            }
        }

        public static ComparisonResult CompareTwoEnumerables(IEnumerable<INode> first, IEnumerable<INode> second)
        {
            var result = new ComparisonResult
                         {
                             CommonNodesFistSequenceIds = new HashSet<string>(),
                             CommonNodesSecondSequenceIds = new HashSet<string>(),   
                         };

            foreach (var firstListMember in first)
            {
                var similar = second.SingleOrDefault(x => x.Name == firstListMember.Name 
                                    && x.NodeType == firstListMember.NodeType);
                if (similar != null)
                {
                    result.CommonNodesFistSequenceIds.Add(firstListMember.Id);
                    result.CommonNodesSecondSequenceIds.Add(similar.Id);
                }
            }

            return result;
        }
    }
}
