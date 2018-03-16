using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ApiDoc.Models
{
    public static class ApiDocReporter
    {
        public static List<IApiDoc> LoadHtmlApiDocFromFolder(string directoryPath)
        {
            List<IApiDoc> apiDocs = new List<IApiDoc>();

            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                foreach (FileInfo file in directory.EnumerateFiles("*.html"))
                {
                    if (file.Name.Split('.').Count() == 2)  //don't add the html file with mulitple period. e.g AccountManagement.amChangeAddress.html
                    {
                        HtmlApiDoc apiDoc = new HtmlApiDoc { Name = Path.GetFileNameWithoutExtension(file.Name), URI = file.FullName };
                        apiDocs.Add(apiDoc);
                       
                    }
                }
            }
       
             return apiDocs;
        }
    }
}