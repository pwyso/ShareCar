using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareCar;
using ShareCar.Controllers;
using System.Threading.Tasks;
using ShareCar.Models;

namespace ShareCar.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        //public void Index()
        public async Task CheckIfRetunIndexView()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        //public void Index()
        public async Task Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = await controller.Index() as ViewResult;
            var liftOffer = (LiftOffer)result.ViewData.Model;

            // Assert
            Assert.AreEqual(6, liftOffer.LiftOfferID);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your app description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
