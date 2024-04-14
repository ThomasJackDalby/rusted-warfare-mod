namespace RustedWarfareMod;

public class EntitySchemaEvaluator
{
    public IniFileSchema Evaluate(IEnumerable<IniFile> files)
    {
        IniSectionSchema parseSectionSchema(string sectionName)
        {
            IniSection[] sections = files
                    .SelectMany(file => file.Sections
                        .Where(section => sectionName.StartsWith("#")
                            ? section.Name.StartsWith(sectionName[1..])
                            : section.Name == sectionName))
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

        return new(sectionSchemas);
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
        "resource",
        "template",
        "animation",
        "placementRule"
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

