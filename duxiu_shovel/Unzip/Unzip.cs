using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using Serilog;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace DuxiuShovel.Zipper
{
    public class DuxiuUnzipper
    {
        private static Encoding encoding;
        public string [] allAvailablePasswords;
        

        public DuxiuUnzipper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encoding = Encoding.GetEncoding("GBK");
            
            allAvailablePasswords = File.ReadAllLines("passwords.txt");
            // filter out empty lines
            var validPasswords = allAvailablePasswords.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (validPasswords.Length == 0)
            {
                Log.Error("No valid passwords found in passwords.txt");
                Environment.Exit(1);
            }
        }

        public string UnzipOne(string filePath, string rootTargetFolder)
        {
            Directory.CreateDirectory(rootTargetFolder);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var absolutePath = Path.GetFullPath(filePath);
            var specificTargetFolder = Path.GetFullPath(rootTargetFolder + Path.DirectorySeparatorChar + fileName);

            var correctPassword = String.Empty;

            for (int i = 0; i < allAvailablePasswords.Length; i++)
            {
                var currentPassword = allAvailablePasswords[i];
                try
                {
                    UnzipOneRaw(absolutePath, currentPassword, specificTargetFolder);
                    if (!String.IsNullOrEmpty(currentPassword))
                    {
                        Log.Logger.Information($"Extracted {filePath} with Password \"{currentPassword}\"");
                        correctPassword = currentPassword;
                    }

                    break;
                }
                catch (EndOfStreamException e)
                {
                    Log.Logger.Error($"Archive File Broken \"{filePath}\"; {e.Message}");
                    throw e;
                }
                catch (ZipException e)
                {
                    ThrowIfPasswordFailed(e, absolutePath, i);
                }
                catch (IndexOutOfRangeException e)
                {
                    ThrowIfPasswordFailed(e, absolutePath, i);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    ThrowIfPasswordFailed(e, absolutePath, i);
                }
            }
            
            return correctPassword;
        }

        private void ThrowIfPasswordFailed(Exception e, string absolutePath, int passwordIndex)
        {
            // if it's the last password that failed
            if (passwordIndex == allAvailablePasswords.Length - 1)
            {
                Log.Logger.Error($"None of the Passwords worked for \"{absolutePath}\".");
                throw e;
            }
        }

        private void UnzipOneRaw(string filePath, string password, string resultFolder)
        {
            switch (Path.GetExtension(filePath.ToLower()))
            {
                case ".zip":
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
                        // archive broken
                        throw e;
                    }
                    catch (ZipException e)
                    {
                        // Log.Logger.Error($"Unzipping Failure: {e.Message}");
                        throw e;
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error(e.Message);
                        throw;
                    }

                    break;
                case ".rar":
                    try
                    {
                        RarArchive rar = RarArchive.Open(filePath, new ReaderOptions
                        {
                            Password = password.Length > 0 ? password : "",
                        });

                        // if (rar == null)
                        // {
                        //     throw new Exception("Cannot open Rar Archive");
                        // }


                        Directory.CreateDirectory(resultFolder);
                        rar.WriteToDirectory(resultFolder, new ExtractionOptions
                        {
                            ExtractFullPath = true,
                            Overwrite = true,
                            PreserveFileTime = true,
                        });
                        rar.Dispose();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw e;
                    }

                    break;
                default:
                    throw new Exception("Unsupported file type");
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
            // get zip and rar files
            string[] extensions = { ".zip", ".rar" };
            string[] files = Directory.GetFiles(rootDirectory, "*.*")
                .Where(f => extensions.Contains(new FileInfo(f).Extension.ToLower())).ToArray();
            UnzipMultiple(files, rootDirectory);
        }
    }
}