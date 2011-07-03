using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using AWC.Domain.Abstract;
using AWC.Domain.Entities;
using AWC.WebUI.Controllers;
using AWC.WebUI.Infrastructure.Logging;
using AWC.WebUI.Models;
using Moq;
using NUnit.Framework;

namespace AWC.WebUI.Tests.Unit
{
    [TestFixture]
    public class ClientsControllerTests
    {
        [Test]
        public void Should_Get_Blank_Form_To_Create_New_Client()
        {
            // Arrange
            var repository = new Mock<IRepository>();
            var logger = new Mock<ILogger>();
            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);
            
            // Act
            ViewResult result = (ViewResult)clientsController.Create();

            // Assert
            Assert.IsInstanceOf(typeof(ClientEditViewModel), result.Model);
        }

        [Test]
        public void Should_Display_Client_Info_Given_Valid_ClientID()
        {
            // Arrange
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();
            Client client = new Client {ClientId = 1};

            repository.Setup(s=> s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(client);

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            ActionResult result = clientsController.BasicInfo(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(ClientEditViewModel), ((ViewResult)result).Model);
            Assert.AreEqual(1, ((ClientEditViewModel)((ViewResult)result).Model).ClientId);
        }

        [Test]
        public void Should_Save_New_Client_And_Redirect_To_Edit_Screen_If_Validation_Passes()
        {
            // Arrange
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();
            Client client = new Client
                                {
                                    ClientId = 0,
                                    FirstName = "Michael",
                                    LastName = "Cox",
                                    AddressLine1 = "123 Fake St",
                                    City = "Auburn",
                                    StateCode = "MA",
                                    CountyCode = "PG",
                                    NumberOfAdults = 2,
                                    NumberOfChildren = 2
                                };

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            RedirectToRouteResult result = clientsController.Create(client) as RedirectToRouteResult;

            // Assert
            Assert.IsTrue(clientsController.ModelState.IsValid);
            repository.Verify(v => v.Add(It.IsAny<Client>()), Times.Once(), "Should add new client.");
            repository.Verify(v => v.CommitChanges(), Times.Once(), "Should save data to repository.");
            Assert.AreEqual("BasicInfo", result.RouteValues["action"]);
        }

        [Test]
        public void Should_Edit_Client_And_Redirect_To_Edit_Screen_If_Validation_Passes()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            
            var repository = new Mock<IRepository>();
            Client client = new Client
            {
                ClientId = 1,
                FirstName = "Michael",
                LastName = "Cox",
                AddressLine1 = "123 Fake St",
                City = "Auburn",
                StateCode = "MA",
                CountyCode = "PG",
                NumberOfAdults = 2,
                NumberOfChildren = 2
            };

            Client existingClient = new Client {ClientId = 1};
            repository.Setup(s => s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(existingClient);

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            RedirectToRouteResult result = clientsController.BasicInfo(client) as RedirectToRouteResult;

            // Assert
            Assert.IsTrue(clientsController.ModelState.IsValid);
            repository.Verify(v => v.CommitChanges(), Times.Once(), "Should save data to repository.");
            Assert.AreEqual("BasicInfo", result.RouteValues["action"]);
        }

        [Test]
        public void Should_Be_Able_To_Add_PartnerOrgInfo_To_Client()
        {
            // Arrange
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();

            var partnerInfoViewModel = new PartnerInfoViewModel
            {
                CaseworkerId = 0,
                ClientId = 1,
                FirstName = "Jane",
                LastName = "Doe",
                PartneringOrgId = 1,
                IsReplacingFurniture = true
            };

            Client existingClient = new Client
            {
                ClientId = 1
            };

            repository.Setup(s => s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(existingClient);

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            var result = clientsController.PartnerInfo(partnerInfoViewModel) as RedirectToRouteResult;

            // Assert
            Assert.IsTrue(clientsController.ModelState.IsValid);
            repository.Verify(v => v.CommitChanges(), Times.Exactly(2), "Should save data to repository.");
            Assert.AreEqual("PartnerInfo", result.RouteValues["action"]);
        }

        [Test]
        public void Should_Return_Validation_Errors_If_Invalid_Client_Data_Submitted()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var repository = new Mock<IRepository>();

            Client client = new Client { ClientId = 1 };
            repository.Setup(s => s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(client);

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            clientsController.ModelState.AddModelError("", "Dummy error message.");

            // Act
            ViewResult result = clientsController.BasicInfo(new Client()) as ViewResult;

            // Assert
            Assert.IsFalse(clientsController.ModelState.IsValid);
            repository.Verify(v => v.CommitChanges(), Times.Never(), "Should not update the database with validation errors.");
        }

        [Test]
        public void Should_Be_Able_To_Add_A_Note_To_A_Client_Record()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var repository = new Mock<IRepository>();

            var client = new Client { ClientId = 1 };
            repository.Setup(s => s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(client);

            var clientsController = new ClientsController(repository.Object, logger.Object);

            var clientNote = new ClientNote
                                        {
                                            Body = "Note Information",
                                            ClientId = 1
                                        };

            // Act
            var result = clientsController.AddNote(clientNote, "BasicInfo") as ViewResult;

            // Assert
            Assert.IsTrue(clientsController.ModelState.IsValid);
            repository.Verify(v => v.Add(clientNote), Times.Once());
            repository.Verify(v => v.CommitChanges(), Times.Once());            

        }

        [Test]
        public void Should_Set_CreateDate_LastUpdateDate_On_Create()
        {
            // Arrange
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();
            Client client = new Client
            {
                ClientId = 0,
                FirstName = "Michael",
                LastName = "Cox",
                AddressLine1 = "123 Fake St",
                City = "Auburn",
                StateCode = "MA",
                CountyCode = "PG",
                NumberOfAdults = 2,
                NumberOfChildren = 2
            };

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            RedirectToRouteResult result = clientsController.Create(client) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual(DateTime.UtcNow, client.CreatedDateTime);
            Assert.AreEqual(DateTime.UtcNow, client.LastUpdatedDateTime);
        }

        [Test]
        public void Should_Set_Blank_Appointment_On_Create()
        {
            // Arrange
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();
            var client = new Client
            {
                ClientId = 0,
                FirstName = "Michael",
                LastName = "Cox",
                AddressLine1 = "123 Fake St",
                City = "Auburn",
                StateCode = "MA",
                CountyCode = "PG",
                NumberOfAdults = 2,
                NumberOfChildren = 2
            };

            var clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            var result = clientsController.Create(client) as RedirectToRouteResult;

            // Assert
            repository.Verify(r => r.Add(It.IsAny<Appointment>()), Times.Once(), "Should create blank appointment on first create.");
            repository.Verify(r => r.CommitChanges(), Times.Once(), "Should persist data to database.");
        }

        [Test]
        public void Should_Set_LastUpdateDate_On_Edit()
        {
            // Arrange
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();
            Client client = new Client
            {
                ClientId = 1,
                FirstName = "Michael",
                LastName = "Cox",
                AddressLine1 = "123 Fake St",
                City = "Auburn",
                StateCode = "MA",
                CountyCode = "PG",
                NumberOfAdults = 2,
                NumberOfChildren = 2
            };

            Client existingClient = new Client
                                        {
                                            ClientId = 1,
                                            CreatedDateTime = new DateTime(2010, 1, 1, 6, 0, 0, DateTimeKind.Utc),
                                            LastUpdatedDateTime = new DateTime(2010, 1, 1, 6, 0, 0, DateTimeKind.Utc)
                                        };

            repository.Setup(s => s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(existingClient);

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            var result = clientsController.BasicInfo(client) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual(DateTime.UtcNow.ToString(), existingClient.LastUpdatedDateTime.ToString());
            Assert.AreEqual(new DateTime(2010, 1, 1, 6, 0, 0, DateTimeKind.Utc), existingClient.CreatedDateTime);
        }

        [Test]
        public void Should_Set_LastUpdateDate_On_PartnerInfo_Edit()
        {
            // Arrange
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();

            var partnerInfoViewModel = new PartnerInfoViewModel
            {
                CaseworkerId = 0,
                ClientId = 1,
                FirstName = "Jane",
                LastName = "Doe",
                PartneringOrgId = 1,
                IsReplacingFurniture = true
            };

            Client existingClient = new Client
            {
                ClientId = 1,
                CreatedDateTime = new DateTime(2010, 1, 1, 6, 0, 0, DateTimeKind.Utc)
            };

            repository.Setup(s => s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(existingClient);

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            var result = clientsController.PartnerInfo(partnerInfoViewModel) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual(DateTime.UtcNow.ToString(), existingClient.LastUpdatedDateTime.ToString());
            Assert.AreEqual(new DateTime(2010, 1, 1, 6, 0, 0, DateTimeKind.Utc), existingClient.CreatedDateTime);
        }
    }
}
