using Microsoft.AspNetCore.Mvc;

namespace Joren.Controllers.OAuth;



[ApiController]
[Route("/oauth/[controller]")]
public class OAuthController : ControllerBase
{
	private readonly ILogger<OAuthController> _logger;

	public OAuthController(ILogger<OAuthController> logger)
	{
		_logger = logger;
	}

	[HttpGet("authorize", Name = "Authorize")]
	[ProducesResponseType(StatusCodes.Status302Found)]
	public IActionResult Authorize([FromBody] Dto.OAuthAuthorizeRequest request)
	{
		var result = new Dto.OAuthAuthorizeRedirectQuery
		{
			code = "1234567890",
			state = request.state
		};

		// TODO リダイレクトURLはサーバーに登録されているものを優先する
		var url = new System.UriBuilder(request.redirect_uri)
		{
			Query = result.ToQueryString()
		};
		return Redirect(url.ToString());
	}

}