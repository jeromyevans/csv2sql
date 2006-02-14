// ----------------------------------------------------------------------------------------------------------
//
// Simple class to represent a single line of a CSV file
//
// Written by Jeromy Evans
// 
// History:
//  13 Jan 06 - modified to suppory multiple-line fields
//  14 Feb 06 - modified to accept an arraylist of CsvFields instead of strings
// ---[ Blue Sky Minds Pty Ltd ]-----------------------------------------------------------------------------\

using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;
using System.Data;

namespace com.blueskyminds
{
	
   public class CsvLine
   {
      /// <summary>
      /// comma separated values extracted on this line
      /// </summary>
      protected CsvField[] values;

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// instantiates a CSV line from a list of fields that have already been separated      
      /// </summary>      
      public CsvLine(ArrayList listOfFields)
      {                                        
         // convert the arraylist of values to the fixed array
         values = (CsvField[]) listOfFields.ToArray(typeof(CsvField));
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Gets the value at the specified index on the line and returns it as a string
      /// </summary>
      /// <param name="indexOnLine">starts from zero</param>
      /// <returns>a blank string if index out of bounds or no value</returns>
      public string getAsString(int indexOnLine)
      {
         if (indexOnLine < values.Length)
         {
            if (values[indexOnLine] != null)
            {
               return values[indexOnLine].Value;
            }
            else
            {
               return "";
            }
         }
         else
         {
            return "";
         }
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Gets the value at the specified index on the line, parsing it as a percent
      /// value (scale down by 100)
      /// </summary>
      /// <param name="indexOnLine">starts from zero</param>
      /// <returns>0.0 if not valid, otherwise the value</returns>
      public float getAsPercent(int indexOnLine)
      {
         float floatValue = 0.0F;
         string stringValue;
         Regex percentExp = new Regex(@"^-?\d+(\.\d+)*");
         Match match;

         if (indexOnLine < values.Length)
         {
            if (values[indexOnLine] != null)
            {
               try 
               {              
                  match = percentExp.Match(values[indexOnLine].RawValue);
                  if (match.Success == true)
                  {
                     stringValue = match.Value;
                                      
                     floatValue = float.Parse(stringValue);
                     floatValue = floatValue / 100.0F;
                  }                  
               }
               catch (Exception)
               {
               }
            }            
         }

         return floatValue;
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Gets the value at the specified index on the line, parsing it as a dollar
      /// value (optionally preceeded by the $ sign)
      /// </summary>
      /// <param name="indexOnLine">starts from zero</param>
      /// <returns>0.0 if not valid, otherwise the value</returns>
      public float getAsDollar(int indexOnLine)
      {
         float floatValue = 0.0F;
         string stringValue;
         
         Regex commaDollaExp = new Regex(@"[\$|,|\s]");
         Regex matchExp = new Regex(@"^\d+(\.\d+)*");  // 1 or more digits, optionally followed by . and one or more digits
         Match match;
         string textString;

         if (indexOnLine < values.Length)
         {
            if (values[indexOnLine] != null)
            {
               try 
               { 
                  // remove commas and $ in the value
                  textString = commaDollaExp.Replace(values[indexOnLine].RawValue, "");                  
                  match = matchExp.Match(textString);
                  if (match.Success == true)
                  {
                     stringValue = match.Value;
                                      
                     floatValue = float.Parse(stringValue);                     
                  }                  
               }
               catch (Exception)
               {
               }
            }            
         }

         return floatValue;
      }


      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Gets the value at the specified index on the line, parsing it as an integer
      /// value (optionally contining commas)
      /// </summary>
      /// <param name="indexOnLine">starts from zero</param>
      /// <returns>int.MinValue if not valid, otherwise the value</returns>
      public int getAsInteger(int indexOnLine)
      {
         int intValue = int.MinValue;
         string stringValue;
         
         Regex commaExp = new Regex(@"[,|\s]");
         Regex matchExp = new Regex(@"^\d+");
         Match match;
         string textString;

         if (indexOnLine < values.Length)
         {
            if (values[indexOnLine] != null)
            {
               try 
               { 
                  // remove commas in the value
                  textString = commaExp.Replace(values[indexOnLine].RawValue, "");                  
                  match = matchExp.Match(textString);
                  if (match.Success == true)
                  {
                     stringValue = match.Value;
                                      
                     intValue = int.Parse(stringValue);                     
                  }                  
               }
               catch (Exception)
               {
               }
            }            
         }

         return intValue;
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Returns true if the current line is non-blank
      /// </summary>      
      public bool NonBlank
      {
         get
         {
            if (values.Length > 0)
            {
               return true;
            }
            else
            {
               return false;
            }
         }
      }


      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Returns all the values on this line as an array of strings
      /// </summary>      
      public string[] getAsStrings()
      {     
         string[] stringValues = new string[values.Length];
         int index = 0;

         foreach (CsvField field in values)
         {
            stringValues[index++] = field.Value;
         }

         return stringValues;         
      }

      // ----------------------------------------------------------------------------------------------------
     

      // ----------------------------------------------------------------------------------------------------
	}
}
