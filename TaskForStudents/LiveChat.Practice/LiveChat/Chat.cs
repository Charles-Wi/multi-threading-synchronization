using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace LiveChat
{
    /// <summary>
    /// Чат
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// Название чата
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Максимальное количество участников
        /// </summary>
        public int MaxMembersCount { get; }

        /// <summary>
        /// Время добавления сообщения в чат (исскуственная задержка)
        /// </summary>
        public TimeSpan ChatInsertionTime { get; }

        /// <summary>
        /// Последняя буква или буква с которой нужно начинать следующее слово 
        /// согласно правилам "игры в слова"
        /// (volatile для предотвращения кэширования)
        /// </summary>
        public volatile char LastChar;

        /// <summary>
        /// Конструктор чата
        /// </summary>
        /// <param name="lastChar">Стартовая буква с которой нужно начинать первое слово</param>
        /// <param name="name">Название</param>
        /// <param name="maxMembersCount">Максимальное количество участников</param>
        /// <param name="chatInsertionTime">Время добавления сообщения в чат (исскуственная задержка)</param>
        public Chat(char lastChar, string name, int maxMembersCount, TimeSpan chatInsertionTime)
        {
            Name = name;
            LastChar = lastChar;
            MaxMembersCount = maxMembersCount;
            ChatInsertionTime = chatInsertionTime;
            Messages = new ObservableCollection<string>();
        }

        /// <summary>
        /// Метод для входа в чат
        /// </summary>
        public void Enter()
        {
            // TODO: Надо как-то ограничить количество участников и не пускать больше определенного количества            
        }

        /// <summary>
        /// Метод для выхода из чата
        /// </summary>
        public void Leave()
        {
            // TODO: Надо как-то сигнализировать о том, что чат-комната освободилась от участника
            // TODO: чтобы другие могли войти
        }

        /// <summary>
        /// Метод, который пытается добавить новое сообщение, 
        /// если оно удовлетворяет правилам игры
        /// </summary>
        /// <param name="chatBot">Чат-бот, который пытается это сделать</param>
        /// <param name="word">Слово-кандидат для добавления</param>
        /// <returns>true, если слово удовлетворяет правилам и было добавлено, false иначе</returns>
        public bool TryAddWord(ChatBot chatBot, string word)
        {
            // Нам нужны слова, а не пустота
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            // TODO: Здесь возникает проблема синхронизации
            // TODO: и в чат попадают слова не подходящие по правилам игры
            // TODO: Надо как-то синхронизировать доступ к чату
            if (LastChar == word.First())
            {
                // Иммитируем долгую операцию...
                Thread.Sleep(ChatInsertionTime);
                // Добавляем слово в список
                Messages.Add($"{chatBot.Name}: {word}");
                // Обновляем последнюю букву
                LastChar = word.Last();
                // Для специальных букв - специальные правила
                switch (LastChar)
                {
                    case 'ё':
                        LastChar = 'е';
                        break;
                    case 'й':
                        LastChar = 'и';
                        break;
                    case 'ь':
                    case 'ъ':
                    case 'ы':
                        LastChar = 'а';
                        break;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Список сообщений, 
        /// ObservableCollection - это специальная коллекция, которая позволяет
        /// отслеживать добавление и удаление элементов через событие CollectionChanged
        /// </summary>
        public ObservableCollection<string> Messages { get; }

        public void Test()
        {
            int[] arr = new int[5]{ 1, 2, 3, 4, 5 };

            lock (arr)
            {
                for (int i = 0; i < 5; i++)
                {
                    arr[i] = i;
                }
                
            }
        }
    }
    
    class myException:System.Exception
    {}
}
