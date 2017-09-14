using GDriveClientLib.Abstractions;
using GDriveClientLib.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Autofac;
using log4net;
using log4net.Core;
using GoogleFileManager = GoogleDriveFileSystemLib.FileManager;
using LocalFileManager = LocalFileSystemLib.FileManager;

using WebClient.Business;

namespace WebClient.API
{
    public class ListController : ApiController
    {
        private LocalFileManager LocalFileManager { get; set; }
        private GoogleFileManager GoogleFileManager { get; set; }

        public ListController()
        {
            var appDataFolder = $@"{HttpRuntime.AppDomainAppPath}App_Data\";
            var init = new Initializer();
            var container = init.RegisterComponents(appDataFolder);

            LocalFileManager = new LocalFileManager();
            GoogleFileManager = new GoogleFileManager(container.Resolve<IGoogleDriveService>());
        }


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
        [Route("api/list/GetLocalFolders/")]
        public async Task<IEnumerable<string>> GetLocalFolders([FromUri] string rootPathEncoded = "")
        {

            var rootPath = string.IsNullOrEmpty(rootPathEncoded)
                ? @"G:\Coding\GoogleDriveClient"
                : Encoding.UTF8.GetString(Convert.FromBase64String(rootPathEncoded));
            
            var localTree = await LocalFileManager.GetTree(rootPath);

            return localTree.Children.Select(x => x.Name);
        }

        [HttpGet]
        [Route("api/list/GetRemoteFolders/")]
        public async Task<IEnumerable<string>> GetRemoteFolders([FromUri] string rootId)
        {
            if (string.IsNullOrEmpty(rootId))
            {
                rootId = "root";
            }

            var remoteTree = await GoogleFileManager.GetTree(rootId);

            return remoteTree.Children.Select(x => x.Name);
        }
    }
}