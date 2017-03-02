using System;
using System.Windows;
using Microsoft.Win32;

using System.IO;

namespace RepCalcGUI
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private string LastSaveName = string.Empty;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void GoButton_OnClick(object sender, RoutedEventArgs e)
    {
      var repMax = 0.0;
      
      try
      {
         repMax = Convert.ToInt32(RepMax.Text);
      }
      catch (FormatException)
      {
        MessageBox.Show(this, $"Your max rep count should be a number!\n\nYou entered: {RepMax.Text}", "Invalid Input",
          MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var eightRepWeight = repMax * 0.75;
      var fiveRepWeight = repMax * 0.85;
      var threeRepWeight = repMax * 0.90;

      OutRepM1W1.Text = $"{eightRepWeight}";
      OutRepM1W2.Text = $"{eightRepWeight+2.5}";
      OutRepM1W3.Text = $"{eightRepWeight+5.0}";
      OutRepM1W4.Text = $"{eightRepWeight+7.5}";

      OutRepM2W1.Text = $"{fiveRepWeight}";
      OutRepM2W2.Text = $"{fiveRepWeight+2.5}";
      OutRepM2W3.Text = $"{fiveRepWeight+5.0}";
      OutRepM2W4.Text = $"{fiveRepWeight+7.5}";

      OutRepM3W1.Text = $"{threeRepWeight}";
      OutRepM3W2.Text = $"{threeRepWeight+2.5}";
      OutRepM3W3.Text = $"{threeRepWeight+5.0}";
      OutRepM3W4.Text = $"{threeRepWeight+7.5}";

      if (PrependDate.IsChecked.GetValueOrDefault())
        Save();
    }

    private void Browse_OnClick(object sender, RoutedEventArgs e)
    {
      Browse();
    }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
      if (LastSaveName == string.Empty)
        Browse();
      else
        Save();
    }

    private void Browse()
    {
      var dialog = new SaveFileDialog
      {
        AddExtension = true,
        CheckFileExists = false,
        CheckPathExists = true,
        CreatePrompt = false,
        DefaultExt = "log",
        DereferenceLinks = false,
        OverwritePrompt = true
      };

      var res = dialog.ShowDialog(this);
      if (res.GetValueOrDefault())
      {
        LastSaveName = SavePathText.Text = dialog.FileName;
        Save();
      }
    }

    private void Save()
    {
      var savePath = LastSaveName;

      // If the savePath is not valid
      if (savePath == string.Empty)
      {
        // Offer to select a file to save to
        Browse();

        // If that still failed, exit
        if (savePath == string.Empty) return;
      }

      // We need to pre-pend the date , but we need to pull the file name apart first
      if (PrependDate.IsChecked.GetValueOrDefault())
      {
        var file = Path.GetFileName(savePath);
        var path = Path.GetDirectoryName(savePath);

        savePath = $"{path}{Path.DirectorySeparatorChar}{DateTime.Now:yyyy-MM-dd} {file}";
      }

      try
      {
        using (var f = File.CreateText(savePath))
        {
          f.Write($@"{RepMax.Text}
{OutRepM1W1.Text},{OutRepM2W1.Text},{OutRepM3W1.Text}
{OutRepM1W3.Text},{OutRepM2W3.Text},{OutRepM3W3.Text}
{OutRepM1W2.Text},{OutRepM2W2.Text},{OutRepM3W2.Text}
{OutRepM1W4.Text},{OutRepM2W4.Text},{OutRepM3W4.Text}");
        }

        MessageBox.Show(this, $"Saved successfully!\n\n{savePath}", "Saved!", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      catch (UnauthorizedAccessException)
      {
        MessageBox.Show(this, $"Access denied when attempting to write to file.\n\n{savePath}", "Unauthorized File Access", MessageBoxButton.OK,
          MessageBoxImage.Error);
      }
      catch (PathTooLongException)
      {
        // Save path was too long!
        if (MessageBox.Show(this, $"That path is too long!  Please choose a folder closer to the drive root.\n\n{savePath}", "Path Too Long",
          MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
          Browse();
      }
      catch (DirectoryNotFoundException)
      {
        // The path no longer exists
        Browse();
      }
    }
  }
}
