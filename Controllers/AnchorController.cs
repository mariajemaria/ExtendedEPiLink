using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using MarijasPlayground.ExtendedLink;

namespace MarijasPlayground.Controllers
{
    [RoutePrefix("api/tiny/anchors")]
    public class AnchorController : ApiController
    {
        [System.Web.Mvc.HttpGet]
        [ResponseType(typeof(List<AnchorHeadingPair>))]
        [Route("frompageid/{pageIdWithVersion}")]
        public IHttpActionResult GetAnchorsForTiny(string pageIdWithVersion)
        {
            var anchors = AnchorService.GetOrSetFromPageIdForTinyMce(pageIdWithVersion);
            return Ok(anchors);
        }

        [System.Web.Mvc.HttpGet]
        [ResponseType(typeof(List<AnchorHeadingPair>))]
        [Route("fromurl")]
        public IHttpActionResult GetAnchorsForLinkBlock(string url)
        {
            var anchors = AnchorService.GetOrSetFromUrlForLinkBlock(url);
            return Ok(anchors);
        }
    }
}