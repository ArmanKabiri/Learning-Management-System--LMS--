using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VU.Entities;

namespace VU.BLL
{
    public class SnapshotManager
    {
        public void Initialize()
        {
            try
            {
                CacheFactory.Instance.CreateCache<String, Snapshot>("Snapshot");
            }
            catch (Exception bllException)
            {
           
            }
        }
        public void Insert(Snapshot snapshot)
        {
            try
            {
                CacheFactory.Instance.Add<String, Snapshot>("Snapshot", snapshot.ID.ToString(), snapshot);
            }
            catch (Exception bllException)
            {
                
            }
        }
    }
}
