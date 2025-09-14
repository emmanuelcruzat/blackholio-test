using System;
using System.Collections.Generic;
using System.Linq;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const int SEND_UPDATES_PER_SEC = 20;
    const float SEND_UPDATES_FREQUENCY = 1f / SEND_UPDATES_PER_SEC;

    public static PlayerController Local { get; private set; }

    private uint PlayerId;
    private float LastMovementSendTimestamp;
    private Vector2? LockInputPosition;
    private List<CircleController> OwnedCircles = new List<CircleController>();

    public string Username => GameManager.Conn.Db.Player.PlayerId.Find(PlayerId).Name;
    public int NumberOfOwnedCircles => OwnedCircles.Count;
    public bool IsLocalPlayer => this == Local;

    public void Initialize(Player player)
    {
        PlayerId = player.PlayerId;
        if (player.Identity == GameManager.LocalIdentity)
        {
            Local = this;
        }
    }

    private void OnDestroy()
    {
        // If we have any circles, destroy them
        foreach (var circle in OwnedCircles)
        {
            if (circle != null)
            {
                Destroy(circle.gameObject);
            }
        }
        OwnedCircles.Clear();
    }

    public void OnCircleSpawned(CircleController circle)
    {
        OwnedCircles.Add(circle);
    }

    public void OnCircleDeleted(CircleController deletedCircle)
    {
        // This means we got eaten
        if (OwnedCircles.Remove(deletedCircle) && IsLocalPlayer && OwnedCircles.Count == 0)
        {
            // DeathScreen.Instance.SetVisible(true);
        }
    }

    public uint TotalMass()
    {
        return (uint)OwnedCircles
            .Select(circle => GameManager.Conn.Db.Entity.EntityId.Find(circle.EntityId))
            .Sum(e => e?.Mass ?? 0); //If this entity is being deleted on the same frame that we're moving, we can have a null entity here.
    }

    public Vector2? CenterOfMass()
    {
        if (OwnedCircles.Count == 0)
        {
            return null;
        }

        Vector2 totalPos = Vector2.zero;
        float totalMass = 0;
        foreach (var circle in OwnedCircles)
        {
            var entity = GameManager.Conn.Db.Entity.EntityId.Find(circle.EntityId);
            var position = circle.transform.position;
            totalPos += (Vector2)position * entity.Mass;
            totalMass += entity.Mass;
        }

        return totalPos / totalMass;
    }

    private void OnGUI()
    {
        if (!IsLocalPlayer || !GameManager.IsConnected())
        {
            return;
        }

        GUI.Label(new Rect(0, 0, 100, 50), $"Total Mass: {TotalMass()}");
    }

    //Automated testing members
    private bool testInputEnabled;
    private Vector2 testInput;

    public void SetTestInput(Vector2 input) => testInput = input;
    public void EnableTestInput() => testInputEnabled = true;

    public void Update()
    {
        if (!IsLocalPlayer || NumberOfOwnedCircles == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (LockInputPosition.HasValue)
            {
                LockInputPosition = null;
            }
            else
            {
                LockInputPosition = (Vector2)Input.mousePosition;
            }
        }

        //shooting logic
        if (Input.GetMouseButtonDown(0)) // left click
        {
            Debug.Log("Fired Garand");
        }


        // Throttled input requests
        if (Time.time - LastMovementSendTimestamp >= SEND_UPDATES_FREQUENCY)
        {
            LastMovementSendTimestamp = Time.time;

            /*var mousePosition = LockInputPosition ?? (Vector2)Input.mousePosition;
            var screenSize = new Vector2
            {
                x = Screen.width,
                y = Screen.height,
            };
            var centerOfScreen = screenSize / 2;

            var direction = (mousePosition - centerOfScreen) / (screenSize.y / 3);
            */
            // Get input from A/D keys or Left/Right arrows (-1.0 to 1.0)
        float horizontalInput = Input.GetAxis("Horizontal");

        // Get input from W/S keys or Up/Down arrows (-1.0 to 1.0)
        float verticalInput = Input.GetAxis("Vertical");

        // Combine them into a single direction vector
        var direction = new Vector2(horizontalInput, verticalInput);
            if (testInputEnabled) { direction = testInput; }
            if (direction != Vector2.zero)
        {
            GameManager.Conn.Reducers.UpdatePlayerInput(direction);
        }
            
        }
    }

}