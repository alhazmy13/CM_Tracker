using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace falcon.cmtracker
{

    public enum Boss
    {
        Keep_Construct, 
        Cairn,Mursaat_Overseer, Samarog,Deimos,
        Soulless_Horror, Dhuum ,
        Conjured_Amalgamate,Twin_Largos, Qadim, 
        Sabir,Adina, Qadim2,
        Greer, Decima, Ura,
        Aetherblade_Hideout,Xunlai_Jade_Junkyard  ,Kaineng_Overlook  ,Harvest_Temple, Old,
        Cosmic_Observatory, Temple_Of_Febe
}
    public class SettingValue: INotifyPropertyChanged
    {
        private Boss _boss;
        private bool _value;
        private string _account;

        public SettingValue(string account, Boss boss, bool value)
        {
            this._account = account;
            this._boss = boss;
            this._value = value;
        }

        public Boss Boss
        {
            get => _boss;
            set
            {
                if (_boss == value) return;
                _boss = value;
            }
        }

        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                NotifyPropertyChanged();
            }
        }

        public string Account
        {
            get => _account;
            set
            {
                if (_account == value) return;
                _account = value;
            }
        }

        protected void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    class SettingUtil : INotifyPropertyChanged
    {
        private HashSet<SettingValue> Setting = new HashSet<SettingValue>(new SettingValueComparer());
        private string _localSettingValue = "";
        public string SettingString
        {
            get => _localSettingValue;
            set
            {
                if (_localSettingValue == value) return;
                _localSettingValue = value;
                NotifyPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public SettingUtil()
        {

        }
        public SettingUtil(String value)
        {
            this._localSettingValue = value;
            this.UpdateSettting(value);
        }

        public void UpdateSettting(String stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return;
            }
            var Values = stringValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(String Item in Values)
            {
                var SplitSetting = Item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                Enum.TryParse(SplitSetting[1], out Boss boss);
                var NewSetting = new SettingValue(SplitSetting[0], boss, Boolean.Parse(SplitSetting[2]));
                NewSetting.PropertyChanged += new PropertyChangedEventHandler(WhenSettingValueChanged);
                this.Setting.Add(NewSetting);
            }
        }

        private void WhenSettingValueChanged(object sender, PropertyChangedEventArgs e)
        {
           var tempSetting = (SettingValue)sender;
           foreach (SettingValue Item in Setting)
            {
                if (Item.Account == tempSetting.Account && Item.Boss == tempSetting.Boss)
                {
                    Item.Value = tempSetting.Value;
                    break;
                }
            }
            this.SettingString = ConvertHashToString(this.Setting);
        }

        public String ConvertHashToString(HashSet<SettingValue> arrayValues)
        {
            var localValue = "";
            foreach (SettingValue Item in arrayValues)
            {
                localValue += String.Format("{0}:{1}:{2};", Item.Account, Item.Boss, Item.Value);
            }
            return localValue;
        }

        static public SettingValue GetSettingForBoss(List<SettingValue> settingList , string account, Boss boss)
        {
            foreach(SettingValue Item in settingList)
            {
                if(Item.Account == account && Item.Boss == boss)
                {
                    return Item;
                }
            }
            return SettingUtil.AddNewBoss(settingList, account, boss);
        }

        public List<SettingValue> GetSettingForAccount(string account)
        {

            List<SettingValue> localSetting = new List<SettingValue>();
            foreach (SettingValue Item in Setting)
            {
                if (Item.Account == account)
                {
                    localSetting.Add(Item);
                }
            }
            return localSetting;
        }

        protected void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void AddNewAccount(string accountName)
        {
            foreach (Boss boss in (Boss[])Enum.GetValues(typeof(Boss)))
            {
                this.Setting.Add(new SettingValue(accountName, boss, false));
            }
            this.SettingString = ConvertHashToString(this.Setting);

        }

        static public SettingValue AddNewBoss(List<SettingValue> settingList, string account, Boss boss)
        {
            settingList.Add(new SettingValue(account, boss, false));
            return settingList[settingList.Count - 1];

        }

        public HashSet<string> GetAllAccounts()
        {
            HashSet<string> accounts = new HashSet<string>();
            foreach(SettingValue Item in Setting)
            {
                accounts.Add(Item.Account);
            }
            return accounts;
        }

        public void ResetAllValues()
        {
            foreach (SettingValue Item in Setting)
            {
                Item.Value = false;
            }
            this.SettingString = ConvertHashToString(this.Setting);
        }
    }
}
