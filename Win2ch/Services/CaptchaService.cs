using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Win2ch.Common;
using Win2ch.Controls;
using Win2ch.Models;

namespace Win2ch.Services {
    public class CaptchaService {

        private CaptchaService() { }

        public static CaptchaService Instance { get; } = new CaptchaService();

        public Task<CaptchaInfo> GetCaptchaInfoForPosting() {
            return GetCapthaInfo(Urls.PostingCaptcha);
        }

        public Task<CaptchaInfo> GetCaptchaInfoForThread() {
            return GetCapthaInfo(Urls.ThreadCaptcha);
        }

        private async Task<CaptchaInfo> GetCapthaInfo(string url) {
            var uri = new Uri(url);
            var client = new HttpClient();
            var response = await client.GetStringAsync(uri);
            var key = GetKey(response);

            return new CaptchaInfo(key != null, "2chaptcha", key);
        }

        private string GetKey(string response) {
            var lines = response.Split('\n');
            if (lines.Length < 2 || lines[0].EqualsNc("vip") || lines[0].Equals("disabled"))
                return null;

            return lines[1];
        }

        public async Task<string> GetCaptchaResult(CaptchaInfo captchaInfo) {
            if (captchaInfo.Key == null)
                return null;

            var uri = new Uri(string.Format(Urls.CaptchaImage, captchaInfo.Key));

            var dialog = new CaptchaDialog();
            dialog.Image.Source = new BitmapImage(uri);

            await dialog.ShowAsync();
            return dialog.TextBox.Text;
        }
    }

    public class CaptchaInfo {
        public CaptchaInfo(bool isCaptchaNeeded, string captchaType, string key) {
            IsCaptchaNeeded = isCaptchaNeeded;
            CaptchaType = captchaType;
            Key = key;
        }

        public bool IsCaptchaNeeded { get; }
        public string CaptchaType { get; }
        public string Key { get; }

    }
}