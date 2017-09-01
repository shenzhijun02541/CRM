using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LIB
{
     public  static  class EnumHelper
    {
        //利用反射机制 并设计字典
        private static ConcurrentDictionary<Type, Dictionary<int, string>> enumNameValueDict = new ConcurrentDictionary<Type, Dictionary<int, string>>();

        //获取枚举对象Key与显示名称的字典
        public static Dictionary<int, string> GetEnumDictionary(Type enumType)
        {
            if (!enumType.IsEnum)
            
                throw new Exception("给定的类型不是枚举类型");
         
            Dictionary<int, string> names = enumNameValueDict.ContainsKey(enumType) ? enumNameValueDict[enumType] : new Dictionary<int, string>();

            if (names.Count == 0)
            {
                names = GetEnumDictionaryItems(enumType);
                enumNameValueDict[enumType] = names;
            }

            return names;
        }

        private static Dictionary<int, string> GetEnumDictionaryItems(Type enumType)
        {

            FieldInfo[] enumItems = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            Dictionary<int, string> names = new Dictionary<int, string>(enumItems.Length);

            foreach (FieldInfo enumItem in enumItems)
            {
                int intValue = (int)enumItem.GetValue(enumType);
                names[intValue] = enumItem.Name;
               
            }
            return names;
        }
        
             


    }
}
