using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChess.Models
{
    public enum Level
    {
        Elephant = 8, Lion = 7,Tiger = 6,
        Leopard = 5, Dog = 4, Wolf = 3,
        Cat = 2, Rat = 1, Dangerous = 0
    }
    public class Animal : INotifyPropertyChanged//, IDisposable
    {
        private Level _cur_lv, _temp_lv;
        private double _width, _height;
        private string _name, _imageName, _animalSound;
        public Level Cur_LV
        {
            get => _cur_lv;
            set
            {
                _cur_lv = value;
                NotifyPropertyChanged("Cur_LV");
            }
        }
        public Level Temp_LV
        {
            get => _temp_lv;
            set
            {
                _temp_lv = value;
                NotifyPropertyChanged("Temp_LV");
            }
        }
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                NotifyPropertyChanged("Width");
            }
        }
        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                NotifyPropertyChanged("Height");
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string ImageName
        {
            get => _imageName;
            set
            {
                _imageName = value;
                NotifyPropertyChanged("ImageName");
            }
        }
        public string AnimalSound
        {
            get => _animalSound;
            set
            {
                _animalSound = value;
                NotifyPropertyChanged("AnimalSound");
            }
        }
        public Animal()
        {
            //Temp_LV = Level.Dangerous;
            Cur_LV = Level.Dangerous;
            Width = 100;
            Height = 100;
            Name = "Animal";
            ImageName = "*.png";
            AnimalSound = "*.wav";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    #region 8 Animals
    public class Elephant : Animal
    {
        public Elephant()
        {
            Cur_LV = Level.Elephant;
            Name = "Elephant";
            ImageName = "elephant.png";
            AnimalSound = "elephant.wav";
        }
    }
    public class Lion : Animal
    {
        public Lion()
        {
            Cur_LV = Level.Lion;
            Name = "Lion";
            ImageName = "lion.png";
            AnimalSound = "lion.wav";
        }
    }
    public class Tiger : Animal
    {
        public Tiger()
        {
            Cur_LV = Level.Tiger;
            Name = "Tiger";
            ImageName = "tiger.png";
            AnimalSound = "tiger.wav";
        }
    }
    public class Leopard : Animal
    {
        public Leopard()
        {
            Cur_LV = Level.Leopard;
            Name = "Leopard";
            ImageName = "leopard.png";
            AnimalSound = "leopard.wav";
        }
    }
    public class Dog : Animal
    {
        public Dog()
        {
            Cur_LV = Level.Dog;
            Name = "Dog";
            ImageName = "dog.png";
            AnimalSound = "dog.wav";
        }
    }
    public class Wolf : Animal
    {
        public Wolf()
        {
            Cur_LV = Level.Wolf;
            Name = "Wolf";
            ImageName = "wolf.png";
            AnimalSound = "wolf.wav";
        }
    }
    public class Cat : Animal
    {
        public Cat()
        {
            Cur_LV = Level.Cat;
            Name = "Cat";
            ImageName = "cat.png";
            AnimalSound = "cat.wav";
        }
    }
    public class Rat : Animal
    {
        public Rat()
        {
            Cur_LV = Level.Rat;
            Name = "Rat";
            ImageName = "rat.png";
            AnimalSound = "rat.wav";
        }
    }
    #endregion
}
