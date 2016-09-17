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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using csprojFix.Service;

namespace csprojFix {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        XML _xmlService;

        public MainWindow() {

            _xmlService = new XML();

            InitializeComponent();
        }

        private void load_Click(object sender, RoutedEventArgs e) {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.ShowDialog();
            path.Content = dialog.FileName;
        }

        private void find_Click(object sender, RoutedEventArgs e) {

            errors.Text = string.Empty;

            if (string.IsNullOrEmpty(path.Content.ToString())) {
                MessageBox.Show("Please choose csproj file.", "Error");
                return;
            }

            try {

                var directory = System.IO.Path.GetDirectoryName(path.Content.ToString());

                var xdoc = XDocument.Load(path.Content.ToString());

                if (duplicated.IsChecked.HasValue && duplicated.IsChecked.Value) {

                    var duplicatedFiles = _xmlService.FindDuplicatedContentFiles(xdoc);

                    if (duplicatedFiles.Any()) {
                        errors.AppendText("-- duplicated content files --");
                        errors.AppendText(Environment.NewLine);
                        errors.AppendText(Environment.NewLine);

                        foreach (var file in duplicatedFiles) {
                            errors.AppendText($"{file.Key} ({file.Value}x)");
                            errors.AppendText(Environment.NewLine);
                        }
                        errors.AppendText(Environment.NewLine);
                    }

                    duplicatedFiles = _xmlService.FindDuplicatedCompileFiles(xdoc);

                    if (duplicatedFiles.Any()) {
                        errors.AppendText("-- duplicated compile files --");
                        errors.AppendText(Environment.NewLine);
                        errors.AppendText(Environment.NewLine);

                        foreach (var file in duplicatedFiles) {
                            errors.AppendText($"{file.Key} ({file.Value}x)");
                            errors.AppendText(Environment.NewLine);
                        }
                        errors.AppendText(Environment.NewLine);
                    }

                }

                if (missingOnDisk.IsChecked.HasValue && missingOnDisk.IsChecked.Value) {

                    var missingFiles = _xmlService.FindMissingContentFilesOnDisk(xdoc, directory);

                    if (missingFiles.Any()) {
                        errors.AppendText("-- missing content files on disk --");
                        errors.AppendText(Environment.NewLine);
                        errors.AppendText(Environment.NewLine);

                        foreach (var file in missingFiles) {
                            errors.AppendText(file);
                            errors.AppendText(Environment.NewLine);
                        }
                        errors.AppendText(Environment.NewLine);
                    }

                    missingFiles = _xmlService.FindMissingCompileFilesOnDisk(xdoc, directory);

                    if (missingFiles.Any()) {
                        errors.AppendText("-- missing compile files on disk --");
                        errors.AppendText(Environment.NewLine);
                        errors.AppendText(Environment.NewLine);

                        foreach (var file in missingFiles) {
                            errors.AppendText(file);
                            errors.AppendText(Environment.NewLine);
                        }
                        errors.AppendText(Environment.NewLine);
                    }

                }

                if (string.IsNullOrEmpty(errors.Text)) {
                    errors.Text = "no errors found :)";
                }
            } catch (Exception) {
                MessageBox.Show("Error occured.", "Error");
            }

        }
    }
}
