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
using Blish_HUD.ArcDps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using falcon.cmtracker.Controls;
using falcon.cmtracker.Persistance;
using static Blish_HUD.GameService;
using Color = Microsoft.Xna.Framework.Color;

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
        private  ArcDpsChecker _arcDpsChecker;
        #endregion

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings)
        {
            var selfManagedSettings = settings.AddSubCollection("Managed Settings", false);


            W3_Keep_Construct = selfManagedSettings.DefineSetting("W3_Keep_Construct", false);
            W4_Cairn = selfManagedSettings.DefineSetting("W4_Cairn", false);
            W4_Mursaat_Overseer = selfManagedSettings.DefineSetting("W4_Mursaat_Overseer", false);
            W4_Samarog = selfManagedSettings.DefineSetting("W4_Samarog", false);
            W4_Deimos = selfManagedSettings.DefineSetting("W4_Deimos", false);
            W5_Soulless_Horror = selfManagedSettings.DefineSetting("W5_Soulless_Horror", false);
            W5_Dhuum = selfManagedSettings.DefineSetting("W5_Dhuum", false);
            W6_Conjured_Amalgamate = selfManagedSettings.DefineSetting("W6_Conjured_Amalgamate", false);
            W6_Twin_Largos = selfManagedSettings.DefineSetting("W6_Twin_Largos", false);
            W7_Adina = selfManagedSettings.DefineSetting("W7_Adina", false);
            W7_Sabir = selfManagedSettings.DefineSetting("W7_Sabir", false);
            W7_Qadim2 = selfManagedSettings.DefineSetting("W7_Qadim2", false);
            W6_Qadim = selfManagedSettings.DefineSetting("W6_Qadim", false);

            Strike_Aetherblade_Hideout = selfManagedSettings.DefineSetting("Strike_Aetherblade_Hideout", false);
            Strike_Xunlai_Jade_Junkyard = selfManagedSettings.DefineSetting("Strike_Xunlai_Jade_Junkyard", false);
            Strike_Kaineng_Overlook = selfManagedSettings.DefineSetting("Strike_Kaineng_Overlook", false);
            Strike_Harvest_Temple = selfManagedSettings.DefineSetting("Strike_Harvest_Temple", false);

            _autoClearEnable = settings.DefineSetting(
                "EnableAutoClears",
                 true,
                 "Enable Auto Clear",
                 "When enabled, Module will auto detect when you cleard the cm boss using ArcDps"
             );



        }


        #region Settings


        public static SettingEntry<bool> W3_Keep_Construct;
        public static SettingEntry<bool> W4_Cairn;
        public static SettingEntry<bool> W4_Mursaat_Overseer;
        public static SettingEntry<bool> W4_Samarog;
        public static SettingEntry<bool> W5_Soulless_Horror;
        public static SettingEntry<bool> W4_Deimos;
        public static SettingEntry<bool> W5_Dhuum;
        public static SettingEntry<bool> W6_Conjured_Amalgamate;
        public static SettingEntry<bool> W6_Twin_Largos;
        public static SettingEntry<bool> W7_Sabir;
        public static SettingEntry<bool> W7_Adina;
        public static SettingEntry<bool> W7_Qadim2;
        public static SettingEntry<bool> W6_Qadim;

        public static SettingEntry<bool> Strike_Aetherblade_Hideout;
        public static SettingEntry<bool> Strike_Xunlai_Jade_Junkyard;
        public static SettingEntry<bool> Strike_Kaineng_Overlook;
        public static SettingEntry<bool> Strike_Harvest_Temple;

        private SettingEntry<bool> _autoClearEnable;


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


        #endregion

        #region Controls

        private WindowTab _ClearsTab;
        private Panel _modulePanel;
        private List<BossButton> _displayedBosses;
        private Panel _squadPanel;

        #endregion


        #region Localization

        private string CmTrakcerTabName = "CM Tracker";

        private string SORTBY_ALL;
        private string SORTBY_STRIKES;
        private string SORTBY_RAID;
        private string VisitUsAndHelpText;
        private string ClearCheckboxTooltipText;

        private string CurrentSortMethod;

        #endregion

        #region Textures

        private Bosses _myBossesClears;

        #endregion

        protected override void Initialize()
        {
            _displayedBosses = new List<BossButton>();
            LoadTextures();
        }

        protected override async Task LoadAsync()
        {
            if (_myBossesClears != null) return;
            _myBossesClears = new Bosses();

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

            _arcDpsChecker = new ArcDpsChecker(GameService.ArcDps, GameService.Overlay, GameService.Gw2Mumble);
            _arcDpsChecker.ArcDpsTimedOut += (o, e) =>
            {
                Logger.Debug("Lost connexion to ArcDPS… Retrying.");
                _arcDpsChecker.Enable();
            };

            GameService.ArcDps.Common.Activate();
            _arcDpsChecker.Enable();

            // Base handler must be called
            base.OnModuleLoaded(e);
        }


        protected override void Update(GameTime gameTime)
        {
            _arcDpsChecker.Check(gameTime);
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            // Unload here
            _squadPanel?.Dispose();
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

            FinishLoadingCmTrackerPanel(wndw, hPanel, _myBossesClears);
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
                }else if(CurrentSortMethod == SORTBY_ALL)
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


        private void FinishLoadingCmTrackerPanel(WindowBase wndw, Panel hPanel, Bosses currentClears)
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
                    Size = new Point(450, 100),
                    Location = new Point(_squadPanel.Location.X + _squadPanel.Width - 450 - RIGHT_MARGIN, _squadPanel.Location.Y + _squadPanel.Height + BOTTOM_MARGIN),
                    ShowBorder = false,
                    CanScroll = false,
                    ShowTint = false,
                    Visible = false
                };

                var confirmText = new TextBox()
                {
                    Parent = confirmPanel,
                    Size = new Point(300, 30),
                    Location = new Point(RIGHT_MARGIN,  5),
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

                var noButton = new StandardButton()
                {
                    Parent = confirmPanel,
                    Size = new Point(50, 30),
                    Location = new Point(yesButton.Right + RIGHT_MARGIN, confirmText.Location.Y),
                    Text = "No"
                };
                confirmPanel.AddChild(confirmText);
                confirmPanel.AddChild(noButton);
                confirmPanel.AddChild(yesButton);

                noButton.Click += delegate
                {
                    clearCheckbox.Visible = true;
                    confirmPanel.Visible = false;
                 
                };
                yesButton.Click += delegate
                {

                        foreach (var boss in currentClears.Tokens)
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

                clearCheckbox.Click += delegate
                {
                    clearCheckbox.Visible = false;
                    confirmPanel.Visible = true;
            
                };


                /* ###################
                /      </FOOTER>
                / ################### */
                var contentPanel = new Panel
                {
                    Parent = hPanel,
                    Size = new Point(header.Size.X, hPanel.Height - header.Height - footer.Height),
                    Location = new Point(0, header.Bottom),
                    ShowBorder = true,
                    CanScroll = true,
                    ShowTint = true
                };
                if (currentClears.Tokens != null)
                {
                    foreach (var boss in currentClears.Tokens)
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
                            Background = boss.setting.Value  ?  Color.Green : Color.Black 
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
