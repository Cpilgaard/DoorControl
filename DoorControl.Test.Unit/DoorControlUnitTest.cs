using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace DoorControl.Test.Unit
{
    [TestFixture]
    public class DoorControlUnitTest
    {
        private DoorControl _uut;
        private IUserValidation _fakeUserValidation;
        private IAlarm _fakeAlarm;
        private IEntryNotification _fakeEntryNotification;
        private IDoor _fakeDoor;

        [SetUp]
        public void SetUp()
        {
            _fakeUserValidation = Substitute.For<IUserValidation>();
            _fakeAlarm = Substitute.For<IAlarm>();
            _fakeEntryNotification = Substitute.For<IEntryNotification>();
            _fakeDoor = Substitute.For<IDoor>();
            _uut = new DoorControl(_fakeUserValidation, _fakeDoor, _fakeEntryNotification, _fakeAlarm);
        }

        [Test]

        public void RequestEntry_ValidationIsOK_DoorOpenRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(true);

            //Act
            _uut.RequestEntry(23);

            //Assert
            _fakeDoor.Received().Open();
        }


        [Test]
        public void RequestEntry_ValidationIsOK_NotifyEntryGrantedRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(true);

            //Act
            _uut.RequestEntry(23);

            //Assert
            _fakeEntryNotification.Received().NotifyEntryGranted();
        }

        [Test]
        public void RequestEntry_ValidationIsOK_NotifyEntryDeniedNotRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(true);

            //Act
            _uut.RequestEntry(23);

            //Assert
            _fakeEntryNotification.DidNotReceive().NotifyEntryDenied();
        }

        [Test]
        public void RequestEntry_ValidationIsNotOK_DoorOpenWasNotRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(false);

            //Act
            _uut.RequestEntry(23);

            //Assert
            _fakeDoor.DidNotReceive().Open();
        }


        [Test]
        public void RequestEntry_ValidationIsNotOK_NotifyEntryGrantedNotRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(false);

            //Act
            _uut.RequestEntry(23);

            //Assert
            _fakeEntryNotification.DidNotReceive().NotifyEntryGranted();
        }

        [Test]
        public void RequestEntry_ValidationIsOK_NotifyEntryDeniedRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(false);

            //Act
            _uut.RequestEntry(23);

            //Assert
            _fakeEntryNotification.Received().NotifyEntryDenied();
        }

        [Test]
        public void DoorOpened_DoorStateIsOpening_DoorCloseRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(true);
            _uut.RequestEntry(23);

            //Act
            _uut.DoorOpened();

            //Assert
            _fakeDoor.Received().Close();
        }

        [Test]
        public void DoorOpened_DoorStateIsOpening_DoorOpenNotRecieved()
        {
            //Arrange
            _fakeUserValidation.ValidateEntryRequest(Arg.Any<int>()).Returns(true);
            _uut.RequestEntry(23);
            _fakeDoor.ClearReceivedCalls();

            //Act
            _uut.DoorOpened();

            //Assert
            _fakeDoor.DidNotReceive().Open();
        }

        [Test]
        public void DoorOpened_DoorStateIsClosed_AlarmSignalAlarmRecieved()
        {
            //Act
            _uut.DoorOpened();

            //Assert
            _fakeAlarm.Received().SignalAlarm();
        }

        [Test]
        public void DoorOpened_DoorStateIsClosed_DoorCloseRecieved()
        {
            //Act
            _uut.DoorOpened();

            //Assert
            _fakeDoor.Received().Close();
        }

        [Test]
        public void DoorOpened_DoorStateIsClosed_DoorOpenNotRecieved()
        {
            //Act
            _uut.DoorOpened();

            //Assert
            _fakeDoor.DidNotReceive().Open();
        }

    }
}
