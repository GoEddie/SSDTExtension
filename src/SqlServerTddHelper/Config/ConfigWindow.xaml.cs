using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

//todo - this would be better as a config page when i have time to implement that.
namespace GoEddieUk.SqlServerTddHelper.Config
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private readonly Settings _settings;

        public ConfigWindow(Settings settings)
        {
            _settings = settings;
            InitializeComponent();

            connection_string_textbox.Text = _settings.ConnectionString;
            use_configured_values_for_sql_cmd_checkbox.IsChecked = _settings.UseDefaultSqlCmdValues;
            deploy_folder_textbox.Text = _settings.DeploymentFolder;
        }

        public bool Cancelled { get; set; }

        public Settings GetSettings()
        {
            
            return _settings;
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            this.Close();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            _settings.ConnectionString = connection_string_textbox.Text;
            _settings.UseDefaultSqlCmdValues = use_configured_values_for_sql_cmd_checkbox.IsChecked.Value;
            _settings.DeploymentFolder = deploy_folder_textbox.Text;
            

            CheckOutputFolderExists();
            this.Close();

        }

        private void CheckOutputFolderExists()
        {
            Directory.CreateDirectory(_settings.DeploymentFolder);
        }
    }
}
