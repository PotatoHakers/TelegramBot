using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient(token: "5443651302:AAEyzORweKDSaUvaqWIawIH46kEzJg2zais");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

botClient.StartReceiving(
HandleUpdatesAsync,
HandleErrorAsync,
receiverOptions,
cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine(me.Username);
Console.ReadLine();

cts.Cancel();

async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await HandleMessage(botClient, update.Message);
        return;
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }

    async Task HandleMessage(ITelegramBotClient botClient , Message message)
    {
        if (message.Text == "/start")
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose commands: /rolloto | /help");
        }
        if (message.Text == "/rolloto")
        {
            ReplyKeyboardMarkup keyboard = new(new[]
            {
              new KeyboardButton[] { "123", "312"},
              new KeyboardButton[] { "456", "654"}
            });
            await botClient.SendTextMessageAsync(message.Chat.Id, "Choose:", replyMarkup: keyboard);
            return;
        }

        await botClient.SendTextMessageAsync(message.Chat.Id, $"You said: \n{message.Text}");
    }
    async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {

    }
}
Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException => $"Ошибка телеграм АПИ:\n {apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}