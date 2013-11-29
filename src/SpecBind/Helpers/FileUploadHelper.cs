// <copyright file="FileUploadHelper.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Helpers
{
    using System;
    using System.IO;

    using SpecBind.Pages;

    /// <summary>
    /// A helper class for uploading files to the server.
    /// </summary>
    public static class FileUploadHelper
    {
        /// <summary>
        /// Uploads the file, finding the resource and creating a temporary path for it.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="uploadAction">The upload action, passing in the temporary file path.</param>
        public static void UploadFile(string fileName, Action<string> uploadAction)
        {
            var locatorName = Path.GetFileNameWithoutExtension(fileName);
            var fileBytes = ResourceLocator.GetResource(locatorName);
            if (fileBytes == null)
            {
                throw new ElementExecuteException(
                    "Could not locate file resource: '{0}'. Registered Resources: '{1}'. Make sure your resource file is public and it is a binary resource.",
                    locatorName,
                    ResourceLocator.GetResourceNames());
            }

            //Create a temporary path for it.
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), fileName);

            try
            {
                File.WriteAllBytes(path, fileBytes);

                uploadAction(path);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);

                    var parent = Path.GetDirectoryName(path);
                    if (parent != null)
                    {
                        Directory.Delete(parent);
                    }
                }
            }
        }
    }
}