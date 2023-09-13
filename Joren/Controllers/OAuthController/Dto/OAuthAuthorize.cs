using System.Web;

namespace Joren.Controllers.OAuth.Dto;

public class OAuthAuthorizeRequest
{
	public string response_type { get; set; } = default!;
	public string client_id { get; set; } = default!;
	public string? redirect_uri { get; set; }
	public string? scope { get; set; }
	public string? state { get; set; }

	static (OAuthAuthorizeRequest? request, Exception? exception) FromQueryString(string query)
	{
		var queryCollection = HttpUtility.ParseQueryString(query);

		var response_type = queryCollection.Get("response_type");
		if (response_type == null)
		{
			return (null, new OAuthAuthorizeParseException("response_type is required"));
		}

		var client_id = queryCollection.Get("client_id");
		if (client_id == null)
		{
			return (null, new OAuthAuthorizeParseException("client_id is required"));
		}

		return (new OAuthAuthorizeRequest
		{
			response_type = response_type,
			client_id = client_id,
			redirect_uri = queryCollection.Get("redirect_uri"),
			scope = queryCollection.Get("scope"),
			state = queryCollection.Get("state")
		}, null);
	}
}

public class OAuthAuthorizeParseException : Exception
{
	public OAuthAuthorizeParseException(string message) : base(message)
	{
	}
}

public class OAuthAuthorizeRedirectQuery
{
	public string code { get; set; } = default!;
	public string? state { get; set; }

	public string ToQueryString()
	{
		var query = HttpUtility.ParseQueryString(string.Empty);
		query.Add("code", code);
		if (state != null)
		{
			query.Add("state", state);
		}
		return query.ToString();
	}
}

public class OAuthAuthorizeRedirectErrorQuery
{
	public OAuthAuthorizedErrorTypeString error { get; set; } = default!;
	public string? error_description { get; set; }
	public string? error_uri { get; set; }
	public string? state { get; set; }

	public string ToQueryString()
	{
		var query = HttpUtility.ParseQueryString(string.Empty);
		query.Add("error", error.ToString());
		if (error_description != null)
		{
			query.Add("error_description", error_description);
		}
		if (error_uri != null)
		{
			query.Add("error_uri", error_uri);
		}
		if (state != null)
		{
			query.Add("state", state);
		}
		return query.ToString();
	}
}

public enum OAuthAuthorizeErrorType : byte
{
	InvalidRequest,
	UnauthorizedClient,
	AccessDenied,
	UnsupportedResponseType,
	InvalidScope,
	ServerError,
	TemporaryUnavailable
}

public class OAuthAuthorizedErrorTypeString
{
	private readonly OAuthAuthorizeErrorType _type;


	public OAuthAuthorizedErrorTypeString(OAuthAuthorizeErrorType type)
	{
		_type = type;
	}

	public override string ToString()
	{
		return _type switch
		{
			OAuthAuthorizeErrorType.InvalidRequest => "invalid_request",
			OAuthAuthorizeErrorType.UnauthorizedClient => "unauthorized_client",
			OAuthAuthorizeErrorType.AccessDenied => "access_denied",
			OAuthAuthorizeErrorType.UnsupportedResponseType => "unsupported_response_type",
			OAuthAuthorizeErrorType.InvalidScope => "invalid_scope",
			OAuthAuthorizeErrorType.ServerError => "server_error",
			OAuthAuthorizeErrorType.TemporaryUnavailable => "temporarily_unavailable",
			_ => throw new Exception("unknown type")
		};
	}
}