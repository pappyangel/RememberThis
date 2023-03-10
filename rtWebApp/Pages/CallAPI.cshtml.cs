using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedModels;



namespace rtWebApp.Pages;

public class CallAPIModel : PageModel
{
    private HttpClient APIclient = new HttpClient();
    private readonly ILogger<CallAPIModel> _logger;

    public CallAPIModel(ILogger<CallAPIModel> logger)
    {
        _logger = logger;
    }

    public string? myAPIMessage { get; set; }    
    public string apiBase { get; set; } = "http://127.0.0.1:5026";
    
    [BindProperty]
    public string apiRoute { get; set; } = "/RememberThis/rtMulti";

    public string apiUrl { get; set; }    = "";

    [BindProperty]
    public IFormFile Upload { get; set; } = null!;
    
    [BindProperty]
    public rtItem thisrtItem { get; set; } = new rtItem{ rtId = 1001, rtUserName = "Cosmo", rtDescription = "fun time digging hole for bone",rtLocation = "backyard", rtDateTime =  DateTime.UtcNow };

    public void OnGet()
    {
    

        
    }

    public  IActionResult OnPost()
    {

        // logic to just read file from disk
        // string ReadResult = "NotYetRead";
        // string filename = "./Images/multipart.txt";        
        // using (var sr = new StreamReader(filename))
        //     {
        //         ReadResult = await sr.ReadToEndAsync();
        //     }
        // end -  logic to just read file from disk
        
        apiUrl = apiBase + apiRoute;

        // using   MemoryStream ms = new MemoryStream();

        
        // var myHttpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        // var myMultipartFormData = new MultipartFormDataContent();
        // ms.Position = 0;
        // var myStreamContent = new StreamContent(ms);

        // myStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(Upload.ContentType);  //Upload.ContentType = "image/jpg"

        // myMultipartFormData.Add(myStreamContent, "file", Upload.FileName);
        


         return Page();

    }




}
