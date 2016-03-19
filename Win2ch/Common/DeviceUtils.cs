namespace Win2ch.Common {
    public enum DeviceFamily {
        Unknown,
        IoT,
        Xbox,
        Team,
        HoloLens,
        Desktop,
        Mobile
    }

    public static class DeviceUtils {
        public static DeviceFamily GetDeviceFamily() {
            var family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            switch (family) {
                case "Windows.Desktop": return DeviceFamily.Desktop;
                case "Windows.Mobile": return DeviceFamily.Mobile;
                case "Windows.Team": return DeviceFamily.Team;
                case "Windows.IoT": return DeviceFamily.IoT;
                case "Windows.Xbox": return DeviceFamily.Xbox;
                case "Windows.HoloLens": return DeviceFamily.HoloLens;
                default: return DeviceFamily.Unknown;
            }
        }
    }
}