using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TG
{
    public class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient(Constants.Token);
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var _client = new Client.ClientAPI();
            string patternImg = @"/song(\d) ";
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text != null)
                {
                    var text = message.Text.ToLower();
                    MatchCollection matches = Regex.Matches(text, patternImg, RegexOptions.IgnoreCase);
                    if (text == "/start")
                    {
                        try
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Hello. Have a guitar? I`ll help you with storagging your favourite songs, " +
                            "searching chords and will try to recommend you some cool song.\nSend me /help if you want to check a commands.");
                        }
                        catch { }
                    }
                    else if (text == "/help")
                    {
                        try
                        {
                            await botClient.SendTextMessageAsync(message.Chat,
                           "/song - Search tabs for song \n    Example:(/song Smells like teen spirit)\n" +
                           "/song{n} - You can also specify the number of results(up to 9) \n    Example:(/song8 Smells like teen spirit)\n" +
                           "/add - Add song from your last search to favorites\n    Example:(/add 1)\n" +
                           "/delete - Delete song from your favorites list\n   Example:(/delete 1)\n" +
                           "/favorites - Show your favorites\n" +
                           "/chord - Search chord\n" +
                           "/chords - Search chords like this\n    Example:(/chord Fmaj7/E)\n" +
                           "/recommend - Recommend some songs based on your favorites\n");
                        }
                        catch { }
                    }
                    else if (matches.Count > 0)
                    {
                        var song = message.Text.Replace(matches[0].ToString(), "");
                        int number = Convert.ToInt32(matches[0].ToString().Trim().Replace("/song", ""));
                        try
                        {
                            var response = await _client.GetSong(song, message.Chat.Id.ToString(), number);
                            await botClient.SendTextMessageAsync(message.Chat, response);
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Can`t find a song");
                        }
                    }
                    else if (text.Contains("/song "))
                    {
                        var song = message.Text.Replace("/song ", "");
                        try
                        {
                            var response = await _client.GetSong(song, message.Chat.Id.ToString());
                            await botClient.SendTextMessageAsync(message.Chat, response);
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Can`t find a song");
                        }
                    }
                    else if (text.Contains("/add "))
                    {
                        var song = message.Text.Replace("/add ", "");
                        try
                        {
                            var numb = Convert.ToInt32(song);
                            await _client.AddToFav(message.Chat.Id.ToString(), Convert.ToInt32(song));
                            await botClient.SendTextMessageAsync(message.Chat, "Check your /favorites");
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Incorrect input");
                        }
                    }
                    else if (text.Contains("/delete "))
                    {
                        var song = message.Text.Replace("/delete ", "");
                        try
                        {
                            await _client.DelfromFav(message.Chat.Id.ToString(), Convert.ToInt32(song));
                            await botClient.SendTextMessageAsync(message.Chat, "Check your /favorites");
                        }
                        catch
                        {
                            await botClient.SendTextMessageAsync(message.Chat, "Incorrect input");
                        }
                    }
                    else if (text.Contains("/favorites"))
                    {
                        var fav = await _client.Favorites(message.Chat.Id.ToString());
                        await botClient.SendTextMessageAsync(message.Chat, fav);
                    }
                    else if (text.Contains("/chords "))
                    {
                        var chord = message.Text.Replace("/chords ", "");
                        try
                        {
                            var ch = await _client.Chords(chord);
                            await botClient.SendTextMessageAsync(message.Chat, ch);
                        }
                        catch { await botClient.SendTextMessageAsync(message.Chat, "Oops! Error"); }
                    }
                    else if (text.Contains("/chord "))
                    {
                        try
                        {
                            var chord = message.Text.Replace("/chord ", "");
                            var ch = await _client.Chord(chord);
                            await botClient.SendTextMessageAsync(message.Chat, ch);
                        }
                        catch { await botClient.SendTextMessageAsync(message.Chat, "Oops! Error"); }
                    }
                    else if (text.Contains("/recommend"))
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Wait a second. We are looking for the best songs for you...");
                        var rec = await _client.Recks(message.Chat.Id.ToString());
                        await botClient.SendTextMessageAsync(message.Chat, rec);
                    }
                    else await botClient.SendTextMessageAsync(message.Chat, "Bazaka?");
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var me = new Chat();
            me.Id = Constants.MyID;
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
            await botClient.SendTextMessageAsync(me, Newtonsoft.Json.JsonConvert.SerializeObject(exception.Message));
        }
        public static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            CreateHostBuilder(args).Build().Run();
           
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
