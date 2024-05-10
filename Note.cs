using System;
using System.Collections.Generic;

namespace Notey
{
    public class Note
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public Note(string title)
        {
            Title = title;
        }

        public void LoadContent(string content)
        {
            Content = content;
        }

        public static string DateOnlytoTitle(DateOnly date)
        {
            string title = date.ToString("yyyy/MM/dd");
            title = title.Replace("/", "-");
            title += ".md";
            return title;
        }
    }
}
