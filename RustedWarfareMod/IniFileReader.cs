namespace RustedWarfareMod;

public class IniFileReader
{
    public static IniFileReader Default { get; } = new();

    public IniFile Read(string filePath)
    {
        using Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        return Read(stream);
    }
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
