using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Search;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;

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
            ReadFiles();
            this.InitializeComponent();
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

            }
        }

        private async void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
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

        }

        private void Calendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            Debug.WriteLine(DateOnly.FromDateTime(args.Item.Date.Date).ToString());
            if (args.Phase == 0)
            {
                args.RegisterUpdateCallback(Calendar_CalendarViewDayItemChanging);
            }
            else if (args.Phase == 1)
            {
                foreach (Note note in notes)
                {
                    if (note.Date == DateOnly.FromDateTime(args.Item.Date.Date))
                    {
                        args.Item.Background = (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
                    }
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
                NotesTabs.TabItems.Add(note);
            }
        }

        private async void Calendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count > 0)
            {
                foreach (Note note in notes)
                {
                    if (note.Date == DateOnly.FromDateTime(args.AddedDates[0].Date))
                    {
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
            }
        }

        private void TemplateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
