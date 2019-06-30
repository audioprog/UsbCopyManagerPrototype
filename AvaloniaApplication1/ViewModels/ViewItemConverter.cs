using Avalonia.Data.Converters;
using Avalonia.Media;
using AvaloniaApplication1.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace AvaloniaApplication1.ViewModels
{

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retrieve the format string and use it to format the value.
            string formatString = parameter as string;
            if (!string.IsNullOrEmpty(formatString))
            {
                return string.Format(culture, formatString, value);
            }

            return value.ToString();
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    class ViewItemButton : ReactiveObject
    {
        public int DbId { get; set; }
    }

    class LastViewItemObject : ViewItemObject
    {
        public LastViewItemObject(string newText, int newWidth, IBrush color, int dbId) : base(newText, newWidth, color)
        {
            DbId = dbId;
        }

        public int DbId { get; set; }
    }


    class ViewItemObject : ReactiveObject
    {
        public ViewItemObject(string newText, int newWidth, IBrush color)
        {
            text = newText;
            width = newWidth;
            BackgroundColor = color;
        }

        private string text;

        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }

        private int width;

        public int Width
        {
            get => width;
            set => this.RaiseAndSetIfChanged(ref width, value);
        }

        private IBrush background;

        public IBrush BackgroundColor
        {
            get => background;
            set => this.RaiseAndSetIfChanged(ref background, value);
        }
    }

    class ViewItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataRow row)
            {
                List<object> converted = new List<object>();
                IBrush color = Brushes.White;

                converted.Add(new ViewItemButton() { DbId = (int)row.ItemArray[0] });

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i < ServiceSettings.Fields.Count)
                    {
                        if (ServiceSettings.Fields[i] != "Id")
                        {
                            converted.AddRange(ConvertToText((int)row.ItemArray[0], row.ItemArray[i], i, ref color));
                        }
                    }
                }
            
                return converted;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static DivineServiceSettings ServiceSettings { get; set; }

        private ViewItemObject[] ConvertToText(int dbId, object obj, int i, ref IBrush color)
        {
            if (obj is string objString)
            {
                return new ViewItemObject[]
                {
                    i == ServiceSettings.Widths.Count - 1 ?
                    new LastViewItemObject(objString, ServiceSettings.Widths[i], color, dbId) :
                    new ViewItemObject(objString, ServiceSettings.Widths[i], color)
                };
            }
            else if (obj is DateTime dateTime)
            {
                color = dateTime.DayOfWeek == DayOfWeek.Sunday ? Brushes.LightBlue : Brushes.White;
                return new ViewItemObject[] { new ViewItemObject(dateTime.ToString("ddd "), ServiceSettings.Widths[i], color),
                    new ViewItemObject(dateTime.ToString("d "), ServiceSettings.Widths[i], color) };
            }
            return new ViewItemObject[] { new ViewItemObject(obj.ToString(), ServiceSettings.Widths[i], color) };
        }
    }
}
