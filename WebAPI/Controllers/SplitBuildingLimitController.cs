using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Features;
using SplitBuildingLimits;
using System.Net;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SplitBuildingLimitController : Controller
    {
        /// <summary>
        /// Post request
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="buildingLimits"></param>
        /// <param name="heightPlateaus"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostBuildingLimitsAndHeightPlateaus/{projectId}")]
        public HttpResponseMessage PostBuildingLimitsAndHeightPlateaus(string projectId, [FromBody]PostModel postModel)
        {
            try
            {
                if (Guid.TryParse(projectId, out Guid Id))
                {
                    SplitBuildingLimits.SplitBuildingLimits.Process(postModel.BuildingLimits, postModel.HeightPlateaus, Id);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                } else
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("The projectId passed in is invalid, please try again"),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
            } catch (Exception e)
            {
                if(e is ArgumentException || e is ArgumentNullException)
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent(e.Message)
                    };
                }

                // Track this exception
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("An internal server error has occured. We will investigate the error")
                };
            }
        }

        /// <summary>
        /// Get request
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="propertyName"></param>
        /// <returns>Property values</returns>
        [HttpGet]
        [Route("GetValueForProperty/{projectId}/{propertyName}")]
        public HttpResponseMessage HttpResponseMessage(string projectId, string propertyName)
        {
            try
            {
                if (Guid.TryParse(projectId, out Guid Id))
                {
                    string value = SplitBuildingLimits.SplitBuildingLimits.Get(Id, propertyName);
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(value),
                        StatusCode = HttpStatusCode.OK
                    };
                }
                else
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("The projectId passed in is invalid, please try again"),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is ArgumentNullException)
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent(e.Message)
                    };
                }

                // An internal error has occured. Make sure to track this exception using 
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("An internal server error has occured. We will investigate the error")
                };
            }
        }
    }
}
