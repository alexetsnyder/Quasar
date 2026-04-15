using Catcophony.core.enums;
using Catcophony.core.reflection.attributes;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Catcophony.scenes.cats
{
    public partial class CatModel
    {
        private string _name;
        public string Name 
        { 
            get
            {
                return _name;
            }
            set
            {
                SetValue(ref _name, value);
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetValue(ref _description, value);
            }
        }


        private string _feelings;
        public string Feelings
        {
            get
            {
                return _feelings;
            }
            set
            {
                SetValue(ref _feelings, value);
            }
        }

        public int _health;

        [StatusAttribute]
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                SetValue(ref _health, value);
            }
        }

        public int _stamina;

        [StatusAttribute]
        public int Stamina
        {
            get
            {
                return _stamina;
            }
            set
            {
                SetValue(ref _stamina, value);
            }
        }

        private int _hunger;

        [StatusAttribute]
        public int Hunger
        {
            get
            {
                return _hunger;
            }
            set
            {
                SetValue(ref _hunger, value);
            }
        }

        private int _thirst;

        [StatusAttribute]
        public int Thirst
        {
            get
            {
                return _thirst;
            }
            set
            {
                SetValue(ref _thirst, value);
            }
        }

        public ActionType _actionType;

        [StatusAttribute]
        public ActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                SetValue(ref _actionType, value);
            }
        }

        private Vector2? _actionPos;

        [StatusAttribute]
        public Vector2? ActionPos
        {
            get
            {
                return _actionPos;
            }
            set
            {
                SetValue(ref _actionPos, value);
            }
        }

        public bool IsDirty { get; private set; } = false;

        public CatModel GetCopy()
        {
            IsDirty = false;

            var copy = new CatModel();

            var type = this.GetType();
            var getProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
            var setProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField);

            foreach (var property in getProperties)
            {
                var setProperty = setProperties.FirstOrDefault(x  => x.Name == property.Name);
                if (setProperty != null)
                {
                    setProperty.SetValue(copy, property.GetValue(this));
                }
            }

            return copy;
        }

        private void SetValue<T>(ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                IsDirty = true;
            }
        }
    }
}