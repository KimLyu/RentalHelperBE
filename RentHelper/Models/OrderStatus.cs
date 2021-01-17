using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RentHelper.Models
{
    public enum OrderStatus
    {
        [Description("暫存")]
        NONE = 0,
        [Description("已立單")]
        CREATE = 1,
        [Description("買家已完成支付")]
        PAYED = 2,
        [Description("已寄送")]
        SEND = 3,
        [Description("已抵達")]
        ARRIVED = 4,
        [Description("歸還已寄出")]
        SENDBACK = 5,
        [Description("已歸還")]
        GETBACK = 6,
        [Description("已結單")]
        CLOSE = 7
    }
    public static class Extensions
    {
        public static string getStatusDesc(this OrderStatus status)
        {
            FieldInfo fi = status.GetType().GetField(status.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }
            return status.ToString();
        }
    }



}
