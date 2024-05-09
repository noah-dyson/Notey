using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Notey
{
    public class TemplateSelector : DataTemplateSelector
    {
        public DataTemplate NotesTemplate { get; set; }
        public DataTemplate TemplateMaker { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Note note)
            {
                if (note.Date.Day % 2 == 0)
                {
                    return NotesTemplate;
                }
                else
                {
                    return TemplateMaker;
                }
            }
            return base.SelectTemplateCore(item);
        }
    }
}
