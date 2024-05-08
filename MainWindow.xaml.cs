using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Search;
using System.Collections.Generic;

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
        List<Note> notes = new List<Note>();

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
                Note note = new Note(file.Name);
                notes.Add(note);
                NotesList.Items.Add(note);
            }
        }

        private async void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesList.SelectedItem != null)
            {
                Note note = (Note)NotesList.SelectedItem;

                for (int i = 0; i < openNotes.Count; i++)
                {
                    if (openNotes[i].Title == note.Title)
                    {
                        NotesTabs.SelectedItem = openNotes[i];
                        return;
                    }
                }

                string currentFile = @"D:\Dev\Windows Apps\Notey\Notes\" + note.Title;
                string fileContent = await File.ReadAllTextAsync(currentFile, System.Text.Encoding.UTF8);
                
                note.LoadContent(fileContent);
                openNotes.Add(note);
                NotesTabs.TabItems.Add(note);

                for (int i = 0; i < NotesTabs.TabItems.Count; i++)
                {
                    if (NotesTabs.TabItems[i] == note)
                    {
                        NotesTabs.SelectedItem = NotesTabs.TabItems[i];
                    }
                }   

                NotesTabs.UpdateLayout();
            }
        }

        private async void NotesTabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            await File.WriteAllTextAsync(@"D:\Dev\Windows Apps\Notey\Notes\" + ((Note)args.Item).Title, ((Note)args.Item).Content);

            openNotes.Remove((Note)args.Item);
            NotesTabs.TabItems.Remove(args.Item);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Note note)
            {
                note.Content = textBox.Text;
            }
        }

        private void SelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectedItem= sender.SelectedItem;
            int currentSelectedIndex = sender.Items.IndexOf(selectedItem);

            switch(currentSelectedIndex)
            {
                case 0:
                    NotesList.Visibility = Visibility.Visible;
                    AddView.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    NotesList.Visibility = Visibility.Collapsed;
                    AddView.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Calendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            foreach (Note note in notes)
            {
                if (note.Date == DateOnly.FromDateTime(args.Item.Date.Date))
                {
                    args.Item.IsBlackout = true;
                }
            }
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (Calendar.SelectedDates.Count > 0)
            {
                string filename = Note.DateOnlytoTitle(DateOnly.FromDateTime(Calendar.SelectedDates[0].Date));
                Note note = new Note(filename);
                File.Create(@"D:\Dev\Windows Apps\Notey\Notes\" + filename);
                notes.Add(note);
                openNotes.Add(note);
                NotesList.Items.Add(note);
                NotesTabs.TabItems.Add(note);
            }
        }
    }
}
