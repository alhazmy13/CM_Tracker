using Blish_HUD.Settings;
using Newtonsoft.Json;

namespace falcon.cmtracker.Persistance
{

    public enum BossType
    {
        Raid,Strike
    }
    public class Token
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public string Icon { get; set; }
        public BossType bossType { get; set; }

        public SettingValue setting { get; set; }

    }
}