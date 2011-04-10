using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
        public void Create_Returns_New_ClientEditViewModel()
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
        public void Edit_GivenValidID_Returns_ClientEditViewModel()
        {
            // Arrange
            int validId = 1;
            var logger = new Mock<ILogger>();

            var repository = new Mock<IRepository>();
            Client client = new Client {ClientId = validId};

            ClientsController clientsController = new ClientsController(repository.Object, logger.Object);

            // Act
            ActionResult result = clientsController.Edit(validId);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(ClientEditViewModel), ((ViewResult)result).Model);
            Assert.AreEqual(validId, ((ClientEditViewModel)((ViewResult)result).Model).ClientId);
        }

    }
}
