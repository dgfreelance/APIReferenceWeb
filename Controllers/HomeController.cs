using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApiDoc.Models;
using System.Data;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using PagedList;
namespace ApiDoc.Controllers
{
    public class HomeController : Controller
    {
        private CustomConfigurationSection _CustomConfig;

        public HomeController()
        {
            _CustomConfig= (CustomConfigurationSection)System.Configuration.ConfigurationManager.GetSection("CustomConfigurationSectionGroup/CustomConfiguration");
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            List<IApiDoc> apiDocs = new List<IApiDoc>();
            apiDocs = ApiDocReporter.LoadHtmlApiDocFromFolder(Server.MapPath(_CustomConfig.ApiDocDirectory.Path));
            return View(apiDocs);
        }

        //
        // GET: /Search/
        public ActionResult Search(String searchingText,int page = 1, int pageSize = 10)
        {
            ViewBag.searchingText = searchingText;
            //create index
            string indexDirectory = Server.MapPath(_CustomConfig.IndexDirectory.Path);
            IntranetIndexer intranetIndexer = new IntranetIndexer(indexDirectory);
            string searchingDirectory = Server.MapPath(_CustomConfig.SearchingDirectory.Path);
            intranetIndexer.AddDirectory(new DirectoryInfo(searchingDirectory), "*.htm*");
            intranetIndexer.Close();

            //search text using index directory
            IntranetSearcher intranetSearcher = new IntranetSearcher();
            List<SearchResult> results = intranetSearcher.search(indexDirectory, searchingText);

            PagedList<SearchResult> pageListResults = new PagedList<SearchResult>(results, page, pageSize);
            return View(pageListResults);
        }
   }
}
