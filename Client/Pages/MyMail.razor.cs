using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using MudBlazor;

using MailApp.State;
using MailApp.Models;
using MailApp.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace MailApp.Client.Pages;

public partial class MyMail : ComponentBase
{
    [Inject]
    NavigationManager Navigation { get; set; } = default!;
    [Inject]
    UserState UserState { get; set; }
    [Inject]
    HttpClient HttpClient { get; set; } = default!;
    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    private HubConnection? hubConnection;

    List<MailFromUserToUser> MailsWithSender { get; set; } = new List<MailFromUserToUser>();

    QueryState _state = QueryState.Idle;

    private async Task<List<MailFromUserToUser>> GetMailsWithSenderUsers()
    {
        _state = QueryState.Loading;
        HttpResponseMessage response = await HttpClient.GetAsync($"Mail/ReceivedUserMail?userId={UserState.Id}");
        if (response.IsSuccessStatusCode)
        {
            _state = QueryState.Success;
            return await response.Content.ReadFromJsonAsync<List<MailFromUserToUser>>() ?? new List<MailFromUserToUser>();
        }
        else
        {
            string msg = await response.Content.ReadAsStringAsync();
            Snackbar.Add($"Error: {msg}", Severity.Error);
            _state = QueryState.Fail;
            return new List<MailFromUserToUser>();
        }
    }

    private async void UpdateMail()
    {
        MailsWithSender = await GetMailsWithSenderUsers();
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        UserState.OnChange += StateHasChanged;
        if (UserState.Id is not null)
        {
            MailsWithSender.AddRange(await GetMailsWithSenderUsers());
        }
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
                        config.Action = "Update";
                        config.ActionColor = Color.Primary;
                        config.Onclick = snackbar =>
                        {
                            UpdateMail();
                            return Task.CompletedTask;
                        };
                    }
                );

            }
            StateHasChanged();
        });

        await hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
        UserState.OnChange -= StateHasChanged;
    }
}