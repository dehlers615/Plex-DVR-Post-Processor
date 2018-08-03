using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;

namespace PlexDVRPostProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:  Capture errors out of ffmpeg and log them
            //TODO:  Make everything in this process configurable in the app.config
            //Things to make configurable: tempDirectory, output file extension, ffmpeg executable path, ffmpeg parameters, log file name and location

            //We can't do anything without any arguments, so make sure we have one
            if (args.Count() > 0)
            {
                
                //Plex sends in the file name with path as the first argument
                var fileToProcess = args[0];
                var finalFileName = "";

                //Obtain the path of the file being processed
                var fileDirectory = Path.GetDirectoryName(fileToProcess);

                //Create a temp directory
                var tempDirectory = string.Concat(fileDirectory, @"\TempProcessing\");
                var exceptionMessage = "";
                var exceptionStackTrace = "";

                //Set the start time so we can track how long it takes to process this file
                var startTime = DateTime.UtcNow;

                try
                {
                    //The temp directory should never exist because it is deleted at the end, but just in case, check if it exists
                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }

                    //Get the filename of the file being processed without the path
                    var fileNameToProcess = Path.GetFileName(fileToProcess);

                    //Swap out the .ts with .mp4 but keep the same file name otherwise
                    var outputFile = string.Concat(tempDirectory, fileNameToProcess).Replace(".ts", ".mp4");

                    //Call ffmpeg in a new process
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = @"""C:\Program Files\ffmpeg\bin\ffmpeg.exe""";
                    proc.StartInfo.Arguments = string.Format(@"-i ""{0}"" -s hd720 -c:v libx264 -preset veryfast -vf yadif -c:a copy ""{1}""", fileToProcess, outputFile);
                    proc.StartInfo.RedirectStandardError = false;
                    proc.StartInfo.RedirectStandardOutput = false;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = false;
                    proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                    //Start the ffmpeg process and wait for it to complete
                    proc.Start();
                    proc.WaitForExit();

                    //Move the new file to the original location, delete the old file, and delete the temp directory
                    finalFileName = fileToProcess.Replace(".ts", ".mp4");
                    File.Move(outputFile, finalFileName);
                    File.Delete(fileToProcess);
                    Directory.Delete(tempDirectory, true);
                }
                catch (Exception ex)
                {
                    //if an exception was throw, then we want to log it
                    exceptionMessage = ex.Message;
                    exceptionStackTrace = ex.StackTrace;
                }

                finally
                {

                    //Add to the log file...
                    var totalProcessingTime = DateTime.UtcNow - startTime;
                    var logfile = string.Concat(@"E:\Video\", "VideoPostProcessingLog.txt");
                    if (!File.Exists(logfile))
                    {
                        File.Create(logfile);
                    }

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
                    {
                        file.WriteLine("");
                        file.WriteLine("******************************************************");
                        file.WriteLine(string.Format("Processing Start: {0}", startTime.ToString()));
                        file.WriteLine(string.Format("Input Video File: {0}", fileToProcess));
                        file.WriteLine(string.Format("Output Video File: {0}", finalFileName));
                        file.WriteLine(string.Format("Total Processing Time: {0} seconds", totalProcessingTime.TotalSeconds));

                        if (string.IsNullOrEmpty(exceptionMessage))
                        {
                            file.WriteLine(string.Format("Status: {0}", "Success"));
                        }

                        else
                        {
                            file.WriteLine(string.Format("Status: {0}", "Error"));
                            file.WriteLine(string.Format("Exception Message: {0}", exceptionMessage));
                        }

                        if (string.IsNullOrEmpty(exceptionStackTrace))
                        {
                            file.WriteLine(string.Format("Exception Stacktrace: {0}", exceptionStackTrace));
                        }


                        file.WriteLine("******************************************************");
                        file.WriteLine("");
                    }
                }
            }
        }
    }
}
