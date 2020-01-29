using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlueBirdLauncherUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool steam_installed = false;
        public bool gtav_installed = false;
        private DispatcherTimer five_m_server_status_checker;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = string.Format("BlueBird RP Launcher v{0}", Help.version_number);

            //HideAllDSEs();
            //this.dseDiscover.Opacity = 100;
            //HideAllContentGrids();
            //this.gridDiscover.Visibility = Visibility.Visible;

            HideAllDSEs();
            this.dseFiveM.Opacity = 100;
            HideAllContentGrids();
            this.gridFiveM.Visibility = Visibility.Visible;

            five_m_server_status_checker = new DispatcherTimer();
            five_m_server_status_checker.Interval = TimeSpan.FromSeconds(1);
            five_m_server_status_checker.Tick += five_m_server_status_checker_Tick;
            five_m_server_status_checker.Start();

            //While debugging, switch to FiveM immediately

            var fivem_path = Help.GetFiveMAppDataPath();

            ////Check steam is installed
            //var steam_path = Help.GetPathToSteam();
            //if (!(steam_path == null))
            //{
            //    steam_installed = true;

            //    //Check GTAV is installed
            //    var GTAV_Path = Help.GetGTAVDirectory();
            //    if (!(GTAV_Path == null))
            //    {
            //        gtav_installed = true;


            //    }
            //    else
            //    {
            //        //Couldn't find GTA5 installed
            //    }
            //}
            //else
            //{
            //    //Couldn't find Steam installed
            //}



        }

        #region MainWindow Events
        private void btnDiscover_Click(object sender, RoutedEventArgs e)
        {
            HideAllDSEs();
            this.dseDiscover.Opacity = 100;

            HideAllContentGrids();
            this.gridDiscover.Visibility = Visibility.Visible;
        }

        private void btnFiveM_Click(object sender, RoutedEventArgs e)
        {
            HideAllDSEs();
            this.dseFiveM.Opacity = 100;

            HideAllContentGrids();
            this.gridFiveM.Visibility = Visibility.Visible;
        }

        private void btnRedM_Click(object sender, RoutedEventArgs e)
        {
            HideAllDSEs();
            this.dseRedM.Opacity = 100;

            HideAllContentGrids();
            this.gridRedM.Visibility = Visibility.Visible;
        }

        private void btnMinecraft_Click(object sender, RoutedEventArgs e)
        {
            HideAllDSEs();
            this.dseMinecraft.Opacity = 100;

            HideAllContentGrids();
            this.gridMinecraft.Visibility = Visibility.Visible;
        }

        private void btnRivals_Click(object sender, RoutedEventArgs e)
        {
            HideAllDSEs();
            this.dseRivals.Opacity = 100;

            HideAllContentGrids();
            this.gridRivals.Visibility = Visibility.Visible;
        }

        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            HideAllDSEs();
            this.dseMore.Opacity = 100;

            HideAllContentGrids();
            this.gridMore.Visibility = Visibility.Visible;
        }


        private void HideAllDSEs()
        {
            this.dseDiscover.Opacity = 0;
            this.dseFiveM.Opacity = 0;
            this.dseMinecraft.Opacity = 0;
            this.dseMore.Opacity = 0;
            this.dseRedM.Opacity = 0;
            this.dseRivals.Opacity = 0;
        }
        private void HideAllContentGrids()
        {
            this.gridDiscover.Visibility = Visibility.Hidden;
            this.gridFiveM.Visibility = Visibility.Hidden;
            this.gridRedM.Visibility = Visibility.Hidden;
            this.gridMinecraft.Visibility = Visibility.Hidden;
            this.gridRivals.Visibility = Visibility.Hidden;
            this.gridMore.Visibility = Visibility.Hidden;
        }
        #endregion


        #region GTA V
        private void btnGTAVJoinNow_Click(object sender, RoutedEventArgs e)
        {
            Help.ConnectToFiveMBlueBird();
            System.Threading.Thread.Sleep(3000);
        }

        private void btnGTAVClearCache_Click(object sender, RoutedEventArgs e)
        {
            var clear_cache_result = Help.ClearFiveMCache();
            if (clear_cache_result)
            {
                MessageBox.Show("FiveM Cache has been cleared successfully!", "Clear FiveM Cache", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("FiveM Cache has not been cleared.  Check the More tab for more details.", "Clear FiveM Cache", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnGTAVInstallAdditionalAssets_Click(object sender, RoutedEventArgs e)
        {
            var install_additional_assets = Help.GTAVInstallAdditionalAssets();
            if (install_additional_assets)
            {
                MessageBox.Show("Additional assets have been installed successfully!", "Install Additional Assets", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Additional assets have not been installed successfully!  Check the More tab for more details.", "Install Additional Assets", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void five_m_server_status_checker_Tick(object sender, EventArgs e)
        {
            five_m_server_status_checker.Interval = TimeSpan.FromSeconds(60);
            var fivem_server_status = Help.CheckFiveMServerPlayerCount();
            if (fivem_server_status.server_online)
            {
                this.lblFiveMCurrentServerStatus.Content = string.Format("Online {0}/{1} players", fivem_server_status.current_users, fivem_server_status.max_users);
            }
            else
            {
                this.lblFiveMCurrentServerStatus.Content = "Offline";
            }
            
        }

        #endregion

    }
}
