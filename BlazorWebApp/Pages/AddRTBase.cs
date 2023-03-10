using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SharedModels;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;

namespace BlazorWebApp.Pages
{

    public class AddRTBase : ComponentBase
    {
        protected IBrowserFile? file;
        protected string? InfoMsg { get; set; } = "API Return Message";
        protected string apiBase { get; set; } = "http://127.0.0.1:5026";

        protected string apiRoute { get; set; } = "/RememberThis/rtMulti";

        public string apiUrl { get; set; } = "";
        protected bool ShowPopUp { get; set; } = false;
        protected rtItem thisrtItem { get; set; } =
            new rtItem
            {
                rtId = 1001,
                rtUserName = "Cosmo",
                rtDescription = "fun time digging hole for bone",
                rtLocation = "backyard",
                rtDateTime = DateTime.UtcNow
            };
        // protected async Task SubmitForm()
        protected RTModalComponent childmodal { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [Inject]
        protected IHttpClientFactory ClientFactory { get; set; }

        [Inject]
        protected IConfiguration Config { get; set; }

        protected void DisplayBtnClicked(string _btnClicked)
        {
            InfoMsg = _btnClicked;

        }


        protected async Task SelectedFileProcess(InputFileChangeEventArgs e)
        {
            file = e.File;
            await jsRuntime.InvokeVoidAsync("loadFileJS");
        }
        protected async Task SubmitForm()
        {
            long _fileSizeLimit = Config.GetValue<long>("FileSizeLimit");

            using var content = new MultipartFormDataContent();
            var client = ClientFactory.CreateClient();

            //Add form data that bound to class into jsaon string via serialization
            string jsonString = JsonSerializer.Serialize(thisrtItem);
            var classContent = new StringContent(jsonString);
            content.Add(classContent, "classData");

            //Add file             
            if (!((file.Size > 0) && (file.Size < _fileSizeLimit)))
            {
                InfoMsg = "File size too large";
            }
            else
            {
                var ms = new MemoryStream();
                await file.OpenReadStream(1024 * 1024 * 10).CopyToAsync(ms);
                ms.Position = 0;
                var streamContent = new StreamContent(ms);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);

                content.Add(streamContent, "file", file.Name);

                try
                {
                    var response1 = await client.PostAsync("http://127.0.0.1:5197/RememberThis", content);

                    switch (response1.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            InfoMsg = await response1.Content.ReadAsStringAsync();
                            break;
                        case System.Net.HttpStatusCode.NoContent:
                            InfoMsg = "No content";
                            break;
                        case System.Net.HttpStatusCode.NotFound:
                            InfoMsg = "API Route not found!";
                            break;
                        case System.Net.HttpStatusCode.Forbidden:
                            InfoMsg = "Your Access to this API route is Forbidden!";
                            break;
                        case System.Net.HttpStatusCode.Unauthorized:
                            InfoMsg = "Your Access to this API route is Unauthorized!";
                            break;
                        default:
                            InfoMsg = "Unhandled Error!";
                            break;

                    }




                }
                catch (Exception Ex)
                {
                    // Opps!  Did we forget to start the API?!?
                    InfoMsg = "API not available";
                    // throw;
                }



                // childmodal.Open();


            }

        }

    }

}