using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RustedWarfareMod.Test
{
    public class EntitySchemaGeneration
    {
        [Fact]
        public void Generate()
        {
            //string[] folderPaths = [
            //    "D:\\SteamLibrary\\steamapps\\workshop\\content\\647960",
            //    "D:\\SteamLibrary\\steamapps\\common\\Rusted Warfare\\assets",
            //    "D:\\SteamLibrary\\steamapps\\common\\Rusted Warfare\\assets\\builtin_mods",
            //];
            string[] folderPaths = [
                @"C:\Program Files (x86)\Steam\steamapps\common\Rusted Warfare\assets",
                @"C:\Program Files (x86)\Steam\steamapps\workshop\content"
            ];

            List<IniFile> files = folderPaths
                .SelectMany(folderPath => Directory.EnumerateFiles(folderPath, "*.ini", SearchOption.AllDirectories))
                .Select(IniFileReader.Default.Read)
                .ToList();

            EntitySchemaEvaluator evaluator = new();
            EntityFileSchema result = evaluator.Evaluate(files);

            EntitySchemaWriter.Default.Write("../../../../RustedWarfareMod/Entity.cs", result);
        }
    }


}