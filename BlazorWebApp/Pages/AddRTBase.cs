using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SharedModels;
using Microsoft.JSInterop;

namespace BlazorWebApp.Pages
{

    public class AddRTBase : ComponentBase
    {
        protected IBrowserFile? file;

        protected string message = "";

        protected string base64Image { get; set; } = string.Empty;

        protected string? myAPIMessage { get; set; }
        protected string apiBase { get; set; } = "http://127.0.0.1:5026";

        protected string apiRoute { get; set; } = "/RememberThis/rtMulti";

        public string apiUrl { get; set; } = "";
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

        [Inject]
        protected IJSRuntime jsRuntime { get; set; } 

        protected async Task ShowAlert()
        {
            await jsRuntime.InvokeVoidAsync("ShowAlert", message);
        }
        protected async Task LoadFile2(InputFileChangeEventArgs e)
        {
            int dog = 0;
            dog++;
            // await jsRuntime.InvokeVoidAsync("loadFileJS", e);
        }
        protected void SubmitForm()
        {
            int dog = 0;
            dog++;

        }
        protected async Task LoadFile(InputFileChangeEventArgs e)
        {
            var format = "image/png";
            //    var resizedImage = await e.File.RequestImageFileAsync(format, 200,200);           
            //    var buffer = new byte[resizedImage.Size];
            //    await resizedImage.OpenReadStream().ReadAsync(buffer);

            //    var buffer = new byte[e.File.Size];
            //    await e.File.OpenReadStream().ReadAsync(buffer);
            //    base64Image = $"data:{format};base64,{Convert.ToBase64String(buffer)}";

            //    var buffer = new byte[e.File.Size];
            //    var stream = e.File.OpenReadStream(1024 * 1024 * 10);           
            //    await stream.ReadAsync(buffer);
            //    base64Image = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            //    stream.Close();

            // var byteArray = new byte[e.File.Size];
            // var stream = e.File.OpenReadStream(1024 * 1024 * 10);   
            // var ms = new MemoryStream();
            // await stream.CopyToAsync(ms);        
            // //await stream.ReadAsync(buffer);
            // byteArray = ms.ToArray();
            // base64Image = $"data:{format};base64,{Convert.ToBase64String(byteArray)}";
            // stream.Close();
            // ms.Close();

            int maxImageSize = 1024 * 1024 * 10; // 10 mb
            var byteArray = new byte[e.File.Size];
            var ms = new MemoryStream();
            await e.File.OpenReadStream(maxImageSize).CopyToAsync(ms);
            base64Image = $"data:{format};base64,{Convert.ToBase64String(ms.ToArray())}";
            ms.Close();




            //file.ContentType = "image/png";
            //file = e.File;


        }

    }

}