//-----------------------------------------------------------------------
// <copyright file="CommandLine.cs" company="Vasont Systems">
// Copyright (c) Vasont Systems. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Vasont.Inspire.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The command line interface class to retrieve parameters and values from a command line.
    /// </summary>
    public static class CommandLine
    {
        #region Private Properties
        /// <summary>
        /// Contains the parameters list.
        /// </summary>
        private static Dictionary<string, string> parameters;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the executable parameter path from the command line arguments.
        /// </summary>
        public static string ExecutablePath => Environment.GetCommandLineArgs()[0];

        /// <summary>
        /// Gets a list collection of command line arguments and values from the command line.
        /// </summary>
        public static Dictionary<string, string> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    parameters = new Dictionary<string, string>();
                    List<string> arguments = ParseCommandLine();

                    if (arguments != null && arguments.Count > 1)
                    {
                        string paramName = string.Empty;
                        string paramValue = string.Empty;

                        for (int index = 1; index < arguments.Count; index++)
                        {
                            if (!string.IsNullOrEmpty(arguments[index]))
                            {
                                // if the first character is a - or / then this is an operator parameter, and will be our name
                                if (arguments[index][0] == '-' || arguments[index][0] == '/')
                                {
                                    // if we already have a name yet...
                                    if (!string.IsNullOrWhiteSpace(paramName))
                                    {
                                        // a name was already set, so it must have just been an operator without a value parameter
                                        // add it to the dictionary and set this as our new parameter name
                                        parameters.Add(paramName, paramValue);
                                    }

                                    // set the new parameter name
                                    paramName = arguments[index].Substring(1);
                                }
                                else if (!string.IsNullOrWhiteSpace(paramName))
                                {
                                    // this is a value so add it
                                    parameters.Add(paramName, arguments[index]);
                                    paramName = string.Empty;
                                }
                                else
                                {
                                    paramValue = arguments[index];
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(paramName))
                        {
                            parameters.Add(paramName, string.Empty);
                        }

                        if (!string.IsNullOrWhiteSpace(paramValue))
                        {
                            parameters.Add("parameter", paramValue);
                        }
                    }
                }

                return parameters;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method is used to manually parse command line parameters because Windows internally interprets \ as escape characters.
        /// I find this rather buggy, but maybe necessary for them to support Unicode in paths. In any event, ending a path with \" breaks
        /// the built in command line parsing in Windows and .NET so we'll write our own parser... :-/
        /// </summary>
        /// <returns>Returns a list of parsed command line values.</returns>
        private static List<string> ParseCommandLine()
        {
            List<string> arguments = new List<string>();
            string cleanCommandLine = System.Text.RegularExpressions.Regex.Replace(Environment.CommandLine.Replace(Environment.NewLine, " ").Replace("\t", " "), @"\s+", " ");
            bool quoteOn = false;
            int i = 0;
            string quotedValue = string.Empty;
            string commandValue = string.Empty;

            while (i < cleanCommandLine.Length)
            {  
                if (cleanCommandLine[i] == ' ' && !quoteOn && !string.IsNullOrWhiteSpace(commandValue))
                {
                    // if quote is not on, and we reached end of command
                    arguments.Add(commandValue);
                    commandValue = string.Empty;
                }
                else if (cleanCommandLine[i] == '"')
                {
                    if (!quoteOn)
                    {
                        quoteOn = true;
                    }
                    else
                    {
                        quoteOn = false;
                        arguments.Add(quotedValue);
                        quotedValue = string.Empty;
                    }
                }
                else if (quoteOn)
                {
                    quotedValue += cleanCommandLine[i];
                }
                else if (cleanCommandLine[i] != ' ')
                {
                    commandValue += cleanCommandLine[i];
                }

                i++;
            }

            if (!string.IsNullOrWhiteSpace(commandValue))
            {
                arguments.Add(commandValue);
            }
            else if (!string.IsNullOrWhiteSpace(quotedValue))
            {
                arguments.Add(quotedValue);
            }

            return arguments;
        }
        #endregion
    }
}
