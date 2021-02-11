using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChess.Models
{
    public class Position : INotifyPropertyChanged
    {
        private int _column, _row;
        public int Column
        {
            get => _column;
            set
            {
                _column = value;
                NotifyPropertyChanged("Column");
            }
        }

        public int Row
        {
            get => _row;
            set
            {
                _row = value;
                NotifyPropertyChanged("Row");
            }
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
    public class Animal : INotifyPropertyChanged//, IDisposable
    {
        private Level _cur_lv, _temp_lv;
        private string _name, _imageName, _animalSound;
        private Position _position;
        private Player _player;
        private Swim _swim;
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
        public Player Player
        {
            get => _player;
            set
            {
                _player = value;
                NotifyPropertyChanged("Player");
            }
        }
        public Swim Swim
        {
            get => _swim;
            set
            {
                _swim = value;
                NotifyPropertyChanged("Swim");
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
        public Position Position
        {
            get => _position;
            set
            {
                _position = value;
                NotifyPropertyChanged("Position");
            }
        }
        public Animal()
        {
            //Temp_LV = Level.Dangerous;
            Cur_LV = Level.Dangerous;
            Name = "Animal";
            ImageName = "*.png";
            AnimalSound = "*.wav";
            Position = new Position { Column = -1, Row = -1 };
            Player = Player.Blue;
            Swim = Swim.NotAvailable;
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
            Temp_LV = Level.Elephant;
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
            Temp_LV = Level.Lion;
            Name = "Lion";
            ImageName = "lion.png";
            AnimalSound = "lion.wav";
            Swim = Swim.Jump;
        }
    }
    public class Tiger : Animal
    {
        public Tiger()
        {
            Cur_LV = Level.Tiger;
            Temp_LV = Level.Tiger;
            Name = "Tiger";
            ImageName = "tiger.png";
            AnimalSound = "tiger.wav";
            Swim = Swim.Jump;
        }
    }
    public class Leopard : Animal
    {
        public Leopard()
        {
            Cur_LV = Level.Leopard;
            Temp_LV = Level.Leopard;
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
            Temp_LV = Level.Dog;
            Name = "Dog";
            ImageName = "dog.png";
            AnimalSound = "dog.wav";
            Swim = Swim.Available;
        }
    }
    public class Wolf : Animal
    {
        public Wolf()
        {
            Cur_LV = Level.Wolf;
            Temp_LV = Level.Wolf;
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
            Temp_LV = Level.Cat;
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
            Temp_LV = Level.Rat;
            Name = "Rat";
            ImageName = "rat.png";
            AnimalSound = "rat.wav";
            Swim = Swim.Available;
        }
    }
    #endregion
}
