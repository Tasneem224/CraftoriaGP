using Microsoft.AspNetCore.SignalR;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Hubs
{
    // سيبيه فاضي، وظيفته بس إنه يفتح Connection
    // بنربطه بالـ Interface اللي عملناه في الـ Abstraction
    public class ChatHub : Hub<IChatClient>
    {
        // الميثود دي اختيارية: لو عايزة تعرفي مين عمل Connect
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
