// ----------------------------------------------------------------------------------------------------------
//
// Simple class to represent a single field on a line of a CSV file
//
// Written by Jeromy Evans
// 
// History:
//  14 Feb 06 - started
// ---[ Blue Sky Minds Pty Ltd ]-----------------------------------------------------------------------------\

using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;
using System.Data;

namespace com.blueskyminds
{
	   

   // ----------------------------------------------------------------------------------------------------

   /// <summary>
   /// Represents a single field with a CSV line and the metadata associated with that field about
   /// how it should be treated
   /// </summary>
   public class CsvField
   {
      protected string csvValue = null;

      protected bool quoteOutput = true;
      protected char quoteChar = '\'';  
      protected string quoteString = "\'";    
      protected string quoteCharEscaped = "\'\'"; 
      protected bool escapeOutput = true;           
      protected bool trimBeforeOutput = true;            

      protected bool blankIsNull = true;                

      protected const string NULL_VALUE = "null";

      // ----------------------------------------------------------------------------------------------------

      public CsvField(string value)
      {
         csvValue = value;
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// This property controls whether the value should be quoted on output
      /// </summary>
      public bool QuoteOutput
      {
         set
         {
            this.quoteOutput = value;
         }

         get
         {
            return this.quoteOutput;
         }
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// This property controls whether blanks should be returned as 'null'
      /// </summary>
      public bool BlankIsNull
      {
         set
         {
            blankIsNull = value;
         }

         get
         {
            return blankIsNull;
         }
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// This property controls whether blanks should be returned as ''
      /// </summary>
      public bool BlankIsBlank
      {
         set
         {
            blankIsNull = !value;
         }

         get
         {
            return (!blankIsNull);
         }
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// This property controls whether the output should be trimmed of whitespace
      /// </summary>
      public bool TrimBeforeOutput
      {
         set
         {
            trimBeforeOutput = value;
         }

         get
         {
            return trimBeforeOutput;
         }
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// This property controls whether the output should be escaped            
      /// </summary>
      public bool EscapeOutput
      {
         set
         {
            escapeOutput = value;
         }

         get
         {
            return escapeOutput;
         }
      }
      
      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Escape text for SQL insert
      /// </summary>
      /// <param name="textToEscape"></param>
      /// <returns></returns>
      protected string escape(string textToEscape)
      {         
         return textToEscape.Replace(quoteString, quoteCharEscaped);
      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Returns the current value in the specified format
      /// </summary>
      public string Value
      {
         get
         {
            string outputValue;
            bool finishedProcessing = false;

            // get the current value and trim whitespace if enabled to do so
            if (trimBeforeOutput)
            {
               outputValue = csvValue.Trim();
            }
            else
            {
               outputValue = csvValue;
            }
                              
            // determine if the length is zero...
            if (outputValue.Length == 0)
            {
               // it's a blank
               if (blankIsNull)
               {
                  outputValue = NULL_VALUE;
                  // value has been set - do not process further
                  finishedProcessing = true;
               }                     
            }   
            
            if (!finishedProcessing)
            {
               if (QuoteOutput)
               {
                  if (EscapeOutput)
                  {
                     // escape and quote the value
                     outputValue = quoteChar+escape(outputValue)+quoteChar;
                  }
                  else
                  {
                     // quote, but don't escape the value (dangerous really)
                     outputValue = quoteChar+outputValue+quoteChar;
                  }
               }
               else
               {
                  // don't quote the value
               }
            }

            return outputValue;
         }

         set
         {
            this.csvValue = value;
         }

      }

      // ----------------------------------------------------------------------------------------------------

      /// <summary>
      /// Returns the current value as a string without formatting/processing
      /// </summary>
      public string RawValue
      {
         get
         {            
            return csvValue;
         }        
      }
   }
}

      