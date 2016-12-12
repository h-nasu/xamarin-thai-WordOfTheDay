using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using static Thaipod101.CrawlData;

namespace Thaipod101
{
    public partial class MainPage : ContentPage
    {
        ICommand PlayAudioCommand;
        List<CrawlData.Examples> MyExamples { set; get; }

        public MainPage()
        {
            InitializeComponent();
            PlayAudioCommand = new Command<String>(PlayAudio);
            datePicker.MaximumDate = DateTime.Now;
                        
            this.SelectedDate(datePicker, new EventArgs());
        }

        async void SelectedDate(object sender, EventArgs e)
        {
            loading.IsRunning = true;
            loading.IsVisible = true;

            var date = ((DatePicker)sender).Date;
            var text = date.Year + "-" + date.Month + "-" + date.Day;

            CrawlData crawlData = new CrawlData();
            var intResult = 0;

            try
            {
                intResult = await crawlData.DownloadHomepage(text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
           
            if (intResult == 1)
            {
                this.MyExamples = crawlData.MyExamples;
                mainImage.Source = crawlData.MyExamples[0].Img;

                exampleArea.Children.Clear();
                foreach (Examples MyExample in this.MyExamples)
                {

                    var button = new Button
                    {
                        Text = "Play Audio",
                        Command = PlayAudioCommand,
                        CommandParameter = MyExample.Thai.Audio
                    };
                    exampleArea.Children.Add(button);

                    var labelThai = new Label
                    {
                        Text = MyExample.Thai.Text,
                        FontSize = 20
                    };
                    exampleArea.Children.Add(labelThai);

                    var labelRomanize = new Label
                    {
                        Text = MyExample.Eng.Romanize,
                        FontSize = 20
                    };
                    exampleArea.Children.Add(labelRomanize);

                    var labelEng = new Label
                    {
                        Text = MyExample.Eng.Text,
                        FontSize = 20
                    };
                    exampleArea.Children.Add(labelEng);

                }
            }

            loading.IsRunning = false;
            loading.IsVisible = false;
        }

        public void PlayAudio(String audioFile)
        {
            DependencyService.Get<IAudio>().PlayAudioFile(audioFile);
        }
    }
}
