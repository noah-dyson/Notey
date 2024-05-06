using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Preview.Notes;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
using Microsoft.UI.Xaml.Media;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Notey
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        List<Note> openNotes = new List<Note>();

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

                string currentFile = @"D:\Dev\Windows Apps\Notey\Notes\" + fileName;

                string fileContent = await File.ReadAllTextAsync(currentFile, System.Text.Encoding.UTF8);
                
                Note note = new Note(fileName, fileContent);

                NotesTabs.TabItems.Add(note);
                for (int i = 0; i < NotesTabs.TabItems.Count; i++)
                {
                    if (NotesTabs.TabItems[i] == note)
                    {
                        NotesTabs.SelectedItem = NotesTabs.TabItems[i];
                    }
                }
            }
        }

        private async void NotesTabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            await File.WriteAllTextAsync(@"D:\Dev\Windows Apps\Notey\Notes\" + ((Note)args.Item).Name, ((Note)args.Item).Content);

            NotesTabs.TabItems.Remove(args.Item);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Note note)
            {
                note.Content = textBox.Text;
            }
        }
    }

    public class Note
    {         
        public string Name { get; set; }
        public string Content { get; set; }

        public Note(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
