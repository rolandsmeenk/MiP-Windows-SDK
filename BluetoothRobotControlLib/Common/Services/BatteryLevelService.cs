﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothRobotControlLib.Common.Services
{
    public class BatteryLevelService : BaseService
    {
        public BatteryLevelService() : base(BluetoothRobotConstants.BATTERY_LEVEL_SERVICE_UUID)
        {
        }

        public async Task<int> ReadBatteryLevel()
        {
            byte[] data = await base.ReadCharacteristicValueAsync(BluetoothRobotConstants.BATTERY_LEVEL_REPORT_CHARACTERISTIC_UUID);
            return BaseService.ConvertIntFromBytes(data);
        }
    }
}
