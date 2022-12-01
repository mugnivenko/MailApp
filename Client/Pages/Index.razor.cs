using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using MudBlazor;

using MailApp.Models;
using MailApp.State;

namespace MailApp.Client.Pages;

public partial class Index : ComponentBase
{
    private HubConnection? hubConnection;

    private List<User> Users { get; set; } = new List<User>();

    bool success;
    MudForm form;

    private string? _selectedUser;
    private string? _title;
    private string? _body;

    [Inject]
    NavigationManager Navigation { get; set; } = default!;
    [Inject]
    IJSRuntime JS { get; set; } = default!;
    [Inject]
    HttpClient HttpClient { get; set; } = default!;
    [Inject]
    ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    UserState UserState { get; set; }


    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        List<User> users = await GetUsers(value, token);
        return users.Select((user) => user.UserName).ToArray();
    }

    private async Task<List<User>> GetUsers(string value, CancellationToken token)
    {
        HttpResponseMessage response = await HttpClient.GetAsync($"User/GetUsers?username={value ?? string.Empty}", token);
        if (response.IsSuccessStatusCode)
        {
            List<User> users = await response.Content.ReadFromJsonAsync<List<User>>() ?? new List<User>();
            Users = users;
            return users;
        }
        else
        {
            string msg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"An error occurred when receiving users. {msg}", Severity.Error);
            return new List<User>();
        }
    }

    private async Task OnSubmit()
    {
        Guid recipientUserId = Users.Where((user) => user.UserName == _selectedUser).First().Id;
        await SendMail(recipientUserId);
    }

    private async Task SendMail(Guid recipientUserId)
    {
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(
            "Mail/SaveMail",
            new { SenderId = UserState.Id, RecipientId = recipientUserId, Title = _title, Body = _body }
        );
        if (response.IsSuccessStatusCode)
        {
            await Send(recipientUserId);
            form.Reset();
            Snackbar.Add("Mail successfully sent", Severity.Success);
        }
        else
        {
            string msg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"An error occurred when sending the mail: {msg}", Severity.Success);
        }
        await JS.InvokeVoidAsync("console.log", response);
    }


    protected override async Task OnInitializedAsync()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/mailhub"))
            .Build();


        hubConnection.On<string, Guid, string>("ReceiveMessage", (senderName, recipientUserId, messageTitle) =>
        {
            if (UserState.Id == recipientUserId)
            {
                Snackbar.Add(
                    $"User {senderName} sent you a message with the subject \"{messageTitle}\"",
                    Severity.Info,
                    config =>
                    {
                        config.Action = "Show";
                        config.ActionColor = Color.Primary;
                        config.Onclick = snackbar =>
                        {
                            Navigation.NavigateTo("mymail");
                            return Task.CompletedTask;
                        };
                    }
                );

            }
            StateHasChanged();
        });

        await hubConnection.StartAsync();
        UserState.OnChange += StateHasChanged;
    }

    private async Task Send(Guid recipientUserId)
    {
        if (hubConnection is not null)
        {
            await hubConnection.SendAsync("SendMail", UserState.Name, recipientUserId, _title);
        }
    }

    public bool IsConnected =>
        hubConnection?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
        UserState.OnChange -= StateHasChanged;
    }
}