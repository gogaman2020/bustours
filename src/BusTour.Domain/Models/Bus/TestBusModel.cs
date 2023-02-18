using BusTour.Domain.Enums;
using Infrastructure.Common.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusTour.Domain.Models.Bus
{
    public class TestBusModel : BusModel
    {
        static public TestBusModel Parse()
        {
            var fileContent = Resources.EmbeddedResource.GetFileContent("BusModel");

            TestBusModel result = null;

            try
            {
                result = JsonConvert.DeserializeObject<TestBusModel>(fileContent);
                if (result != null)
                {
                    byte seatId = 1;

                    foreach (var table in result.Tables)
                    {
                        if (table.Seats == null)
                        {
                            table.Seats = new List<SeatModel>();
                        }
                        else
                        {
                            table.Seats.Clear();
                        }

                        switch (table.Type)
                        {
                            case TableTypes.Two:
                                table.Seats.Add(new SeatModel { Id = seatId++, Number = "A" });
                                table.Seats.Add(new SeatModel { Id = seatId++, Number = "B" });
                                break;
                            case TableTypes.Four:
                                table.Seats.Add(new SeatModel { Id = seatId++, Number = "A" });
                                table.Seats.Add(new SeatModel { Id = seatId++, Number = "B" });
                                table.Seats.Add(new SeatModel { Id = seatId++, Number = "C" });
                                table.Seats.Add(new SeatModel { Id = seatId++, Number = "D" });
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.For<TestBusModel>()
                    .LogError(ex, ex.Message);
            }

            return result;
        }

        public void ApplySelection(List<BusObject> selectedObjects)
        {
            if (selectedObjects == null) return;

            foreach (var table in Tables)
            {
                table.IsSelected = selectedObjects.Any(p => p.Type == BusObjectTypes.Table && p.Id == table.Id);

                table.Seats.ForEach(s => s.IsSelected = selectedObjects.Any(p => p.Type == BusObjectTypes.Seat && p.Id == s.Id));
            }
        }

        public void LockSwitch(BusObject busObject)
        {
            switch (busObject.Type)
            {
                case BusObjectTypes.Table:
                    {
                        var table = GetTable(busObject.Id);
                        if (table == null) return;

                        var isLocked = table.IsLocked;

                        table.Seats.ForEach(p => p.IsLocked = !isLocked);
                    }
                    break;
                case BusObjectTypes.Seat:
                    {
                        var seat = GetSeat(busObject.Id);
                        if (seat == null) return;

                        seat.IsLocked = !seat.IsLocked;
                    }
                    break;
            }
        }
    }
}
