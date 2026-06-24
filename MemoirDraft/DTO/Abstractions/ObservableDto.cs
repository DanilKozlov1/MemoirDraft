using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoirDraft.DTO.Abstractions
{
    /// <summary>
    /// Класс-абстракция для dto, которые нужны для отображения в UI
    /// </summary>
    public class ObservableDto : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Изменение свойства модели
        /// </summary>
        /// <param name="field">Свойство модели</param>
        /// <param name="value">Значение для свойства</param>
        /// <param name="propertyName">Название свойства модели</param>
        /// <returns>Изменено ли значение свойства</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
