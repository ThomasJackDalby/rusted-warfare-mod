using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace RustedWarfareMod;


public class Program
{
    public static void Main(string[] args)
    {

    }
}

//public class TestMod
//{
//    public Mod Create()
//    {

//        Mod mod = new();

//        var entity = mod.AddEntity("my-unit");

//        //entity.

//    }

//}

public class Mod
{
    private readonly List<Entity> entities = [];

    public Entity AddEntity(string name)
    {
        return new Entity();
    }
}


public abstract class Section
{

}

// folder structure
// assets
// units
// templates

public enum IniPropertyType
{
    Integer,
    Float,
    Boolean,
    String,
    Enum
};


