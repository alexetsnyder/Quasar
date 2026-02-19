namespace Quasar.scenes.cats
{
    public partial class CatData(string name, string description, string feelings, int health)
    {
        public string Name { get; set; } = name;

        public string Description { get; set; } = description;

        public string Feelings { get; set; } = feelings;

        public int Health { get; set; } = health;
    }
}