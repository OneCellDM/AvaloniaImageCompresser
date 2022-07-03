using Avalonia.Data.Converters;

using AvaloniaImageCompress.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaImageCompress.Converters
{
    public class StatusToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = value is CompressedStatusEnum;
            if (status)
            {
                switch ((CompressedStatusEnum)value)
                {
                    case CompressedStatusEnum.Wait:
                        {
                            return "Ожидание";
                            
                        }
                    case CompressedStatusEnum.processed:
                        {
                            return "Обработка";
                          
                        }
                  
                    case CompressedStatusEnum.Finished:
                        {
                            return "Завершено";
                            
                        }
                    case CompressedStatusEnum.Error:
                        {
                            return "Произошла проблема";
                           
                        }
                }
            }
                return "";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
