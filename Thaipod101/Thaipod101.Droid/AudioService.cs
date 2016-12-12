using System;
using Xamarin.Forms;
using Thaipod101.Droid;
using Android.Media;
using Android.Content.Res;

[assembly: Dependency(typeof(AudioService))]
namespace Thaipod101.Droid
{
    public class AudioService : IAudio
    {
        public AudioService()
        {
        }

        public async void PlayAudioFile(string fileName)
        {
            /*
            var player = new MediaPlayer();
            var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
            player.Prepared += (s, e) =>
            {
                player.Start();
            };
            player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            player.Prepare();
            */

            var player = new MediaPlayer();
            //Tell our player to stream music
            player.SetAudioStreamType(Stream.Music);
            //When we have prepared the song start playback
            player.Prepared += (sender, args) => player.Start();
            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            player.Completion += (sender, args) => player.Stop();
            player.Error += (sender, args) => {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
                player.Stop();//this will clean up and reset properly.
            };

            //await player.SetDataSourceAsync(ApplicationContext, Android.Net.Uri.Parse(fileName));
            await player.SetDataSourceAsync(Forms.Context, Android.Net.Uri.Parse(fileName));
            player.PrepareAsync();

        }
    }
}