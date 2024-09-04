namespace pharmace.Helper;
using System.Text.RegularExpressions;



public static class DocumentSettings
{
    public static string UploadFile(IFormFile file, string folderName, string newFileName)
    {
        string folderPath = Path.Combine("D:\\MEDIA", folderName);

        string extension = Path.GetExtension(file.FileName);

        // Sanitize the newFileName
        string sanitizedFileName = SanitizeFileName(newFileName);

        string fileName = $"{Guid.NewGuid()}{sanitizedFileName}{extension}";

        string filePath = Path.Combine(folderPath, fileName);

        using var fs = new FileStream(filePath, FileMode.Create);
        file.CopyTo(fs);

        return fileName;
    }

    private static string SanitizeFileName(string fileName)
    {
        // Remove invalid characters
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

        return Regex.Replace(fileName, invalidRegStr, "_");
    }

    public static void DeleteFile(string fileName, string folderName)
    {
        var filePath = Path.Combine("D:\\MEDIA", folderName, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public static string UploadDoc(IFormFile file, string folderName)
    {
        // 1. Get Located Folder Path
        //string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", folderName);
        string folderPath = Path.Combine("D:\\", folderName);

        // 2. Get File Name and Make it Unique
        string fileName = $"{Guid.NewGuid()}{file.FileName}";

        // 3. Get File Path
        string filePath = Path.Combine(folderPath, fileName);

        // 4. Save File as Streams : [Data Per Time]
        using var fs = new FileStream(filePath, FileMode.Create);

        file.CopyTo(fs);


        return fileName;
    }

    public static void DeleteDoc(string fileName, string folderName)
    {
        var filePath = Path.Combine("D:\\", folderName, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
