using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtility
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class KeyID : Attribute
    {
        private string id;
        /// <summary>
        /// 指定实际ID
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
