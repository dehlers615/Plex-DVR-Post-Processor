# Plex-DVR-Post-Processor
Simple application that Plex can call after a video is recorded by Plex DVR that will convert .ts to 720p .mp4.

# How to Use it
In Plex, assuming you are a Plex Pass subscriber, you have access to the DVR functionality.  After a show is successfully recorded and Plex removes commercials (if desired), Plex lets you configure it to call your own custom script or application.  Plex will pass in the video file's name (with path) as the first argument to your script or application.  To configure it to call this you just need to:
 - Log into Plex server
 - Go to settings
 - Go to Server
 - Go to Live TV & DVR
 - Click "DVR Settings"
 - In the "POSTPROCESSING SCRIPT" text box, enter your script name including its path (Example: E:\Video\PlexDVRPostProcessor.exe)

# Items to Change for Personal Use
If you want to build and use this, you should change the following:
 - ffmpeg executable path and logfile
 - Optional: tempDirectory, output file extension, and ffmpeg parameters
 
# Need Help?
If anyone ever sees this and wants it to be customizable, it can be done quite easily by moving the hardcoded variables and parameters in the application to be loaded from the app.config.  For my purposes, I just wrote this up real quick for my own use, but I could make it re-usable for others if anyone is interested.

# Dependencies
- ffmpeg: https://ffmpeg.org/
- .NET Framework 4.6.1

