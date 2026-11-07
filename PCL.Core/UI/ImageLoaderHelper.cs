using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PCL.Core.UI;

/// <summary>
/// 图像加载帮助类，提供通用的图像加载功能
/// </summary>
public static class ImageLoaderHelper {
    // Server icons are displayed at 34–54 device-independent pixels. Decoding them to
    // 128 pixels preserves their appearance on high-DPI displays without retaining
    // unnecessarily large bitmaps supplied by a server.
    private const int ServerIconDecodePixelSize = 128;

    /// <summary>
    /// 异步设置服务器图标
    /// </summary>
    /// <param name="base64String">Base64 图像字符串</param>
    /// <param name="imageElement">目标 Image 控件</param>
    /// <param name="defaultImageUri">默认图像 URI，如果为 null 则使用内置默认图标</param>
    /// <returns></returns>
    public static async Task SetServerLogoAsync(string base64String, Image imageElement, string? defaultImageUri = null) {
        await SetImageFromBase64AsyncCore(base64String, imageElement,
            defaultImageUri ?? "pack://application:,,,/Plain Craft Launcher 2;component/Images/Icons/DefaultServer.png",
            ServerIconDecodePixelSize);
    }

    /// <summary>
    /// 通用的 Base64 图像加载方法
    /// </summary>
    /// <param name="base64String">Base64 图像字符串</param>
    /// <param name="imageElement">目标 Image 控件</param>
    /// <param name="fallbackImageUri">加载失败时的后备图像 URI</param>
    /// <returns></returns>
    public static async Task SetImageFromBase64Async(
        string base64String, 
        Image imageElement, 
        string? fallbackImageUri = null) {
        await SetImageFromBase64AsyncCore(base64String, imageElement, fallbackImageUri, 0);
    }

    private static async Task SetImageFromBase64AsyncCore(
        string base64String,
        Image imageElement,
        string? fallbackImageUri,
        int decodePixelSize) {
        ArgumentNullException.ThrowIfNull(imageElement);
        
        if (string.IsNullOrWhiteSpace(base64String)) {
            SetFallbackImage(imageElement, fallbackImageUri);
            return;
        }

        try {
            // 提取 Base64 数据部分
            var separatorIndex = base64String.IndexOf(',');
            var base64Data = separatorIndex >= 0
                ? base64String.Substring(separatorIndex + 1)
                : base64String;

            // 验证 Base64 字符串
            if (string.IsNullOrWhiteSpace(base64Data)) {
                SetFallbackImage(imageElement, fallbackImageUri);
                return;
            }

            // 异步转换图像
            var bitmapImage = await Task.Run(() => _CreateBitmapFromBase64(base64Data, decodePixelSize));
            
            // 在 UI 线程上设置图像
            if (imageElement.Dispatcher.CheckAccess()) {
                imageElement.Source = bitmapImage;
            } else {
                await imageElement.Dispatcher.InvokeAsync(() => imageElement.Source = bitmapImage);
            }
        } catch {
            SetFallbackImage(imageElement, fallbackImageUri);
        }
    }

    /// <summary>
    /// 从 Base64 字符串创建 BitmapImage
    /// </summary>
    /// <param name="base64Data">Base64 数据</param>
    /// <returns>BitmapImage 对象</returns>
    private static BitmapImage _CreateBitmapFromBase64(string base64Data, int decodePixelSize) {
        var imageBytes = Convert.FromBase64String(base64Data);

        var decodePixelWidth = 0;
        var decodePixelHeight = 0;
        if (decodePixelSize > 0) {
            using var metadataStream = new MemoryStream(imageBytes, writable: false);
            var frame = BitmapFrame.Create(metadataStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            if (Math.Max(frame.PixelWidth, frame.PixelHeight) > decodePixelSize) {
                if (frame.PixelWidth >= frame.PixelHeight)
                    decodePixelWidth = decodePixelSize;
                else
                    decodePixelHeight = decodePixelSize;
            }
        }

        using var ms = new MemoryStream(imageBytes);
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        if (decodePixelWidth > 0) bitmap.DecodePixelWidth = decodePixelWidth;
        if (decodePixelHeight > 0) bitmap.DecodePixelHeight = decodePixelHeight;
        bitmap.StreamSource = ms;
        bitmap.EndInit();
        bitmap.Freeze(); // 确保跨线程安全
        
        return bitmap;
    }

    /// <summary>
    /// 设置后备图像
    /// </summary>
    /// <param name="imageElement">目标 Image 控件</param>
    /// <param name="fallbackImageUri">后备图像 URI</param>
    public static void SetFallbackImage(Image imageElement, string? fallbackImageUri) {
        try {
            if (!string.IsNullOrWhiteSpace(fallbackImageUri)) {
                var defaultBitmap = new BitmapImage(new Uri(fallbackImageUri));
                if (imageElement.Dispatcher.CheckAccess()) {
                    imageElement.Source = defaultBitmap;
                } else {
                    imageElement.Dispatcher.Invoke(() => imageElement.Source = defaultBitmap);
                }
            } else {
                // 如果没有提供后备图像，则清空 Source
                if (imageElement.Dispatcher.CheckAccess()) {
                    imageElement.Source = null;
                }
                else {
                    imageElement.Dispatcher.Invoke(() => imageElement.Source = null);
                }
            }
        } catch {
            // 处理后备图像加载失败的情况
            if (imageElement.Dispatcher.CheckAccess()) {
                imageElement.Source = null;
            } else {
                imageElement.Dispatcher.Invoke(() => imageElement.Source = null);
            }
        }
    }

    /// <summary>
    /// 从文件路径异步加载图像
    /// </summary>
    /// <param name="imagePath">图像文件路径</param>
    /// <param name="imageElement">目标 Image 控件</param>
    /// <param name="fallbackImageUri">后备图像 URI</param>
    /// <returns></returns>
    public static async Task SetImageFromFileAsync(
        string imagePath,
        Image imageElement,
        string fallbackImageUri) {
        ArgumentNullException.ThrowIfNull(imageElement);

        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath)) {
            SetFallbackImage(imageElement, fallbackImageUri);
            return;
        }

        try {
            var bitmapImage = await Task.Run(() =>
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            });

            if (imageElement.Dispatcher.CheckAccess()) {
                imageElement.Source = bitmapImage;
            } else {
                await imageElement.Dispatcher.InvokeAsync(() => imageElement.Source = bitmapImage);
            }
        } catch {
            SetFallbackImage(imageElement, fallbackImageUri);
        }
    }
}
