using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace DetectPeaks
{
    class Program
    {
        static void Main(string[] args)
        {

             // Add this namespace for TextFieldParser

            // Initialize list to store data and x values
            List<double> data = new List<double>();
            List<double> xValues = new List<double>();

            // Specify the column indices for x and y
            int xColumn = 0;
            int yColumn = 1;


            using (TextFieldParser parser = new TextFieldParser(@"cordevibrante.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    try
                    {
                        double x = double.Parse(fields[xColumn]);
                        double y = double.Parse(fields[yColumn]);

                        xValues.Add(x);
                        data.Add(y);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        continue; // Skip to the next line of data
                    }
                }
            }

            // Set sliding window size and peak detection threshold
            int windowSize = 3;
            double peakThreshold = 0.0005;

            // Initialize list to store peak indices and groups
            List<int> peakIndices = new List<int>();
            List<List<int>> peakGroups = new List<List<int>>();

            // Loop over data points and detect peaks
            for (int i = 1; i < data.Count - 1; i++)
            {
                double left = data[i - 1];
                double middle = data[i];
                double right = data[i + 1];

                // Check if middle point is higher than its neighbors
                if (middle > left + peakThreshold && middle > right + peakThreshold)
                {

                    // Check if this peak belongs to an existing group
                    bool addedToGroup = false;
                    foreach (List<int> group in peakGroups)
                    {
                        int lastPeakIndex = group[group.Count - 1];
                        if (i - lastPeakIndex <= windowSize)
                        {
                            group.Add(i);
                            addedToGroup = true;
                            break;
                        }
                    }

                    // If this peak doesn't belong to an existing group, create a new group
                    if (!addedToGroup)
                    {
                        peakGroups.Add(new List<int> { i });
                    }

                    // Add this peak to the peak index list
                    peakIndices.Add(i);
                }
            }

            // Find the highest peak in each group
            List<int> highestPeaks = new List<int>();
            foreach (List<int> group in peakGroups)
            {
                int highestPeakIndex = group[0];
                double highestPeakValue = data[highestPeakIndex];
                for (int i = 1; i < group.Count; i++)
                {
                    int peakIndex = group[i];
                    double peakValue = data[peakIndex];
                    if (peakValue > highestPeakValue)
                    {
                        highestPeakIndex = peakIndex;
                        highestPeakValue = peakValue;
                    }
                }
                highestPeaks.Add(highestPeakIndex);
            }

            // Get x values corresponding to the highest peaks
            List<double> highestPeakXValues = new List<double>();
            foreach (int index in highestPeaks)
            {
                double xValue = xValues[index];
                highestPeakXValues.Add(xValue);
            }

            // Print the x values of the highest peaks in each group
            foreach (double xValue in highestPeakXValues)
            {
                Console.WriteLine(xValue);
            }



            Console.ReadKey();
        }
    }
}
