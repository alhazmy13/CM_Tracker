using System.Collections.Generic;
using Newtonsoft.Json;
using static falcon.cmtracker.Module;

namespace falcon.cmtracker.Persistance
{

    internal class Bosses
    {

        public Bosses(List<SettingValue> setting)
        {
            Tokens = GenrateTokens(setting);

        }

        [JsonProperty("tokens")] public IList<Token> Tokens { get; set; }

        private List<Token> GenrateTokens(List<SettingValue> setting)
        {
            var tokens = new List<Token>();


            var Keep_Construct = new Token();
            Keep_Construct.Id = 77302;
            Keep_Construct.Name = "Keep Construct";
            Keep_Construct.Icon = "icon_keep_Construct.png";
            Keep_Construct.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Keep_Construct);
            Keep_Construct.bossType = BossType.Raid;
            tokens.Add(Keep_Construct);

            var Cairn = new Token();
            Cairn.Id = 77302;
            Cairn.Name = "Cairn";
            Cairn.Icon = "icon_cairn.png";
            Cairn.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Cairn);
            Cairn.bossType = BossType.Raid;

            tokens.Add(Cairn);

            var Mursaat_Overseer = new Token();
            Mursaat_Overseer.Id = 77302;
            Mursaat_Overseer.Name = "Mursaat Overseer";
            Mursaat_Overseer.Icon = "icon_mursaat_overseer.png";
            Mursaat_Overseer.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Mursaat_Overseer);
            Mursaat_Overseer.bossType = BossType.Raid;
            tokens.Add(Mursaat_Overseer);

            var Samarog = new Token();
            Samarog.Id = 77302;
            Samarog.Name = "Samarog";
            Samarog.Icon = "icon_samarog.png";
            Samarog.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Samarog);
            Samarog.bossType = BossType.Raid;

            tokens.Add(Samarog);

            var Deimos = new Token();
            Deimos.Id = 77302;
            Deimos.Name = "Deimos";
            Deimos.Icon = "icon_deimos.png";
            Deimos.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Deimos);
            Deimos.bossType = BossType.Raid;

            tokens.Add(Deimos);

            var Soulless_Horror = new Token();
            Soulless_Horror.Id = 77302;
            Soulless_Horror.Name = "Soulless Horror";
            Soulless_Horror.Icon = "icon_sh.png";
            Soulless_Horror.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Soulless_Horror);
            Soulless_Horror.bossType = BossType.Raid;

            tokens.Add(Soulless_Horror);

            var Dhuum = new Token();
            Dhuum.Id = 77302;
            Dhuum.Name = "Dhuum";
            Dhuum.Icon = "icon_dhuum.png";
            Dhuum.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Dhuum);
            Dhuum.bossType = BossType.Raid;

            tokens.Add(Dhuum);

            var Conjured_Amalgamate = new Token();
            Conjured_Amalgamate.Id = 77302;
            Conjured_Amalgamate.Name = "Conjured Amalgamate";
            Conjured_Amalgamate.Icon = "icon_ca.png";
            Conjured_Amalgamate.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Conjured_Amalgamate);
            Conjured_Amalgamate.bossType = BossType.Raid;

            tokens.Add(Conjured_Amalgamate);

            var Twin_Largos = new Token();
            Twin_Largos.Id = 77302;
            Twin_Largos.Name = "Twin Largos";
            Twin_Largos.Icon = "icon_twin_largos.png";
            Twin_Largos.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Twin_Largos);
            Twin_Largos.bossType = BossType.Raid;

            tokens.Add(Twin_Largos);

            var Qadim = new Token();
            Qadim.Id = 77302;
            Qadim.Name = "Qadim";
            Qadim.Icon = "icon_qadim.png";
            Qadim.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Qadim);
            Qadim.bossType = BossType.Raid;

            tokens.Add(Qadim);


            var Adina = new Token();
            Adina.Id = 77302;
            Adina.Name = "Adina";
            Adina.Icon = "icon_adina.png";
            Adina.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Adina);
            Adina.bossType = BossType.Raid;

            tokens.Add(Adina);

            var Sabir = new Token();
            Sabir.Id = 77302;
            Sabir.Name = "Sabir";
            Sabir.Icon = "icon_sabir.png";
            Sabir.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Sabir);
            Sabir.bossType = BossType.Raid;

            tokens.Add(Sabir);



            var Qadim2 = new Token();
            Qadim2.Id = 77302;
            Qadim2.Name = "Qadim the Peerless";
            Qadim2.Icon = "icon_qadim2.png";
            Qadim2.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Qadim2);
            Qadim2.bossType = BossType.Raid;

            tokens.Add(Qadim2);


            var Aetherblade_Hideout = new Token();
            Aetherblade_Hideout.Id = 77302;
            Aetherblade_Hideout.Name = "Aetherblade Hideout";
            Aetherblade_Hideout.Icon = "icon_aetherblade_hideout.png";
            Aetherblade_Hideout.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Aetherblade_Hideout);
            Aetherblade_Hideout.bossType = BossType.Strike;

            tokens.Add(Aetherblade_Hideout);

            var Xunlai_Jade_Junkyard = new Token();
            Xunlai_Jade_Junkyard.Id = 77302;
            Xunlai_Jade_Junkyard.Name = "Xunlai Jade Junkyard";
            Xunlai_Jade_Junkyard.Icon = "icon_xunlai_jade.png";
            Xunlai_Jade_Junkyard.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Xunlai_Jade_Junkyard);
            Xunlai_Jade_Junkyard.bossType = BossType.Strike;

            tokens.Add(Xunlai_Jade_Junkyard);

            var Kaineng_Overlook = new Token();
            Kaineng_Overlook.Id = 77302;
            Kaineng_Overlook.Name = "Kaineng Overlook";
            Kaineng_Overlook.Icon = "icon_kaineng_overlook.png";
            Kaineng_Overlook.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Kaineng_Overlook);
            Kaineng_Overlook.bossType = BossType.Strike;

            tokens.Add(Kaineng_Overlook);

            var Harvest_Temple = new Token();
            Harvest_Temple.Id = 77302;
            Harvest_Temple.Name = "Harvest Temple";
            Harvest_Temple.Icon = "icon_harvest_temple.png";
            Harvest_Temple.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Harvest_Temple);
            Harvest_Temple.bossType = BossType.Strike;

            tokens.Add(Harvest_Temple);

            var Old = new Token();
            Old.Id = 77302;
            Old.Name = "Old Lion's Court";
            Old.Icon = "icon_old.png";
            Old.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Old);
            Old.bossType = BossType.Strike;

            tokens.Add(Old);

            var Cosmic_Observatory = new Token();
            Cosmic_Observatory.Id = 77302;
            Cosmic_Observatory = "Cosmic Observatory";
            Cosmic_Observatory.Icon = "icon_cosmic_observatory.png";
            Cosmic_Observatory.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Cosmic_Observatory);
            Cosmic_Observatory.bossType = BossType.Strike;
            tokens.Add(Cosmic_Observatory);

            var Temple_Of_Febe = new Token();
            Temple_Of_Febe.Id = 77302;
            Temple_Of_Febe = "Temple of Febe";
            Temple_Of_Febe.Icon = "icon_temple_of_febe.png";
            Temple_Of_Febe.setting = SettingUtil.GetSettingForBoss(setting, Module.CURRENT_ACCOUNT.Value, Boss.Temple_Of_Febe);
            Temple_Of_Febe.bossType = BossType.Strike;
            tokens.Add(Temple_Of_Febe);



            return tokens;
        }
    }
}