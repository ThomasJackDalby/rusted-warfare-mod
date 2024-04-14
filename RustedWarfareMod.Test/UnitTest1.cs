using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RustedWarfareMod.Test
{
    public class EntitySchemaGeneration
    {
        [Fact]
        public void Generate()
        {
            string[] folderPaths = [
                "D:\\SteamLibrary\\steamapps\\workshop\\content\\647960",
                "D:\\SteamLibrary\\steamapps\\common\\Rusted Warfare\\assets",
                "D:\\SteamLibrary\\steamapps\\common\\Rusted Warfare\\assets\\builtin_mods",
            ];

            List<IniFile> files = folderPaths
                .SelectMany(folderPath => Directory.EnumerateFiles(folderPath, "*.ini", SearchOption.AllDirectories))
                .Select(IniFileReader.Default.Read)
                .ToList();

            EntitySchemaEvaluator evaluator = new();
            IniFileSchema result = evaluator.Evaluate(files);

            EntitySchemaWriter.Default.Write("../../../../RustedWarfareMod/Entity.cs", result);
        }
    }

    public class EntitySchemaWriter
    {
        public static EntitySchemaWriter Default { get; } = new();

        public void Write(string filePath, IniFileSchema schema)
        {
            using Stream stream = File.Open(filePath, FileMode.Create, FileAccess.Write);
            Write(stream, schema);
        }
        public void Write(Stream stream, IniFileSchema schema)
        {
            using StreamWriter writer = new(stream);

            writer.WriteLine("namespace RustedWarfareMod;");
            writer.WriteLine("");
            writer.WriteLine("public class Entity");
            writer.WriteLine("{");
            foreach(var section in schema.Sections)
            {
                string sectionName = section.Name;
                if (sectionName.StartsWith("#")) sectionName = sectionName[1..];
                sectionName = Char.ToUpper(sectionName[0]) + sectionName[1..];

                string typeName = $"{sectionName}Section";
                string propertyName = sectionName;



                writer.WriteLine($"\tpublic {typeName} {propertyName} {{ get; }} = new();");
            }
            writer.WriteLine("}");

            foreach(var section in schema.Sections)
            {
                string sectionName = section.Name;
                if (sectionName.StartsWith("#")) sectionName = sectionName[1..];
                sectionName = sectionName.ToUpperFirst();

                string typeName = $"{sectionName}Section";

                writer.WriteLine($"public class {typeName}");
                writer.WriteLine("{");
                foreach(var property in section.Properties)
                {
                    if (property.Name.StartsWith("@")) continue;
                    if (property.Name.StartsWith("-")) continue;

                    string formattedPropertyType = property.Type switch
                    {
                        IniPropertyType.Integer => "int",
                        IniPropertyType.Float => "double",
                        IniPropertyType.Boolean => "bool",
                        IniPropertyType.String => "string",
                        _ => "object",
                    };
                    string propertyName = property.Name;

                    propertyName = propertyName.ToUpperFirst();
                    
                    writer.WriteLine($"\tpublic {formattedPropertyType} {propertyName} {{ get; set; }}");
                }
                writer.WriteLine("}");
            }
        }
    }

    public static class Extensions
    {
        public static string ToUpperFirst(this string self) => Char.ToUpper(self[0]) + self[1..];
    }
}