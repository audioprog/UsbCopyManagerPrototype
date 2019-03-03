using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace AvaloniaApplication1.ViewModels
{
    class TreeViewItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataRowCollection collection)
            {
                List<ExpandoObject> result = new List<ExpandoObject>();

                foreach (object test in collection)
                {
                    if (test is DataRow crow)
                    {
                        ExpandoObject resultRow = new ExpandoObject();

                        for (int i = 0; i < crow.ItemArray.Length; i++)
                        {
                            //if (i < ServiceSettings.Fields.Count)
                            //{
                            //    if (ServiceSettings.Fields[i] != "Id")
                            //    {
                            //        resultRow.TryAdd(ServiceSettings.Fields[i], ConvertToText(crow.ItemArray[i]));
                            //    }
                            //}
                        }

                        result.Add(resultRow);
                    }
                }

                return result;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
