using System;
using Xamarin.Forms;
using Thaipod101;
using Thaipod101.iOS;
using System.IO;
using Foundation;
using AVFoundation;

[assembly: Dependency (typeof (AudioService))]
namespace Thaipod101.iOS
{
    public class AudioService : IAudio
    {
        public AudioService()
        {
        }

        public void PlayAudioFile(string fileName)
        {
            string sFilePath = NSBundle.MainBundle.PathForResource
            (Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
            var url = NSUrl.FromString(sFilePath);
            var _player = AVAudioPlayer.FromUrl(url);
            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                _player = null;
            };
            _player.Play();
        }
    }
}