using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LiveChat
{
    class Program
    {
        /// <summary>
        /// Набор слов из русского языка
        /// </summary>
        public static string[] Words;

        /// <summary>
        /// Количество попыток подбора слова у каждого бота
        /// </summary>
        const int ChatBotAttemptsCount = 10;

        /// <summary>
        /// Ограничение количества участников в чате
        /// </summary>
        const int ChatMembersLimit = 3;

        /// <summary>
        /// Первая буква с которой нужно начинать поиск слова
        /// </summary>
        const char FirstChar = 'а';

        /// <summary>
        /// Показывать изменения состояния бота
        /// </summary>
        const bool ShowChatBotStateChanges = true;

        /// <summary>
        /// Исскуственная задержка при добавлении слова
        /// </summary>
        static TimeSpan ChatInsertionTime = TimeSpan.FromMilliseconds(10);

        /// <summary>
        /// Список чат-ботов с именами
        /// </summary>
        static ChatBot[] ChatBots = new ChatBot[]
            {
                    new ChatBot("Василий"),
                    new ChatBot("Алиса"),
                    new ChatBot("Геннадий"),
                    new ChatBot("Ольга"),
                    new ChatBot("Юлия"),
                    new ChatBot("Хабиб"),
                    new ChatBot("Джон"),
                    new ChatBot("Галина"),
                    new ChatBot("Сергей"),
                    new ChatBot("Мария"),
                    new ChatBot("Трактор"),
                    new ChatBot("Александр"),
                    new ChatBot("Макар"),
                    new ChatBot("Абдрахман"),
                    new ChatBot("Евгений"),
                    new ChatBot("Турия"),
                    new ChatBot("Сумка Г"),
                    new ChatBot("Петя"),
                    new ChatBot("Фанат"),
                    new ChatBot("Trevor")
            };

        /// <summary>
        /// Чат
        /// </summary>
        static Chat Chat = new Chat(FirstChar, "Игра в слова", ChatMembersLimit, ChatInsertionTime);

        static void Main(string[] args)
        {
            // Это для регистрации кодировки Windows-1251 (не важно для занятия)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Запоминаем слова в оперативной памяти
            Words = File.ReadAllLines("russian.txt", Encoding.GetEncoding(1251));

            // Подписываемся на изменение списка сообщений в чате
            Chat.Messages.CollectionChanged += Messages_CollectionChanged;

            // Запускаем всех чат-ботов
            foreach (var chatBot in ChatBots)
            {
                // Подписываемся на изменение состояния если нужно
                if (ShowChatBotStateChanges)
                {
                    chatBot.StateChanged += ChatBot_StateChanged;
                }
                chatBot.Start(Chat, ChatBotAttemptsCount);
            }

            Console.Read();

            Console.WriteLine($"Количество сообщений: {Chat.Messages.Count}");

            Console.Read();
        }

        private static void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Выводим все новые сообщения, которые пришли с событием
            foreach (var newItem in e.NewItems)
            {
                // Используем критическую секцию, чтобы цвета не путались,
                // т.к. Console.ForegroundColor - общий ресурс
                lock (Chat)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\t{newItem}");
                    Console.ResetColor();
                }
            }
        }

        private static void ChatBot_StateChanged(object sender, EventArgs e)
        {
            if (sender is ChatBot chatBot)
            {
                // Используем критическую секцию, чтобы цвета не путались,
                // т.к. Console.ForegroundColor - общий ресурс
                lock (Chat)
                {
                    Console.ForegroundColor = chatBot.State == "думает" ? ConsoleColor.DarkGray : ConsoleColor.Green;
                    Console.WriteLine($"\t{chatBot.Name} {chatBot.State}");
                    Console.ResetColor();
                }
            }
        }
    }
}
