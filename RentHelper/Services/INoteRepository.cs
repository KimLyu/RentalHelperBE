using RentHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentHelper.Services
{
    public interface INoteRepository
    {
        Task<int> addNote(Guid uid, Note note);
        void firebasePushNotifaction(Note note);

        Task<IEnumerable<Note>> getList(Guid uid, Guid oid);

        Task<int> delNote(Guid uid, Guid nid);
    }
}
