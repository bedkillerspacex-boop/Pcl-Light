using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCL.Core.UI;

namespace PCL.Core.Test.UI;

[TestClass]
public class ImageLoaderHelperTest
{
    [DataTestMethod]
    [DataRow(512, 256, 128, 64)]
    [DataRow(256, 512, 64, 128)]
    [DataRow(64, 64, 64, 64)]
    public void ServerLogoDecodeIsBoundedWithoutChangingAspectRatio(
        int sourceWidth, int sourceHeight, int expectedWidth, int expectedHeight)
    {
        BitmapSource? decoded = null;
        Exception? failure = null;
        using var completed = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            try
            {
                var dispatcher = Dispatcher.CurrentDispatcher;
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
                var image = new Image();
                var dataUri = CreateLargePngDataUri(sourceWidth, sourceHeight);
                var loadTask = ImageLoaderHelper.SetServerLogoAsync(dataUri, image, string.Empty);
                loadTask.ContinueWith(_ => dispatcher.BeginInvokeShutdown(DispatcherPriority.Background),
                    TaskScheduler.Default);

                Dispatcher.Run();
                loadTask.GetAwaiter().GetResult();
                decoded = image.Source as BitmapSource;
            }
            catch (Exception ex)
            {
                failure = ex;
            }
            finally
            {
                completed.Set();
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.IsTrue(completed.Wait(TimeSpan.FromSeconds(15)), "WPF dispatcher did not complete image loading.");
        thread.Join();

        if (failure is not null) Assert.Fail(failure.ToString());
        Assert.IsNotNull(decoded);
        Assert.AreEqual(expectedWidth, decoded!.PixelWidth);
        Assert.AreEqual(expectedHeight, decoded.PixelHeight);
    }

    private static string CreateLargePngDataUri(int width, int height)
    {
        var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        var pixels = new byte[width * height * 4];
        for (var i = 0; i < pixels.Length; i += 4)
        {
            pixels[i] = 0x40;
            pixels[i + 1] = 0x80;
            pixels[i + 2] = 0xC0;
            pixels[i + 3] = 0xFF;
        }

        bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
        bitmap.Freeze();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = new MemoryStream();
        encoder.Save(stream);
        return $"data:image/png;base64,{Convert.ToBase64String(stream.ToArray())}";
    }
}
