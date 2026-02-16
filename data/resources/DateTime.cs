using Godot;

namespace Quasar.data.resources
{
    [GlobalClass]
    public partial class DateTime : Resource
    {
        [Export(PropertyHint.Range, "0, 59")]
        public int Seconds { get; set; } = 0;

        [Export(PropertyHint.Range, "0, 59")]
        public int Minutes { get; set; } = 0;

        [Export(PropertyHint.Range, "0, 23")]
        public int Hours { get; set; } = 0;

        [Export(PropertyHint.Range, "0, 6")]
        public int Days { get; set; } = 0;

        [Export(PropertyHint.Range, "0, 3")]
        public int Weeks { get; set; } = 0;

        [Export(PropertyHint.Range, "0, 2")]
        public int Months { get; set; } = 0;

        [Export(PropertyHint.Range, "0, 3")]
        public int Seasons { get; set; } = 0;

        [Export(PropertyHint.Range, "0, 999")]
        public int Years { get; set; } = 0;

        [Export]
        public int Ages { get; set; } = 0;

        private double _deltaTime = 0;

        public void IncreaseTimeInSeconds(double deltaSeconds)
        {
            _deltaTime += deltaSeconds;

            if (_deltaTime >= 1.0f)
            {
                var deltaTimeInt = (int)_deltaTime;
                _deltaTime -= deltaTimeInt;

                Seconds += deltaTimeInt;
                Minutes += Seconds / 60;
                Hours += Minutes / 60;
                Days += Hours / 24;
                Weeks += Days / 7;
                Months += Weeks / 4;
                Seasons += Months / 3;
                Years += Seasons / 4;
                Ages += Years / 1000;

                Seconds %= 60;
                Minutes %= 60;
                Hours %= 24;
                Days %= 7;
                Weeks %= 4;
                Months %= 3;
                Seasons %= 4;
                Years %= 1000;
            }
        }

        public override string ToString()
        {
            return $"Ages: {Ages} Years: {Years} Seasons: {Seasons} " +
                   $"Months: {Months} Weeks: {Weeks} Days: {Days} " +
                   $"Hours: {Hours} Minutes: {Minutes} Seconds: {Seconds}";
        }
    }
}
