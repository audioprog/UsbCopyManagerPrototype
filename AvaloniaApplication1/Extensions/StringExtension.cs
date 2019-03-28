using System;

namespace AvaloniaApplication1.Extensions
{
    public static class StringExtension
    {
        public static string Section(this string original, char splitter, int startIndex, int endIndex = -1)
        {
            int startChar = 0;
            if (startIndex < 0)
            {
                try
                {
                    int lastMatched = original.Length;
                    for (int i = 0; i > startIndex; i--)
                    {
                        int index = original.LastIndexOf(splitter, lastMatched - 1);
                        if (index > -1)
                        {
                            lastMatched = index;
                            startChar = index + 1;
                        }
                        else
                        {
                            startChar = 0;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    startChar = 0;
                }
            }
            else
            {
                try
                {
                    int lastMatched = -1;
                    for (int i = 0; i < startIndex; i++)
                    {
                        int index = original.IndexOf(splitter, lastMatched + 1);
                        if (index > -1)
                        {
                            lastMatched = index;
                            startChar = index + 1;
                        }
                        else
                        {
                            startChar = original.Length;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    startChar = original.Length;
                }
            }

            int endChar = original.Length - 1;
            if (endIndex < 0)
            {
                try
                {
                    int lastMatched = original.Length;
                    for (int i = -1; i > endIndex; i--)
                    {
                        int index = original.LastIndexOf(splitter, lastMatched - 1);
                        if (index > -1)
                        {
                            lastMatched = index;
                            endChar = index - 1;
                        }
                        else
                        {
                            endChar = 0;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    endChar = -1;
                }
            }
            else
            {
                try
                {
                    int lastMatched = -1;
                    for (int i = -1; i < endIndex; i++)
                    {
                        int index = original.IndexOf(splitter, lastMatched + 1);
                        if (index > -1)
                        {
                            lastMatched = index;
                            endChar = index - 1;
                        }
                        else
                        {
                            endChar = original.Length - 1;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    endChar = original.Length - 1;
                }
            }

            if (startChar == original.Length || endChar == -1)
            {
                return string.Empty;
            }
            if (startChar > endChar)
            {
                return string.Empty;
            }
            return original.Substring(startChar, endChar - startChar + 1);
        }
    }
}
