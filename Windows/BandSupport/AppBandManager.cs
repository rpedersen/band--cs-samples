using System;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.Band;

namespace BandSupport
{
    public class AppBandManager
    {
        public static readonly AppBandManager Instance = new AppBandManager();

        private IBandClient _bandClient;

        private AppBandManager()
        { }

        public async Task<IBandClient> GetBandClientAsync()
        {
            if (_bandClient != null)
            {
                // test to see if the band client is still good by looking at the version number
                try
                {
                    await _bandClient.GetFirmwareVersionAsync();
                    return _bandClient;
                }
                catch (Exception)
                {
                    _bandClient = null;
                }
            }

            // TODO: what if we get a band exception here?
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();
            if (pairedBands.Length < 1)
            {
                throw new BandNotPairedException();
            }

            _bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

            return _bandClient;
        }

        public async Task SetupBandAsync()
        {
            var bandClient = await GetBandClientAsync();
            await SetThemeAsync(bandClient);
            await SetBackgroundAsync(bandClient);
        }

        private static async Task SetThemeAsync(IBandClient bandClient)
        {
            var bandTheme = new BandTheme()
            {
                Base = new BandColor(0x08, 0x22, 0x5B),
                Highlight = new BandColor(0x0D, 0x36, 0x7F),
                Lowlight = new BandColor(0x06, 0x1C, 0x3E),
                Muted = Colors.Gray.ToBandColor(),
                SecondaryText = Colors.Black.ToBandColor()
            };

            await bandClient.PersonalizationManager.SetThemeAsync(bandTheme);
        }

        private static async Task SetBackgroundAsync(IBandClient bandClient)
        {
            var backgroundImageUri = new Uri("ms-appx:///Assets/BandBackground.png");
            var bandImage = await backgroundImageUri.GetBandImageAsync();

            await bandClient.PersonalizationManager.SetMeTileImageAsync(bandImage);
        }
    }
}