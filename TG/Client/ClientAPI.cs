using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TG.Client
{
    public class ClientAPI
    {
        private HttpClient _client;
        private static string _adress;

        public ClientAPI()
        {
            _adress = Constants.Adres;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_adress);
        }

        public async Task<string> GetSong(string pattern,string user, int number=3)
        {
            var responce = await _client.GetAsync($"songs?pattern={pattern}&number={number}&user={user}");
            responce.EnsureSuccessStatusCode();

            if (responce.IsSuccessStatusCode)
            {
                var content = responce.Content.ReadAsStringAsync().Result;
                var songs = JsonConvert.DeserializeObject<List<Models.SongResponse>>(content);
                var result = $"Best matches for {pattern}:\n\n";
                if (songs.Count>0)
                {
                    for (int i = 0; i < songs.Count; i++)
                    {
                        result += $"{i + 1}. {songs[i].ArtistName} - {songs[i].Title}\n" +
                            $"Genre: {songs[i].genre}\nDifficulty: {songs[i].difficulty}\n" +
                            $"Tabs:{songs[i].URL}\n" +
                            $"Youtube:{songs[i].YoutubeURL}\n\n ";

                    }
                    return result;
                }
                else return "We could not find this song. Try again";
            }
            else return "Oops! Error";
        }
        public async Task AddToFav(string user, int number)
        {
            var responce = await _client.GetAsync($"addtofav?user={user}&number={number}");
            responce.EnsureSuccessStatusCode();
        }
        public async Task DelfromFav(string user, int number)
        {
            var responce = await _client.DeleteAsync($"delete?user={user}&number={number}");
            responce.EnsureSuccessStatusCode();
        }
        public async Task<string> Favorites(string user)
        {
            var responce = await _client.GetAsync($"favorites?user={user}");
            responce.EnsureSuccessStatusCode();

            if (responce.IsSuccessStatusCode)
            {
                var content = responce.Content.ReadAsStringAsync().Result;
                var songs = JsonConvert.DeserializeObject<List<Models.SongResponse>>(content);
                var result = $"Your favorites:\n\n";
                if (songs.Count > 0)
                {
                    for (int i = 0; i < songs.Count; i++)
                    {
                        result += $"{i + 1}. {songs[i].ArtistName} - {songs[i].Title}\n" +
                            $"Genre: {songs[i].genre}\nDifficulty: {songs[i].difficulty}\n" +
                            $"Tabs:{songs[i].URL}\n" +
                            $"Youtube:{songs[i].YoutubeURL}\n\n ";
                    }

                    return result;
                }
                else return "You favorites list is empty. Search end add some songs\n /help";
            }
            else return "Oops! Error";
        }
        public async Task<string> Chord(string chord)
        {
            var responce = await _client.GetAsync($"chord?chord={chord}");

            if (responce.IsSuccessStatusCode)
            {
                var content = responce.Content.ReadAsStringAsync().Result;
                var chords = JsonConvert.DeserializeObject<List<Models.ChordResponse>>(content);
                var ch = chords[0];
                var str = $"{ch.chordName.Replace(",", "")}\n{ch.strings}\nFeet to tones: {ch.tones}";
                return str;

            }
            else return "Oops, we can`t find this chord";
        }
        public async Task<string> Chords(string chord)
        {
            var responce = await _client.GetAsync($"chords?chord={chord}");

            if (responce.IsSuccessStatusCode)
            {
                var content = responce.Content.ReadAsStringAsync().Result;
                var chords = JsonConvert.DeserializeObject<List<Models.ChordResponse>>(content);
                var str = "";
                foreach (var ch in chords)
                {
                    str += $"{ch.chordName.Replace(",", "")}\n{ch.strings}\nFeet to tones: {ch.tones}\n\n";
                }
                return str;

            }
            else return "Oops, we can`t find this chord";
        }
        public async Task<string> Recks(string user)
        {
            try
            {
                var responce = await _client.GetAsync($"recommendations?user={user}");
                responce.EnsureSuccessStatusCode();

                var content = responce.Content.ReadAsStringAsync().Result;
                var songs = JsonConvert.DeserializeObject<List<Models.SongResponse>>(content);
                var result = $"Your recommendations:\n\n";
                if (songs.Count > 0)
                {
                    for (int i = 0; i < songs.Count; i++)
                    {
                        result += $"{i + 1}. {songs[i].ArtistName} - {songs[i].Title}\n" +
                            $"Genre: {songs[i].genre}\nDifficulty: {songs[i].difficulty}\n" +
                            $"Tabs:{songs[i].URL}\n" +
                            $"Youtube:{songs[i].YoutubeURL}\n\n ";
                    }

                    return result;
                }
                else return "We could not pick you a song. Try again";
            }
            catch { return "Oops! Error"; }
        }
    }
}
