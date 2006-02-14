// ----------------------------------------------------------------------------------------------------------
//
// Simple class to read a CSV file into an object.  the file is read into an
// array of CsvLine objects
//
// Written by Jeromy Evans
// 
// History:
//  13 Jan 06 - modified to suppory multiple-line fields
//  14 Feb 06 - created a CsvField subclass that contains metadata about the field.  This is the first step
//   towards allowing metadata to be specified for columns that affect how the data is handled
//   (eg. blanks are blank or blanks are nulls, on a column by column basis)
// ---[ Blue Sky Minds Pty Ltd ]-----------------------------------------------------------------------------


using System;
using System.IO;
using System.Text;
using System.Collections;

namespace com.blueskyminds
{
	/// <summary>
	/// Summary description for CSVFile.
	/// </summary>
	public class CsvTextReader
	{
      enum State {IN_TEXT, IN_QUOTE};

      // ----------------------------------------------------------------------------------------------------
      /// <summary>
      /// Array of lines in the file
      /// </summary>
      protected CsvLine[] csvLines = null;

      /// <summary>
      /// Current index into the CSVlines
      /// </summary>
      protected int lineIndex = -1;

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Instantiates the reader, reads the entire file into memory and makes each line 
      /// available for parsing
      /// </summary>
      /// <param name="fileName"></param>
		public CsvTextReader(string fileName)
		{
			string content = readTextFile(fileName);

         if (content.Length > 0)
         {
            parseCSV(content);
         }
		}

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// reads an entire text file into memory for parsing
      /// </summary>
      /// <param name="fileName"></param>
      /// <returns>a string containing the entire content</returns>
      protected static string readTextFile(string fileName)
      {
         string content = "";
         StreamReader sr = null;

         try 
         {
            sr = File.OpenText(fileName);
            content = sr.ReadToEnd();
         }
         catch (Exception)
         {
         }
         finally 
         {
            if (sr != null)
            {
               sr.Close();
            }
         }
         return content;
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// moves to the next line in the csv buffer.  returns true if successful
      /// </summary>
      /// <returns></returns>
      public bool Read()
      {
         if (csvLines != null)
         {
            lineIndex++;   
            if (lineIndex < csvLines.Length)
            {
               return true;
            }
            else
            {
               lineIndex--;  // undo - keep in bounds
               return false;
            }
         }
         else
         {
            return false;
         }
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// This inner class implements a state machine to parse a text file character
      /// by character and convert it into the array of CSV lines
      /// </summary>
      protected class CSVParser
      {

         // ----------------------------------------------------------------------------------------------------

         /// <summary>
         /// Array of characters to be parsed
         /// </summary>
         protected char[] charArray;

         // ----------------------------------------------------------------------------------------------------

         /// <summary>
         /// List of the CSVLines created from this document
         /// </summary>
         protected ArrayList listOfCsvLines;         

         // ----------------------------------------------------------------------------------------------------
         
         const char QUOTE_CHAR = '\"';
         const char ESCAPE_CHAR = '\\';
         const char EOL_CHAR1 = '\n';
         const char EOL_CHAR2 = '\r';
         const char SEPARATOR = ',';         

         // ----------------------------------------------------------------------------------------------------
         /// <summary>
         /// The content of the current field being assembled
         /// </summary>
         protected StringBuilder thisFieldString;

         // ----------------------------------------------------------------------------------------------------
         /// <summary>
         /// The list of fields found on the current line
         /// </summary>
         protected ArrayList fieldsOnThisLine;

         // ----------------------------------------------------------------------------------------------------

         /// <summary>
         /// Extracts the field for the current position and returns it as a string
         /// </summary>
         /// <returns>The field value that was added</returns>
         protected CsvField extractThisField()
         {
            string thisValue = thisFieldString.ToString().Trim();
            CsvField csvField = new CsvField(thisValue);
                               
            // special exception check - a blank value in quotations is converted
            // to a single quotation (parser first treats as an escaped quote)
            // this converts back to a blank.
            if (thisValue.Equals("\""))
            {
               csvField.Value = "";
            }
            else
            {
               if (thisValue.Equals("null"))
               {
                  csvField.QuoteOutput = false;
               }
               else
               {
                  if (thisValue.Equals("NOW()"))
                  {
                     csvField.QuoteOutput = false;
                  }
               }
            }

            return csvField;
         }

         // ----------------------------------------------------------------------------------------------------
         /// <summary>
         /// Adds the specified field value to this list for the current line
         /// </summary>
         protected void storeThisField(CsvField csvField)
         {
            fieldsOnThisLine.Add(csvField);
         }

         // ----------------------------------------------------------------------------------------------------
         /// <summary>
         /// Resets the current field value
         /// </summary>
         protected void resetThisField()
         {
            thisFieldString.Length = 0;
         }

         // ----------------------------------------------------------------------------------------------------
         /// <summary>
         /// Resets the current line
         /// </summary>
         protected void resetThisLine()
         {
            fieldsOnThisLine.Clear();
         }

         // ----------------------------------------------------------------------------------------------------
         /// <summary>
         /// remembers the current line.
         /// </summary>
         protected void storeThisLine()
         {            
            listOfCsvLines.Add(new CsvLine(fieldsOnThisLine));
         }

         // ----------------------------------------------------------------------------------------------------

         public CSVParser(char[] charArray)
         {
            this.charArray = charArray;
            
            doParse();
         }

         // ----------------------------------------------------------------------------------------------------

         /// <summary>
         /// Performs the parsing of the character array
         /// Loops through character by character, adding each char to a stringBuffer to form
         /// field, and adding each field to an arraylist to form a line.  
         /// The character-by-character parsing is necessary to detect
         /// quotes (to permit multi-line fields) and escape characters (escaped commas 
         /// and quotes)
         /// </summary>
         protected void doParse()
         {            
            int currentPos = 0;
            bool keepThisChar;            
            bool parsed;            
            CsvField thisFieldValue;
            bool inQuote = false;                 
            int totalChars = charArray.Length;
            char thisChar;

            listOfCsvLines = new ArrayList();
            thisFieldString = new StringBuilder();
            fieldsOnThisLine = new ArrayList(1000);

            // loop character by character...
            while (currentPos < totalChars)
            {                        
               thisChar = charArray[currentPos];

               keepThisChar = true;               
                  
               // determine what this character is...
               switch (thisChar)
               {
                  /*case ESCAPE_CHAR:
                     // the escape char means the next character is part of
                     // the text. This is important if its a comma, quote etc

                     // preview the next char
                     if (currentPos < charArray.Length-1)
                     {  
                        // the next char is escaped so it becomes part of the text
                        // instead of the escape char
                        thisChar = charArray[currentPos+1];                                          

                        // LEAP AHEAD one character - it's escaped, so it doesn't need
                        // to be parsed (this simplies the parser because it doesn't
                        // encounter an escaped comma, quote, etc
                        currentPos++;
                     }
                     else
                     {
                        // at the end of the document...the character is part of the last field
                     }
                     break;*/
                  case QUOTE_CHAR:        
          
                     // the quote char may indicate the start of quoted text
                     // two quotes in a row indicate an escaped quote char

                     parsed = false;

                     // preview the next char to see if it's a quote too
                     if (currentPos < charArray.Length-1)
                     {  
                        if (charArray[currentPos+1] == QUOTE_CHAR)
                        {                           
                           // the next char is a quotation too...this is text and 
                           // needs to be retained (exception: if this is a blank string
                           // in quotations)

                           // LEAP AHEAD one character - it's escaped, so it doesn't need
                           // to be parsed (this simplifies the parser because it doesn't
                           // need to look back when it hits the next quote)
                           currentPos++;
                           parsed = true;
                        }
                     }

                     if (!parsed)
                     {                     
                        // unescaped quotes allow fields to extend over multiple lines
                        if (inQuote)
                        {
                           // this is the end of the current quote
                           inQuote = false;
                        }
                        else
                        {
                           // this is the start of a quote                         
                           inQuote = true;
                        }
                        
                        // don't use this character in the field's value
                        keepThisChar = false;                        
                     }
                     
                     break;

                  case SEPARATOR :
                     if (!inQuote)
                     {
                        // the unquoted separator character marks the end of a field
                        thisFieldValue = extractThisField();
                        storeThisField(thisFieldValue);

                        // reset the current field's value
                        resetThisField();

                        // don't keep this character in the field's value
                        keepThisChar = false;
                     }
                     else
                     {
                        // this character is inside the quotes
                        // it's text
                        // (no action to perform for this other than the default below)
                     }
                     break;
                  case EOL_CHAR1 :
                  case EOL_CHAR2 :
                     if (!inQuote)
                     {
                        // this is the end of the current field and current csv line
                        thisFieldValue = extractThisField();
                     
                        // ensure this is not a blank line with blank value
                        if ((fieldsOnThisLine.Count == 0) && (thisFieldValue.RawValue.Length == 0))
                        {
                           // nothing on this line to process
                           // (also arrives here between \n\r)
                        }
                        else
                        {
                           // remember the field's value (blanks are permitted)                           
                           storeThisField(thisFieldValue);

                           // do something with the CSV line
                           storeThisLine();
                        }

                        // reset the current field's value                        
                        resetThisField();

                        // start a new CSV line
                        resetThisLine();

                        // don't keep this character in the field's value
                        keepThisChar = false;                                 
                     }
                     else
                     {
                        // the end-of-line is within a quotation...it will be retained as
                        // party of the value
                        // (no action to perform for this other than the default below)
                     }
                     break;
                  default:
                     // every other character will be keep and appended to the
                     // current field's value (trailing and leading spaces removed later)
                     // (no action to perform for this other than the default below)
                     break;
               }                      

               if (keepThisChar)
               {
                  // keep this char
                  thisFieldString.Append(thisChar);
               }

               currentPos++; 
            }

            // IMPORTANT: if the end of the document is reached and the last line
            // still contains information then it needs to be processed too
            thisFieldValue = extractThisField();            
            if (fieldsOnThisLine.Count > 0) 
            {
               storeThisField(thisFieldValue);
               storeThisLine();
            }
            else
            {
               // there's nothing on this line...but if the field contains something
               // then it may be a single-value line
               if (thisFieldValue.RawValue.Length > 0)
               {
                  storeThisField(thisFieldValue);
                  storeThisLine();
               }
            }
         }

         // ----------------------------------------------------------------------------------------------------

         /// <summary>
         /// Returns the list of CsvLine objects generated by the parser
         /// </summary>
         public CsvLine[] CsvLines
         {
            get
            {
               return (CsvLine[]) listOfCsvLines.ToArray(typeof(CsvLine));               
            }
         }
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// parses a string of comma deliminated text into a two dimensional array of values
      /// Each array row is a new line in the CSV.
      /// Each subArray is the list of values on that line.  This can be jagged (varying number of values)
      /// </summary>
      /// <param name="csvContent"></param>      
      protected void parseCSV(string csvContent)
      {                             
         // parser
         CSVParser csvParser = new CSVParser(csvContent.ToCharArray());          
         
         csvLines = csvParser.CsvLines;

         /*
         lines = csvContent.Split('\n');
         csvLines = new CsvLine[lines.Length];         
         for (int i = 0; i < lines.Length; i++)
         {
            if (lines[i].Length > 0)
            {
               csvLines[i] = new CsvLine(lines[i]);                      
            }
         } */        
      }

      // ----------------------------------------------------------------------------------------------------

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Gets the value at the specified index on the line and returns it as a string
      /// </summary>
      /// <param name="indexOnLine">starts from zero</param>
      /// <returns>a blank string if index out of bounds or no value</returns>
      public string getAsString(int indexOnLine)
      {
         if (lineIndex < csvLines.Length)
         {
            if (csvLines[lineIndex] != null)
            {
               return csvLines[lineIndex].getAsString(indexOnLine);
            }
         }
         return null;
      }


      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Gets the value at the specified index on the line, parsing it as a percent
      /// value (optionally including the % sign, and scale down by 100)
      /// </summary>
      /// <param name="indexOnLine">starts from zero</param>
      /// <returns>0.0 if not valid, otherwise the value</returns>
      public float getAsPercent(int indexOnLine)
      {
         if (lineIndex < csvLines.Length)
         {
            if (csvLines[lineIndex] != null)
            {
               return csvLines[lineIndex].getAsPercent(indexOnLine);
            }
         }
         return 0.0F;
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
         if (lineIndex < csvLines.Length)
         {
            if (csvLines[lineIndex] != null)
            {
               return csvLines[lineIndex].getAsDollar(indexOnLine);
            }
         }
         return 0.0F;
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Gets the value at the specified index on the line, parsing it as an integer
      /// value (optionally containing commas)
      /// </summary>
      /// <param name="indexOnLine">starts from zero</param>
      /// <returns>int.MinValue if not valid, otherwise the value</returns>
      public int getAsInteger(int indexOnLine)
      {
         if (lineIndex < csvLines.Length)
         {
            if (csvLines[lineIndex] != null)
            {
               return csvLines[lineIndex].getAsInteger(indexOnLine);
            }
         }
         return int.MinValue;
      }

      // ----------------------------------------------------------------------------------------------------

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Returns true if the current line contains text
      /// </summary>      
      public bool NonBlank
      {
         get
         {
            bool nonBlank = false;

            if (lineIndex < csvLines.Length)
            {
               if (csvLines[lineIndex] != null)
               {
                  nonBlank = csvLines[lineIndex].NonBlank;
               }
            }
            return nonBlank;
         }
      }  

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Returns all the values on the current line as an array of strings
      /// </summary>
      /// <returns></returns>
      public string[] getAsStrings()
      {
         string[] values = null;

         if (lineIndex < csvLines.Length)
         {
            if (csvLines[lineIndex] != null)
            {
               values = csvLines[lineIndex].getAsStrings();
            }
         }

         return values;
      }

      // ----------------------------------------------------------------------------------------------------

	}
}
