using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DoorControl
{
    public class DoorControl
    {
        private IUserValidation _userValidation;
        private IDoor _door;
        private IEntryNotification _entryNotification;
        private DoorState _doorState;
        private IAlarm _alarm;

        public DoorControl(IUserValidation userValidation, IDoor door, IEntryNotification entryNotification, IAlarm alarm)
        {
            _userValidation = userValidation;
            _door = door;
            _entryNotification = entryNotification;
            _alarm = alarm;
            _doorState = DoorState.Closed;
        }
        public void RequestEntry(int id)
        {
            if (_userValidation.ValidateEntryRequest(id))
            {
                _door.Open();
                _entryNotification.NotifyEntryGranted();
                _doorState = DoorState.Opening;
            }

            else _entryNotification.NotifyEntryDenied();
        }

        public void DoorOpened()
        {
            switch (_doorState)
            {
                case DoorState.Opening:
                    _door.Close();
                    _doorState = DoorState.Open;
                    break;
                case DoorState.Closed:
                    _door.Close();
                    _alarm.SignalAlarm();
                    _doorState = DoorState.Breached;
                    break;
            }
        }

        public void DoorClosed()
        {
            _doorState = DoorState.Closed;
        }
    }
}
