
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Net.Http.Json;

using MailApp.State;
using MailApp.Models;
using MudBlazor;

namespace MailApp.Client.Pages;

public partial class Enter : ComponentBase
{
    [Inject]
    UserState UserState { get; set; }
    [Inject]
    IJSRuntime JS { get; set; } = default!;
    [Inject]
    HttpClient HttpClient { get; set; } = default!;
    [Inject]
    ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    NavigationManager Navigation { get; set; } = default!;

    bool success;
    EnterAccountForm model = new();

    public class EnterAccountForm
    {
        [Required]
        [StringLength(20, ErrorMessage = "Name length can't be more than 20.")]
        public string Username { get; set; }
    }

    private async Task OnValidSubmit(EditContext context)
    {
        HttpResponseMessage response = await HttpClient.GetAsync($"User/GetUser?username={model.Username}");
        if (response.IsSuccessStatusCode)
        {
            User user = await response.Content.ReadFromJsonAsync<User>();
            UserState.Id = user.Id;
            UserState.Name = user.UserName;
            Navigation.NavigateTo("");
        }
        else
        {
            string msg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"An error occurred when receiving users. {msg}", Severity.Error);
        }
    }
}