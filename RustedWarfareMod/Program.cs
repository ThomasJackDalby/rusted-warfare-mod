
public class Program
{
    public static void Main(string[] args)
    {
        IniFileReader reader = new();
        string[] folderPaths = new[] {
             @"C:\Program Files (x86)\Steam\steamapps\common\Rusted Warfare\assets",
             @"C:\Program Files (x86)\Steam\steamapps\workshop\content\647960"
        };

        List<IniFile> files = new();
        foreach (string filePath in folderPaths.SelectMany(folderPath => Directory.EnumerateFiles(folderPath, "*.ini", SearchOption.AllDirectories)))
        {
            using Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            files.Add(reader.Read(stream));
        }

        IniSectionSchema parseSectionSchema(string sectionName)
        {           
            IniSection[] sections = files
                    .SelectMany(file => file.Sections.Where(section => sectionName.StartsWith("#") ? section.Name.StartsWith(sectionName[1..]) : section.Name == sectionName))
                    .ToArray();

            IniPropertySchema parsePropertySchema(string propertyName)
            {
                string[] propertyValues = sections
                    .SelectMany(section => section.Properties.Where(property => property.Name == propertyName))
                    .Select(property => property.Value)
                    .Distinct()
                    .ToArray();

                IniPropertyType propertyType = parsePropertyType(propertyValues);

                return new IniPropertySchema(propertyName, propertyType, propertyValues);
            }

            IniPropertySchema[] propertySchemas = sections
                .SelectMany(section => section.Properties.Select(property => property.Name))
                .Distinct()
                .Select(parsePropertySchema)
                .OrderBy(property => property.Name)
                .ToArray();

            return new IniSectionSchema(sectionName, propertySchemas);
        }

        IniSectionSchema[] sectionSchemas = files
                .SelectMany(file => file.Sections.Select(section => section.Name))
                .Select(parseSectionName)
                .Distinct()
                .Select(parseSectionSchema)
                .OrderBy(section => section.Name)
                .ToArray();

        IniFileSchema fileSchema = new(sectionSchemas);


    }

    static string[] sectionPrefixes =
    {
        "action",
        "effect",
        "projectile",
        "turret",
        "decal",
        "hiddenAction",
        "arm",
        "attachment",
        "canBuild",
        "leg",
        "animation"
    };

    static string parseSectionName(string sectionName)
    {
        string? sectionPrefix = sectionPrefixes.FirstOrDefault(sectionName.StartsWith);
        if (sectionPrefix is not null) return $"#{sectionPrefix}";
        return sectionName;
    }

    static IniPropertyType parsePropertyType(string[] values)
    {
        if (values.All(value => Int32.TryParse(value, out _))) return IniPropertyType.Integer;
        else if (values.All(value => Double.TryParse(value, out _))) return IniPropertyType.Float;
        else if (values.All(value => Boolean.TryParse(value, out _))) return IniPropertyType.Boolean;
        else if (values.All(value => value.All(Char.IsUpper))) return IniPropertyType.Enum;
        else return IniPropertyType.String;
    }
}
public enum IniPropertyType
{
    Integer,
    Float,
    Boolean,
    String,
    Enum
};
public record IniPropertySchema(string Name, IniPropertyType Type, string[] Values);
public record IniSectionSchema(string Name, IniPropertySchema[] Properties);
public record IniFileSchema(IniSectionSchema[] Sections);

class IniFileReader
{
    public IniFile Read(Stream stream)
    {
        StreamReader reader = new(stream);

        IniFile file = new();
        IniSection? section = null;
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine()?.Trim();
            if (line is not null && line.Contains('#')) line = line[..line.IndexOf('#')];
            if (String.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("[")) // its a new group
            {
                section = null;
                int end = line.IndexOf(']');
                string groupName = line.Substring(1, end - 1);
                if (groupName.StartsWith("comment")) continue;
                section = new IniSection(groupName);
                file.Sections.Add(section);
            }
            else
            {
                if (section is null) continue;
                if (!line.Contains(":")) continue;
                string[] parts = line.Split(":");
                string propertyName = parts[0].Trim();
                string propertyValue = parts[1].Trim();
                section.Properties.Add(new IniProperty(propertyName, propertyValue));
            }
        }

        return file;
    }
}

public record IniSection(string Name)
{
    public List<IniProperty> Properties { get; } = new();
}
public record IniProperty(string Name, string Value);

public class IniFile
{
    public List<IniSection> Sections { get; } = new();

}