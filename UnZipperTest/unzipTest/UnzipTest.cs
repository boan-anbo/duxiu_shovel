using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuxiuShovel.Main;
using Xunit;
using DuxiuShovel.Zipper;
using Xunit.Abstractions;

namespace UnZipperTest;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;
    private Shovel _duxiuShovel = new Shovel();
    private string unencryptedFile = @"./unzipTest/unencrypted_file_one.zip";
    private string unencryptedFile_with_root_level_files = @"./unzipTest/unencrypted_file_with_root_level_files.zip";
    private string encryptedFile = @"./unzipTest/encrypted_file_one.zip";
    private string encryptedRarFile = @"./unzipTest/encrypted_file.rar";
    private string encryptedRarErrorFile = @"./unzipTest/encrypted_rar_error.rar";
    private string encryptedRarPasswordFile = @"./unzipTest/encrypted_rar_password.rar";
    private string encryptedFileUnknownPassword = @"./unzipTest/encrypted_file_one_with_invalid_password.zip";
    private string unencryptedFileBroken = @"./unzipTest/unencrypted_file_one_broken.zip";
    private string zipFolder = @"./unzipTest/";
    private string passwordFile = @"./unzipTest/passwords.txt";

    private string[] multipleFiles;

    private string result_folder = @"./unzipTest/result";

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        multipleFiles = new string[]
        {
            unencryptedFileBroken,
            encryptedFile,
            encryptedFileUnknownPassword,
            unencryptedFile
        };
    }

    [Fact]
    public void fixPassword()
    {
        var passwords = File.ReadAllLines(passwordFile);
        _testOutputHelper.WriteLine($"Passwords: {passwords.Length}");
        var uniquePasswordsSet = new HashSet<string>();
        foreach (var password in passwords)
        {
            uniquePasswordsSet.Add(password);
        }
        _testOutputHelper.WriteLine($"Unique: {uniquePasswordsSet.Count}");
        File.WriteAllLinesAsync("clean_passwords", uniquePasswordsSet);
            
    }

    [Fact]
    public void Should_Unzip_to_directory()
    {
        var relativePath = @"./unzipTest/unencrypted_file_one.zip";

        var result_folder = @"./unzipTest/result";


        _duxiuShovel.Zipper.UnzipOne(relativePath, result_folder);
        Assert.Equal(true, File.Exists(relativePath));
    }

    [Fact]
    public void Should_handle_broken_file()
    {
        _duxiuShovel.Zipper.UnzipOne(unencryptedFileBroken, result_folder);
    }
    
    [Fact]
    public void Should_unzip_encrypted_zip_file()
    {
        _duxiuShovel.Zipper.UnzipOne(encryptedFile, result_folder);
    }
    
    [Fact]
    public void Should_unzip_encrypted_rar_file()
    {
        _duxiuShovel.Zipper.UnzipOne(encryptedRarFile, result_folder);
    }
    
    [Fact]
    public void Should_unzip_encrypted_rar_error_file()
    {
        _duxiuShovel.Zipper.UnzipOne(encryptedRarErrorFile, result_folder);
    }
    
    [Fact]
    public void Should_unzip_encrypted_rar_password_file()
    {
        _duxiuShovel.Zipper.UnzipOne(encryptedRarPasswordFile, result_folder);
    }
    
    [Fact]
    public void Should_handle_encrypted_file_with_unknown_password()
    {
        _duxiuShovel.Zipper.UnzipOne(encryptedFileUnknownPassword, result_folder);
    }
    
    [Fact]
    public void Should_extract_unencrypted_file_with_root_level_file()
    {
        _duxiuShovel.Zipper.UnzipOne(unencryptedFile_with_root_level_files, result_folder);
    }

    [Fact]
    public void Should_extract_multiple_files()
    {
        _duxiuShovel.Zipper.UnzipMultiple(multipleFiles, result_folder);
    }

    [Fact]
    public void Should_unzip_directory()
    {
        _duxiuShovel.Zipper.UnzipDirectory(zipFolder);
    }

}