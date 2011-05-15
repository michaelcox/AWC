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
            Assert.AreEqual("Edit", result.RouteValues["action"]);
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
            Assert.AreEqual("Edit", result.RouteValues["action"]);
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

            Client client = new Client { ClientId = 1 };
            repository.Setup(s => s.Single(It.IsAny<Expression<Func<Client, bool>>>())).Returns(client);

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            ClientNote clientNote = new ClientNote
                                        {
                                            ClientId = 1,
                                            Body = "Note information"
                                        };

            // Act
            ViewResult result = clientsController.AddNote(clientNote) as ViewResult;

            // Assert
            Assert.IsTrue(clientsController.ModelState.IsValid);
            repository.Verify(v => v.Add(clientNote), Times.Once());
            repository.Verify(v => v.CommitChanges(), Times.Once());            

        }
    }
}
