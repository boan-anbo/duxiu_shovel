using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using Serilog;

namespace DuxiuShovel.Zipper
{
    public class DuxiuUnzipper
    {
        private static Encoding encoding;

        public DuxiuUnzipper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding("GBK");
        }

        public void UnzipOne(string filePath, string rootTargetFolder)
        {
            Directory.CreateDirectory(rootTargetFolder);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var absolutePath = Path.GetFullPath(filePath);
            var specificTargetFolder = Path.GetFullPath(rootTargetFolder + Path.DirectorySeparatorChar + fileName);

            for (int i = 0; i < Passwords.allPasswords.Length; i++)
            {
                var currentPassword = Passwords.allPasswords[i];
                try
                {
                    UnzipOneRaw(absolutePath, currentPassword, specificTargetFolder);
                    if (!String.IsNullOrEmpty(currentPassword))
                    {
                        Log.Logger.Information($"Extracted {filePath} with Password \"{currentPassword}\"");
                    }
                    break;
                }
                catch (ZipException e)
                {
                    // if it's the last password that failed
                    if (i == Passwords.allPasswords.Length - 1)
                    {
                        Log.Logger.Error($"None of the Passwords worked for \"{absolutePath}\".");
                    }

                    continue;
                }
            }
        }

        public void UnzipOneRaw(string filePath, string password, string resultFolder)
        {
            try
            {
                Extract.ExtractZipContent(filePath, password, resultFolder);
                var extractedFiles = Directory.GetFiles(resultFolder);
                var extractedDirectories = Directory.GetDirectories(resultFolder);
                Log.Logger.Information(
                    $"{extractedFiles.Length} Files & {extractedDirectories.Length} Directories Extracted From \"{filePath}\" To \"{resultFolder}\"");
            }
            catch (EndOfStreamException e)
            {
                Log.Logger.Error($"Archive File Broken \"{filePath}\"; {e.Message}");
            }
            catch (ZipException e)
            {
                // Log.Logger.Error($"Unzipping Failure: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                throw;
            }
        }

        public void UnzipMultiple(string[] fileNames, string targetFolder)
        {

            for (int i = 0; i < fileNames.Length; i++)
            {
                UnzipOne(fileNames[i], targetFolder);
                
            }
        }

        public void UnzipDirectory(string rootDirectory)
        {
            var files = Directory.GetFiles(rootDirectory, "*.zip");
            UnzipMultiple(files, rootDirectory);
        }
    }
}