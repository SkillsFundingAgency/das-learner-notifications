using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.LearnerNotifications.Api.Controllers
{
    [ApiController]
    [Route("learner/")]
    public class LearnerNotificationsController : ControllerBase
    {
        [HttpGet("{accountIdentifier}")]
        public IActionResult GetNotifications(Guid accountIdentifier)
        {
            return Ok(new { Message = "Placeholder – full implementation coming soon" });
        }

    }
}
