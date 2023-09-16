using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArcanepadSDK;
using ArcanepadSDK.Models;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<PlayerCarController> players = new List<PlayerCarController>();
    public bool gameStarted { get; private set; }
    public static bool isGamePaused = false;
    async void Start()
    {
        var initialState = await Arcane.ArcaneClientInitialized();

        initialState.pads.ForEach(pad =>
        {
            createPlayer(pad);
        });

        Arcane.Msg.On(AEventName.OpenArcaneMenu, () =>
        {
            isGamePaused = true;
        });
        Arcane.Msg.On(AEventName.CloseArcaneMenu, () =>
        {
            isGamePaused = false;
        });

        Arcane.Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
        {
            var playerExists = players.Any(p => p.Pad.IframeId == e.iframeId);
            if (playerExists) return;

            var pad = new ArcanePad(deviceId: e.deviceId, internalId: e.internalId, iframeId: e.iframeId, isConnected: true,
            user: Arcane.Devices.FirstOrDefault(d => d.id == e.deviceId).user);

            createPlayer(pad);
        });

        Arcane.Msg.On(AEventName.IframePadDisconnect, (IframePadDisconnectEvent e) =>
        {
            var player = players.FirstOrDefault(p => p.Pad.IframeId == e.IframeId);

            if (player == null) Debug.LogError("Player not found to remove on disconnect");
            destroyPlayer(player);
        });

    }

    void createPlayer(ArcanePad pad)
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerCarController playerComponent = newPlayer.GetComponent<PlayerCarController>();
        playerComponent.Initialize(pad);

        players.Add(playerComponent);

        UpdateCameraViewports();
    }

    void destroyPlayer(PlayerCarController playerComponent)
    {
        playerComponent.Pad.Dispose();
        players.Remove(playerComponent);
        Destroy(playerComponent.gameObject);

        UpdateCameraViewports();
    }

    void UpdateCameraViewports()
    {
        if (players.Count == 1)
        {
            players[0].playerCamera.rect = new Rect(0, 0, 1, 1); // Full screen
        }
        else if (players.Count == 2)
        {
            players[0].playerCamera.rect = new Rect(0, 0, 0.5f, 1); // Left half of the screen
            players[1].playerCamera.rect = new Rect(0.5f, 0, 0.5f, 1); // Right half of the screen
        }
    }

}
