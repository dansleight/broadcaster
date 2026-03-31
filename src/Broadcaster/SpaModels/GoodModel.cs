using NameGenerator;
using NameGenerator.Generators;

namespace Broadcaster.SpaModels;

public class GoodModel
{
    #region Properties

    public int Id { get; set; } = 1;
    public string Name { get; set; } = null!;

    #endregion

    #region Constructor

    public GoodModel() { }

    public GoodModel(int id)
    {
        Id = id;
        if (Id == 1)
        {
            Random random = new Random();
            Id = random.Next(1001, 10000);
        }
        RealNameGenerator Generator = new RealNameGenerator();
        Name = Generator.Generate();
    }

    #endregion
}
