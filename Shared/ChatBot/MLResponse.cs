using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ChatBot
{
    public class MLResponse
    {
        // لازم الاسم هنا يكون "response" بنفس الحروف الصغيرة 
        // عشان يطابق الـ JSON اللي شفناه في الـ Swagger بتاع الـ ML
        public string response { get; set; }

        // لو حابة تسجلي الوقت اللي خده الموديل (اختياري)
        public int generation_time_ms { get; set; }

        // لو حابة تسجلي عدد التوكنز (اختياري)
        public int tokens_generated { get; set; }
    }
}
