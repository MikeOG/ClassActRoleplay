using System;
using System.Collections.Generic;
using System.Linq;

namespace Roleplay.Shared
{
    public class CommandArgs
    {
        private string asString = string.Empty;
        private List<string> args = new List<string>();

        public int Count
        {
            get
            {
                return this.args.Count<string>();
            }
            private set
            {
            }
        }

        public bool HasAny
        {
            get
            {
                return this.Count > 0;
            }
            private set
            {
            }
        }

        public CommandArgs(string argStr)
        {
            this.asString = argStr;
            if (argStr.IndexOf(' ') > 0)
            {
                this.args.AddRange((IEnumerable<string>)argStr.Split(new char[1]
                {
                    ' '
                }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(argStr))
                    return;
                this.args.Add(argStr);
            }
        }

        public override string ToString()
        {
            return this.asString;
        }

        public string Get(int index)
        {
            return this.args[index];
        }

        public bool GetBool(int index)
        {
            return Convert.ToBoolean(this.args[index]);
        }

        public short GetInt16(int index)
        {
            return Convert.ToInt16(this.args[index]);
        }

        public int GetInt32(int index)
        {
            return Convert.ToInt32(this.args[index]);
        }

        public long GetInt64(int index)
        {
            return Convert.ToInt64(this.args[index]);
        }

        public float GetFloat(int index)
        {
            return (float)Convert.ToDouble(this.args[index]);
        }

        public double GetDouble(int index)
        {
            return Convert.ToDouble(this.args[index]);
        }
    }
}