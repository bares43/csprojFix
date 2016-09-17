using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace csprojFix.Service {
    class XML {

        public IDictionary<string,int> FindDuplicatedContentFiles(XDocument xdoc) {
            return this.FindDuplicatedFiles(xdoc, "Content");
        }

        public IDictionary<string, int> FindDuplicatedCompileFiles(XDocument xdoc) {
            return this.FindDuplicatedFiles(xdoc, "Compile");
        }

        public IList<string> FindMissingContentFilesOnDisk(XDocument xdoc, string basePath) {
            return this.FindMissingFilesOnDisk(xdoc, basePath, "Content");
        }

        public IList<string> FindMissingCompileFilesOnDisk(XDocument xdoc, string basePath) {
            return this.FindMissingFilesOnDisk(xdoc, basePath, "Compile");
        }

        private IList<string> FindMissingFilesOnDisk(XDocument xdoc, string basePath, string node) {
            var files = xdoc
                   .Root
                   .Descendants()
                   .Where(o => o.Name.LocalName == "ItemGroup")
                   .Descendants()
                   .Where(o => o.Name.LocalName == node)
                   .Select(o => o.Attribute("Include").Value)
                   .Distinct()
                   .ToList();

            var missingFiles = new List<string>();
            
            foreach (var file in files) {
                if (!File.Exists(basePath + "/" + file)) {
                    missingFiles.Add(file);
                }
            }

            return missingFiles;
        }
        
        private IDictionary<string, int> FindDuplicatedFiles(XDocument xdoc, string node) {
            var files = xdoc
                   .Root
                   .Descendants()
                   .Where(o => o.Name.LocalName == "ItemGroup")
                   .Descendants()
                   .Where(o => o.Name.LocalName == node)
                   .Select(o => o.Attribute("Include").Value)
                   .ToList()
                   .GroupBy(o => o)
                   .Select(group => new {
                       File = group.Key,
                       Count = group.Count()
                   })
                   .Where(o => o.Count > 1)
                   .ToDictionary(o => o.File, o => o.Count);
            
            return files;
        }

    }
}
