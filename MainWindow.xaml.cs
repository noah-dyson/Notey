using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Search;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Notey
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ReadFiles();
        }

        public async void ReadFiles()
        {
            StorageFolder mainFolder = await StorageFolder.GetFolderFromPathAsync(@"D:\Dev\Windows Apps\Notey\Notes");
            StorageFileQueryResult query = mainFolder.CreateFileQuery();
            var files = await query.GetFilesAsync();

            foreach (StorageFile file in files)
            {
                ListViewItem item = new ListViewItem();
                TextBlock name = new TextBlock();
                name.Text = file.Name;
                item.Content = name;
                NotesList.Items.Add(item);
            }
        }

        private async void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesList.SelectedItem != null)
            {
                string fileName = ((TextBlock)((ListViewItem)NotesList.SelectedItem).Content).Text; 
                StorageFolder mainFolder = await StorageFolder.GetFolderFromPathAsync(@"D:\Dev\Windows Apps\Notey\Notes");
                Debug.WriteLine("test");
                StorageFile currentFile = await mainFolder.GetFileAsync(fileName);

                string content = await FileIO.ReadTextAsync(currentFile);
                NoteContent.Text = content;
            }
        }
    }
}
