using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebLab.Services.BeerTypeService;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;
using WebLab.Services.BeerService;
using WebLab.Controllers;

namespace WebLab.Tests
{
    public class BeerControllerTests
    {
        [Fact]
        public async Task Index_BeerTypesNotRecieved_ReturnsNotFound()
        {
            var beerTypeServiceMock = new Mock<IBeerTypeService>();
            beerTypeServiceMock.Setup(s => s.GetBeerTypeListAsync())
                .ReturnsAsync(new ResponseData<List<BeerType>>(null) { IsSuccess = false, ErrorMessage = "ERROR" });

            var beerServiceMock = new Mock<IBeerService>();

            var controller = new BeerController(beerServiceMock.Object, beerTypeServiceMock.Object);

            var result = await controller.Index(null, default);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Index_BeersNotReceived_ReturnsNotFound()
        {
            var beerTypeServiceMock = new Mock<IBeerTypeService>();
            beerTypeServiceMock.Setup(s => s.GetBeerTypeListAsync())
                .ReturnsAsync(new ResponseData<List<BeerType>>(default));

            var beerServiceMock = new Mock<IBeerService>();
            beerServiceMock.Setup(s => s.GetBeerListAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResponseData<ListModel<Beer>>(null) { IsSuccess = false, ErrorMessage = "Error" });

            var controller = new BeerController(beerServiceMock.Object, beerTypeServiceMock.Object);

            var result = await controller.Index(null, default);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Index_CorrectAction()
        {
            var expectedBeerType = new BeerType { Id = 1, Name = "Beer", NormalizedName = "beer" };
            var expectedBeerTypes = new List<BeerType>() { expectedBeerType };

            var beerTypeServiceMock = new Mock<IBeerTypeService>();
            beerTypeServiceMock.Setup(s => s.GetBeerTypeListAsync())
                .ReturnsAsync(new ResponseData<List<BeerType>>(expectedBeerTypes));

            var beerMocks = new List<Beer>() { new Beer { Id = 1, Name = "Beer", Description = "Description", Type = expectedBeerType, TypeId = expectedBeerType.Id, Price = 10 } };

            var beerServiceMock = new Mock<IBeerService>();
            beerServiceMock.Setup(s => s.GetBeerListAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResponseData<ListModel<Beer>>(new ListModel<Beer>(beerMocks)));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Request.Headers)
                .Returns(new HeaderDictionary { { "X-Requested-With", "XMLHttpRequest" } });

            var controller = new BeerController(beerServiceMock.Object, beerTypeServiceMock.Object) { ControllerContext = new() { HttpContext = httpContextMock.Object } };

            var result = await controller.Index(expectedBeerType.NormalizedName, default);

            Assert.Equal(expectedBeerTypes, controller.ViewData["beerTypes"]);
            Assert.Equal(expectedBeerType.NormalizedName, controller.ViewData["beerType"]);
            Assert.IsType<ListModel<Beer>>(controller.ViewData.Model);
        }
    }
}
