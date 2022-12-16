using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using falcon.cmtracker.Controls;
using falcon.cmtracker.Persistance;
using static Blish_HUD.GameService;
using Color = Microsoft.Xna.Framework.Color;
using Gw2Sharp.WebApi.V2.Models;

namespace falcon.cmtracker
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        internal static readonly Logger Logger = Logger.GetLogger<Module>();
        internal static Module ModuleInstance;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings)
        {
            var selfManagedSettings = settings.AddSubCollection("Managed Settings", false);

            CURRENT_ACCOUNT = selfManagedSettings.DefineSetting("CURRENT_ACCOUNT", LOCAL_ACCOUNT_NAME);
            CM_CLEARS = selfManagedSettings.DefineSetting("CM_CLEARS", "");

        }


        #region Settings
        public static SettingEntry<string> CURRENT_ACCOUNT;
        public static SettingEntry<string> CM_CLEARS;



        #endregion

        #region Cached Textures

        internal Texture2D _CmClearsIconTexture;
        internal Texture2D _CmClearsLogoTexture;

        internal Texture2D _deletedItemTexture;

        internal Texture2D _sortByStrikeTexture;
        internal Texture2D _sortByRaidTexture;
        internal Texture2D _notificationBackroundTexture;
        #endregion

        #region Constants

        private const int RIGHT_MARGIN = 5;
        private const int BOTTOM_MARGIN = 10;
        private const string LOCAL_ACCOUNT_NAME = "Local";

        #endregion

        #region Controls

        private WindowTab _ClearsTab;
        private Panel _modulePanel;
        private List<BossButton> _displayedBosses;
        private Panel _squadPanel;
        private static SettingUtil _localSetting;
        private Dropdown accountDropDown;
        private Panel contentPanel;
        #endregion


        #region Localization

        private string CmTrakcerTabName = "CM Tracker";

        private string SORTBY_ALL;
        private string SORTBY_STRIKES;
        private string SORTBY_RAID;
        private string ClearCheckboxTooltipText;

        private string CurrentSortMethod;

        #endregion

        #region Textures

        private Bosses _myBossesClears;

        #endregion

        protected override void Initialize()
        {
            _displayedBosses = new List<BossButton>();
            _localSetting = new SettingUtil(CM_CLEARS.Value);
            if (string.IsNullOrEmpty(CM_CLEARS.Value))
            {
                _localSetting.AddNewAccount(LOCAL_ACCOUNT_NAME);
            }

            Gw2ApiManager.SubtokenUpdated += OnApiSubTokenUpdated;
            LoadTextures();
        }

        protected override async Task LoadAsync()
        {
            if (_myBossesClears != null) return;
            _myBossesClears = new Bosses(_localSetting.GetSettingForAccount(CURRENT_ACCOUNT.Value));


        }



        private void ChangeLocalization(object sender, EventArgs e)
        {
            SORTBY_ALL = "Show All";
            SORTBY_STRIKES = "Strike";
            SORTBY_RAID = "Raid";
            CurrentSortMethod = SORTBY_ALL;

            _modulePanel?.Dispose();
            _modulePanel = BuildHomePanel(Overlay.BlishHudWindow);

            if (_ClearsTab != null)
                Overlay.BlishHudWindow.RemoveTab(_ClearsTab);

            _ClearsTab = Overlay.BlishHudWindow.AddTab(CmTrakcerTabName, _CmClearsIconTexture, _modulePanel, 0);
        }

        private void CMClearsUpdated(object sender, EventArgs e)
        {
            _localSetting.UpdateSettting((string)sender);
        }

        private void OnLocalSettingUpdated(object sender, EventArgs e)
        {
            var temp = (SettingUtil)sender;
            CM_CLEARS.Value = temp.SettingString;

        }


        private void LoadTextures()
        {
            _CmClearsIconTexture = ContentsManager.GetTexture("logo.png");
            _CmClearsLogoTexture = ContentsManager.GetTexture("logo.png");

            _deletedItemTexture = ContentsManager.GetTexture("deleted_item.png");

            _sortByStrikeTexture = ContentsManager.GetTexture("icon_strike.png");
            _sortByRaidTexture = ContentsManager.GetTexture("icon_raid.png");

            _notificationBackroundTexture = ContentsManager.GetTexture("ns-button.png");
        }

        protected override void OnModuleLoaded(EventArgs e)
        {

            ChangeLocalization(null, null);

            Overlay.UserLocaleChanged += ChangeLocalization;
            _localSetting.PropertyChanged += OnLocalSettingUpdated;
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private async Task FetchCurrentAccountName()
        {
            try
            {
                // v2/account/achievements requires "account" and "progression" permissions.

                Logger.Debug("Getting user achievements from the API.");

                var account = await Gw2ApiManager.Gw2ApiClient.V2.Account.GetAsync();


                Logger.Debug("Loaded {Account} player name from the API.", account.Name);
                _localSetting.AddNewAccount(account.Name);
                accountDropDown.Items.Clear();
                foreach (string Item in _localSetting.GetAllAccounts())
                {
                    accountDropDown.Items.Add(Item);
                }

            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to load account name.");
            }
        }


        protected override void Update(GameTime gameTime)
        {

        }

        /// <inheritdoc />
        protected override void Unload()
        {
            // Unload here
            _squadPanel?.Dispose();
            Gw2ApiManager.SubtokenUpdated -= OnApiSubTokenUpdated;
            foreach (var c in _displayedBosses) c?.Dispose();

            Overlay.BlishHudWindow.RemoveTab(_ClearsTab);
            // All static members must be manually unset
            ModuleInstance = null;
            // All static members must be manually unset
        }



        private Panel BuildHomePanel(WindowBase wndw)
        {
            return BuildTokensPanel(wndw);
        }

        private async void OnApiSubTokenUpdated(object sender, ValueEventArgs<IEnumerable<TokenPermission>> e)
        {
            await FetchCurrentAccountName();
        }

        public Panel BuildTokensPanel(WindowBase wndw)
        {
            var hPanel = new Panel
            {
                CanScroll = false,
                Size = wndw.ContentRegion.Size
            };

            var pageLoading = new LoadingSpinner
            {
                Parent = hPanel
            };

            pageLoading.Location = new Point(hPanel.Size.X / 2 - pageLoading.Size.X / 2, hPanel.Size.Y / 2 - pageLoading.Size.Y / 2);

            foreach (var e1 in _displayedBosses) e1.Dispose();
            _displayedBosses.Clear();

            FinishLoadingCmTrackerPanel(wndw, hPanel);
            pageLoading.Dispose();

            return hPanel;
        }

        private void RepositionTokens()
        {
            var pos = 0;
            foreach (var e in _displayedBosses)
            {
                var x = pos % 3;
                var y = pos / 3;
                e.Location = new Point(x * (e.Width + 8), y * (e.Height + 8));

                ((Panel)e.Parent).VerticalScrollOffset = 0;
                e.Parent.Invalidate();
                if (e.Visible) pos++;
            }
        }
        private void MousePressedSortButton(object sender, MouseEventArgs e)
        {
            var bSortMethod = (Control)sender;
            bSortMethod.Size = new Point(bSortMethod.Size.X - 4, bSortMethod.Size.Y - 4);
            CurrentSortMethod = bSortMethod.BasicTooltipText;
            foreach (var boss in _displayedBosses)
            {

                if (CurrentSortMethod == SORTBY_RAID && boss.Token.bossType == BossType.Raid)
                {
                    boss.Visible = true;
                }
                else if (CurrentSortMethod == SORTBY_STRIKES && boss.Token.bossType == BossType.Strike)
                {
                    boss.Visible = true;
                }
                else if (CurrentSortMethod == SORTBY_ALL)
                {
                    boss.Visible = true;
                }
                else
                {
                    boss.Visible = false;
                }
            }
            RepositionTokens();
        }


        private void MouseLeftSortButton(object sender, MouseEventArgs e)
        {
            var bSortMethod = (Control)sender;
            bSortMethod.Size = new Point(32, 32);

        }


        private void FinishLoadingCmTrackerPanel(WindowBase wndw, Panel hPanel)
        {
            /* ###################
            /      <HEADER>
            / ################### */
            var header = new Panel
            {
                Parent = hPanel,
                Size = new Point(hPanel.Width, 50),
                Location = new Point(0, 0),
                CanScroll = false
            };

            var accountPanel = new Panel
            {
                Parent = header,
                Size = new Point(340, 32),
                Location = new Point(header.Left + RIGHT_MARGIN, header.Location.Y),
                ShowTint = false
            };

            var accountLabel = new Label
            {
                Parent = accountPanel,
                Size = new Point(50, 32),
                Location = new Point(accountPanel.Left, accountPanel.Location.Y),
                Text = "Account"
            };

            accountDropDown = new Dropdown
            {
                Parent = accountPanel,
                Size = new Point(140, 32),
                Location = new Point(accountLabel.Right + RIGHT_MARGIN, accountPanel.Location.Y)
            };
            accountDropDown.SelectedItem = CURRENT_ACCOUNT.Value;
            foreach (string Item in _localSetting.GetAllAccounts())
            {
                accountDropDown.Items.Add(Item);
            }
            accountDropDown.ValueChanged += delegate (object sender, ValueChangedEventArgs args)
            {
                CURRENT_ACCOUNT.Value = args.CurrentValue;
                _displayedBosses.Clear();
                _myBossesClears = new Bosses(_localSetting.GetSettingForAccount(args.CurrentValue));
                SetupBossClears();
            };
            var sortingsMenu = new Panel
            {
                Parent = header,
                Size = new Point(140, 32),
                Location = new Point(header.Right - 140 - RIGHT_MARGIN, header.Location.Y),
                ShowTint = true
            };
            var bSortByAll = new Image
            {
                Parent = sortingsMenu,
                Size = new Point(32, 32),
                Location = new Point(RIGHT_MARGIN, 0),
                Texture = Content.GetTexture("255369"),
                BackgroundColor = Color.Transparent,
                BasicTooltipText = SORTBY_ALL
            };
            bSortByAll.LeftMouseButtonPressed += MousePressedSortButton;
            bSortByAll.LeftMouseButtonReleased += MouseLeftSortButton;
            bSortByAll.MouseLeft += MouseLeftSortButton;
            var bSortByRaid = new Image
            {
                Parent = sortingsMenu,
                Size = new Point(32, 32),
                Location = new Point(bSortByAll.Right + 20 + RIGHT_MARGIN, 0),
                Texture = _sortByRaidTexture,
                BasicTooltipText = SORTBY_RAID
            };
            bSortByRaid.LeftMouseButtonPressed += MousePressedSortButton;
            bSortByRaid.LeftMouseButtonReleased += MouseLeftSortButton;
            bSortByRaid.MouseLeft += MouseLeftSortButton;
            var bSortByStrikeProof = new Image
            {
                Parent = sortingsMenu,
                Size = new Point(32, 32),
                Location = new Point(bSortByRaid.Right + RIGHT_MARGIN, 0),
                Texture = _sortByStrikeTexture,
                BasicTooltipText = SORTBY_STRIKES
            };
            bSortByStrikeProof.LeftMouseButtonPressed += MousePressedSortButton;
            bSortByStrikeProof.LeftMouseButtonReleased += MouseLeftSortButton;
            bSortByStrikeProof.MouseLeft += MouseLeftSortButton;

            /* ###################
            /      </HEADER>
            / ###################
            / ###################
            /      <FOOTER>
            / ################### */
            var footer = new Panel
            {
                Parent = hPanel,
                Size = new Point(hPanel.Width, 50),
                Location = new Point(0, hPanel.Height - 50),
                CanScroll = false
            };


            _squadPanel = new Panel
            {
                Parent = hPanel,
                Size = new Point(header.Size.X, hPanel.Height - header.Height - footer.Height),
                Location = new Point(0, header.Bottom),
                ShowBorder = true,
                CanScroll = true,
                ShowTint = true
            };



            var clearCheckbox = new StandardButton()
            {
                Parent = hPanel,
                Size = new Point(200, 30),
                Location = new Point(_squadPanel.Location.X + _squadPanel.Width - 200 - RIGHT_MARGIN, _squadPanel.Location.Y + _squadPanel.Height + BOTTOM_MARGIN),
                Text = "Reset Weekly Clears",
                BasicTooltipText = ClearCheckboxTooltipText,
                Visible = true
            };



            Panel confirmPanel = new Panel
            {
                Parent = hPanel,
                Size = new Point(550, 100),
                Location = new Point(_squadPanel.Location.X + _squadPanel.Width - 550 - RIGHT_MARGIN, _squadPanel.Location.Y + _squadPanel.Height + BOTTOM_MARGIN),
                ShowBorder = false,
                CanScroll = false,
                ShowTint = false,
                Visible = false
            };

            var confirmText = new Label()
            {
                Parent = confirmPanel,
                Size = new Point(300, 30),
                Location = new Point(RIGHT_MARGIN, 5),
                Text = "Are you sure you want to rest weekly clears?",
                BackgroundColor = Color.Transparent,
            };


            var yesButton = new StandardButton()
            {
                Parent = confirmPanel,
                Size = new Point(50, 30),
                Location = new Point(confirmText.Right + RIGHT_MARGIN, confirmText.Location.Y),
                Text = "Yes"
            };
            var yesAllButton = new StandardButton()
            {
                Parent = confirmPanel,
                Size = new Point(130, 30),
                Location = new Point(yesButton.Right + RIGHT_MARGIN, confirmText.Location.Y),
                Text = "Yes/All Accounts"
            };

            var noButton = new StandardButton()
            {
                Parent = confirmPanel,
                Size = new Point(50, 30),
                Location = new Point(yesAllButton.Right + RIGHT_MARGIN, confirmText.Location.Y),
                Text = "No"
            };

            noButton.Click += delegate
            {
                clearCheckbox.Visible = true;
                confirmPanel.Visible = false;

            };
            yesButton.Click += delegate
            {

                foreach (var boss in _myBossesClears.Tokens)
                {
                    boss.setting.Value = false;
                }
                foreach (var BossButton in _displayedBosses)
                {
                    BossButton.Background = BossButton.Token.setting.Value ? Color.Green : Color.Black;
                }
                clearCheckbox.Visible = true;
                confirmPanel.Visible = false;
            };
            yesAllButton.Click += delegate
            {

                foreach (var boss in _myBossesClears.Tokens)
                {
                    boss.setting.Value = false;
                }
                foreach (var BossButton in _displayedBosses)
                {
                    BossButton.Background = BossButton.Token.setting.Value ? Color.Green : Color.Black;
                }
                _localSetting.ResetAllValues();
                clearCheckbox.Visible = true;
                confirmPanel.Visible = false;
            };

            clearCheckbox.Click += delegate
            {
                clearCheckbox.Visible = false;
                confirmPanel.Visible = true;

            };


            /* ###################
            /      </FOOTER>
            / ################### */
            contentPanel = new Panel
            {
                Parent = hPanel,
                Size = new Point(header.Size.X, hPanel.Height - header.Height - footer.Height),
                Location = new Point(0, header.Bottom),
                ShowBorder = true,
                CanScroll = true,
                ShowTint = true
            };

            SetupBossClears();


        }

        private void SetupBossClears()
        {
            if (_myBossesClears.Tokens != null)
            {

                contentPanel.ClearChildren();
                foreach (var boss in _myBossesClears.Tokens)
                {
                    var BossButton = new BossButton
                    {
                        Parent = contentPanel,
                        Icon = boss.Icon == null ? ContentsManager.GetTexture("icon_token.png") : ContentsManager.GetTexture(boss.Icon),
                        Font = Content.GetFont(ContentService.FontFace.Menomonia,
                            ContentService.FontSize.Size16, ContentService.FontStyle.Regular),
                        Text = boss.Name,
                        HighlightType = DetailsHighlightType.ScrollingHighlight,
                        Token = boss,
                        Background = boss.setting.Value ? Color.Green : Color.Black
                    };

                    BossButton.Click += delegate
                    {
                        boss.setting.Value = !boss.setting.Value;
                        BossButton.Background = boss.setting.Value ? Color.Green : Color.Black;

                    };

                    _displayedBosses.Add(BossButton);

                }
            }


            RepositionTokens();
        }


    }

}
