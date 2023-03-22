using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Explorer.Model
{
    public class DataRoomwithPermission : DataRooms.UI.Models.DataRoom
    {
        public DataRoomPermission DataRoomPermission { get; set; }
    }
}