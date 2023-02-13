using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Type
{
    public abstract class BaseModel<T> where T : Enum
    {
        public T Type { get; set; }
        public string Code { get; init; }
        public string Name { get; init; }

        public BaseModel(T type, string code, string name)
        {
            this.Type = type;
            this.Code = code;
            this.Name = name;
        }

    }

}
