﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.ResultItems;

namespace James.Workflows.Outputs
{
    public class MagicOutput : BasicOutput
    {
        const char HORIZONTALSPLIT = '|';

        [ComponentField("Subtitle Format")]
        public string SubtitleFormat { get; set; } = "{0}";

        [ComponentField("Icon Format")]
        public string IconFormat { get; set; } = "{0}";

        [ComponentField("Auto close after execution")]
        public bool Hide { get; set; } = false;

        public override string GetDescription() => "Displays data as a SearchResult and allowes further components to be executed";

        /// <summary>
        /// Prepares the MagicResultItems and displays it on the search window
        /// </summary>
        /// <param name="output"></param>
        public override void Run(string[] output)
        {
            var outputResults = new List<MagicResultItem>();
            foreach (string text in output)
            {
                string[] splits = text.Split(HORIZONTALSPLIT);
                outputResults.Add(new MagicResultItem()
                {
                    Icon = GetIcon(IconFormat.InsertArguments(splits)),
                    Title = FormatString.InsertArguments(splits),
                    Subtitle = SubtitleFormat.InsertArguments(splits),
                    WorkflowComponent = this,
                    WorkflowArguments = splits
                });
            }
            if (outputResults.Count > 0)
            {
                Windows.MainWindow.GetInstance().searchResultControl.WorkflowOutput(outputResults);
            }
        }

        /// <summary>
        /// Loads the icon
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private BitmapImage GetIcon(string filePath)
        {
            if (!File.Exists(filePath))
            {
                filePath = ParentWorkflow.Path + "\\" + filePath;
            }
            if (File.Exists(filePath))
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        BitmapImage image = new BitmapImage(new Uri(filePath));
                        image.Freeze();
                       
                        return image;
                    }
                }
                catch (Exception e)
                {
                    return ParentWorkflow.Icon;
                }
            }
            return ParentWorkflow.Icon;
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}