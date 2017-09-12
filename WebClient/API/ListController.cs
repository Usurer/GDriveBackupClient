using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Autofac;

using GoogleFileManager = GoogleDriveFileSystemLib.FileManager;
using LocalFileManager = LocalFileSystemLib.FileManager;

using WebClient.Business;

namespace WebClient.API
{
    public class ListController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return id.ToString();
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("api/list/GetLocalFolders/{rootPath}")]
        public async Task<IEnumerable<string>> GetLocalFolders(string rootPath)
        {
            var appDataFolder = $@"{HttpRuntime.AppDomainAppPath}App_Data\";
            var init = new Initializer();
            var container = init.RegisterComponents(appDataFolder);

            var localManager = new LocalFileManager();
            var googleManager = new GoogleFileManager(container.Resolve<IGoogleDriveService>());

            var localTree = await localManager.GetTree(@"G:\Coding\GoogleDriveClient");
            var remoteTree = await googleManager.GetTree("root");


            var a = HttpRuntime.AppDomainAppPath;
            var b = HttpRuntime.AppDomainAppVirtualPath;

            //return new[] {a, b, rootPath};
            return new[] { localTree.Name, remoteTree.Name };

        }
    }
}