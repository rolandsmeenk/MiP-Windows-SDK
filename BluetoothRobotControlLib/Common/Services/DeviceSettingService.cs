using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothRobotControlLib.Common.Services
{
    public class DeviceSettingService : BaseService
    {
        private BluetoothRobotConstants.ACTIVATION_STATUS activationStatus = BluetoothRobotConstants.ACTIVATION_STATUS.NOT_READ;

        public DeviceSettingService() : base(BluetoothRobotConstants.DEVICE_SETTING_SERVICE_UUID)
        {
        }

        public async Task<BluetoothRobotConstants.ACTIVATION_STATUS> ReadProductActivationStatus()
        {
            if (activationStatus == BluetoothRobotConstants.ACTIVATION_STATUS.NOT_READ)
            {
                byte[] data = await base.ReadChacateristicValueAsync(BluetoothRobotConstants.DEVICE_SETTING_PRODUCT_ACTIVIATION_CHARACTERISTIC_UUID);
                activationStatus = BaseService.ConvertEnumFromBytes<BluetoothRobotConstants.ACTIVATION_STATUS>(data);
            }

            return activationStatus;
        }

        public async Task<bool> WriteProductActivationStatus(BluetoothRobotConstants.ACTIVATION_STATUS status)
        {
            return await base.WriteCharacteristicValueAsync(BluetoothRobotConstants.DEVICE_SETTING_PRODUCT_ACTIVIATION_CHARACTERISTIC_UUID, new byte[] { (byte)status });
        }
    }
}
