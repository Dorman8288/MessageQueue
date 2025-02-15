using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.PluginLoader
{
    public class PluginLoaderBase
    {
        ILogger logger;

        public PluginLoaderBase(ILogger logger) => this.logger = logger;

        public virtual bool IsEligableType(Type type) => true;

        public IEnumerable<Type> ExtractTypes(string path)
        {
            List<Type> types = [];
            var candidates = Assembly.LoadFrom(path).GetTypes();
            foreach (Type type in candidates)
                if (IsEligableType(type))
                    types.Add(type);
            return types.ToArray();
        }

        public IEnumerable<Type> Load(string folderPath)
        {
            logger.Info($"Looking for Eligable Types in folder ({folderPath})");
            var files = Directory.GetFiles(folderPath);
            if (files.Length == 0)
                logger.Warn($"The Provided Folder Path ({folderPath}) does not contain any dll files.");
            List<Type> types = [];
            foreach (var file in files)
            {
                string extension = Path.GetExtension(file);
                if (extension == ".dll")
                {
                    logger.Info($"Looking for Eligable Types in folder ({file})");
                    var foundedTypes = ExtractTypes(file);
                    types.AddRange(foundedTypes);
                    if (foundedTypes.Count() == 0)
                    {
                        logger.Warn($"Found No Eligable Types in File {file}");
                        continue;
                    }
                    logger.Info($"Found the Following Types in {file}");
                    foreach (var type in types)
                        logger.Info($"+ Added type: {type}");
                }
            }
            if (types.Count == 0)
                logger.Warn("No Eligable Types Found");
            return types.ToArray();
        }
    }
}
