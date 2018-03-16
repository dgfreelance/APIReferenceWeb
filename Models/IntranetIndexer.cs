using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace ApiDoc.Models
{
    /// <summary>
    /// Summary description for Indexer.
    /// </summary>
    /// <see href="http://www.dotlucene.net/30648/lucene-net-api-search-demo">lucene-net-api-search</see>
    public class IntranetIndexer
    {
        private IndexWriter writer;
        private string docRootDirectory;
        private string pattern;

        /// <summary>
        /// Creates a new index in <c>directory</c>. Overwrites the existing index in that directory.
        /// </summary>
        /// <param name="directory">Path to index (will be created if not existing).</param>
        public IntranetIndexer(string directory)
        {
            writer = new IndexWriter(FSDirectory.Open(directory), new StandardAnalyzer(Version.LUCENE_30), true, IndexWriter.MaxFieldLength.LIMITED);
            writer.UseCompoundFile = true;
        }

        /// <summary>
        /// Add HTML files from <c>directory</c> and its subdirectories that match <c>pattern</c>.
        /// </summary>
        /// <param name="directory">Directory with the HTML files.</param>
        /// <param name="pattern">Search pattern, e.g. <c>"*.html"</c></param>
        public void AddDirectory(DirectoryInfo directory, string pattern)
        {
            this.docRootDirectory = directory.FullName;
            this.pattern = pattern;

            addSubDirectory(directory);
        }

        private void addSubDirectory(DirectoryInfo directory)
        {
            foreach (FileInfo fi in directory.GetFiles(pattern))
            {
                AddHtmlDocument(fi.FullName);
            }
            foreach (DirectoryInfo di in directory.GetDirectories())
            {
                addSubDirectory(di);
            }
        }

        /// <summary>
        /// Loads, parses and indexes an HTML file.
        /// </summary>
        /// <param name="path"></param>
        public void AddHtmlDocument(string path)
        {
            Document doc = new Document();

            string html;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                html = sr.ReadToEnd();
            }

            int relativePathStartsAt = this.docRootDirectory.EndsWith("\\") ? this.docRootDirectory.Length : this.docRootDirectory.Length + 1;
            string relativePath = path.Substring(relativePathStartsAt);
           // string relativePath = path;

            doc.Add(new Field("text", ParseHtml(html), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("path", relativePath, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("title", GetTitle(html), Field.Store.YES, Field.Index.ANALYZED));

            writer.AddDocument(doc);
        }

        /// <summary>
        /// Very simple, inefficient, and memory consuming HTML parser. Take a look at Demo/HtmlParser in DotLucene package for a better HTML parser.
        /// </summary>
        /// <param name="html">HTML document</param>
        /// <returns>Plain text.</returns>
        private static string ParseHtml(string html)
        {
            string temp = Regex.Replace(html, "<[^>]*>", "");
            return temp.Replace("&nbsp;", " ");
        }

        /// <summary>
        /// Finds a title of HTML file. Doesn't work if the title spans two or more lines.
        /// </summary>
        /// <param name="html">HTML document.</param>
        /// <returns>Title string.</returns>
        private static string GetTitle(string html)
        {
            Match m = Regex.Match(html, "<title>(.*)</title>");
            if (m.Groups.Count == 2)
                return m.Groups[1].Value;
            return "(unknown)";
        }

        /// <summary>
        /// Optimizes and save the index.
        /// </summary>
        public void Close()
        {
            writer.Optimize();
            writer.Dispose();
        }


    }
}