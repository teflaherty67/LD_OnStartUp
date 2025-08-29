using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD_OnStartUp.Classes
{
    class clsRoomData
    {
        public ElementId RoomId { get; set; }
        public string Name { get; set; }
        public string CeilingFinish { get; set; }
        public string FloorFinish { get; set; }       

        // Constructor to capture from existing room
        public clsRoomData(Room room)
        {
            RoomId = room.Id;

            // Get the Name parameter
            Parameter paramName = room.get_Parameter(BuiltInParameter.ROOM_NAME);
            Name = paramName?.AsString() ?? "";

            // Get the CeilingFinish parameter  
            Parameter paramCeilingFinish = room.LookupParameter("Ceiling Finish");
            CeilingFinish = paramCeilingFinish?.AsString() ?? "";

            // Get the FloorFinish parameter
            Parameter paramFloorFinish = room.LookupParameter("Floor Finish");
            FloorFinish = paramFloorFinish?.AsString() ?? "";
        }

        // Method to restore/transfer to room
        public void RestoreToElement(Room room)
        {
            // Restore Name parameter
            Parameter paramName = room.get_Parameter(BuiltInParameter.ROOM_NAME);
            paramName?.Set(Name);

            // Transfer CeilingFinish to CeilingHeight parameter
            Parameter paramCeilingHeight = room.LookupParameter("Ceiling Height");
            paramCeilingHeight?.Set(CeilingFinish);

            // Restore FloorFinish parameter
            Parameter paramFloorFinish = room.LookupParameter("Floor Finish");
            paramFloorFinish?.Set(FloorFinish);
        }
    }
}
